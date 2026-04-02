using Cairo;
using GdkPixbuf;
using Gio;
using GLib;

namespace Platform.Maui.Linux.Gtk4.Graphics;

internal static class PixbufExtensions
{
	public static void PaintPixbuf(this Context context, Pixbuf pixbuf, double x = 0, double y = 0, double? width = default, double? height = default)
	{
		var translate = x != 0 || y != 0;
		var scale = width.HasValue && height.HasValue && (width != pixbuf.Width || height != pixbuf.Height);
		if (translate || scale)
			context.Save();
		if (translate)
			context.Translate(x, y);
		if (scale)
			context.Scale(width.Value / pixbuf.Width, height.Value / pixbuf.Height);
		Gdk.Functions.CairoSetSourcePixbuf(context, pixbuf, 0, 0);

		using var p = context.GetSource();
		if (p is SurfacePattern pattern)
		{
			// Fixes blur issue when rendering on an image surface
			pattern.Filter =
				width > pixbuf.Width || height > pixbuf.Height ? Filter.Fast : Filter.Good;
		}

		context.Paint();
		if (translate || scale)
			context.Restore();
	}

	public static Pixbuf? CreatePixbuf(this ImageSurface? surface)
	{
		if (surface == null)
			return null;

		var surfaceData = surface.GetData();
		var nbytes = surface.Format == Format.Argb32 ? 4 : 3;
		var pixData = new byte[surfaceData.Length / 4 * nbytes];

		var i = 0;
		var n = 0;
		var stride = surface.Stride;
		var ncols = surface.Width;

		if (BitConverter.IsLittleEndian)
		{
			var row = surface.Height;

			while (row-- > 0)
			{
				var prevPos = n;
				var col = ncols;

				while (col-- > 0)
				{
					var alphaFactor = nbytes == 4 ? 255d / surfaceData[n + 3] : 1;
					pixData[i] = (byte)(surfaceData[n + 2] * alphaFactor + 0.5);
					pixData[i + 1] = (byte)(surfaceData[n + 1] * alphaFactor + 0.5);
					pixData[i + 2] = (byte)(surfaceData[n + 0] * alphaFactor + 0.5);

					if (nbytes == 4)
						pixData[i + 3] = surfaceData[n + 3];

					n += 4;
					i += nbytes;
				}

				n = prevPos + stride;
			}
		}
		else
		{
			var row = surface.Height;

			while (row-- > 0)
			{
				var prevPos = n;
				var col = ncols;

				while (col-- > 0)
				{
					var alphaFactor = nbytes == 4 ? 255d / surfaceData[n + 3] : 1;
					pixData[i] = (byte)(surfaceData[n + 1] * alphaFactor + 0.5);
					pixData[i + 1] = (byte)(surfaceData[n + 2] * alphaFactor + 0.5);
					pixData[i + 2] = (byte)(surfaceData[n + 3] * alphaFactor + 0.5);

					if (nbytes == 4)
						pixData[i + 3] = surfaceData[n + 0];

					n += 4;
					i += nbytes;
				}

				n = prevPos + stride;
			}
		}

		return Pixbuf.NewFromBytes(Bytes.New(pixData),
			Colorspace.Rgb,
			nbytes == 4,
			8, surface.Width, surface.Height, surface.Width * nbytes);
	}

	public static string? ToImageExtension(this ImageFormat imageFormat) =>
		imageFormat switch
		{
			ImageFormat.Bmp => "bmp",
			ImageFormat.Png => "png",
			ImageFormat.Jpeg => "jpeg",
			ImageFormat.Gif => "gif",
			ImageFormat.Tiff => "tiff",
			_ => default
		};

	public static void SaveToStream(this Pixbuf? pixbuf, Stream stream, ImageFormat imageFormat = ImageFormat.Png)
	{
		if (pixbuf == null)
			return;

		using var outputStream = MemoryOutputStream.NewResizable();
		var success = pixbuf.SaveToStreamv(outputStream, imageFormat.ToImageExtension(), default, default, default);

		if (!success)
			throw new Exception("Failed to save pixbuf to stream");

		var bytes = outputStream.StealAsBytes();
		stream.Write(bytes.GetRegionSpan<byte>(0, bytes.GetSize()));
	}

	public static Pixbuf? LoadFromStream(Stream stream)
	{
		var loader = PixbufLoader.New();
		loader.LoadFromStream(stream);
		return loader.GetPixbuf();
	}

	private static void LoadFromStream(this PixbufLoader loader, Stream input)
	{
		const int bufferSize = 8192;
		var buffer = new byte[bufferSize];
		int bytesRead;

		while ((bytesRead = input.Read(buffer, 0, bufferSize)) != 0)
		{
			loader.Write(buffer.AsSpan(0, bytesRead));
		}
	}
}