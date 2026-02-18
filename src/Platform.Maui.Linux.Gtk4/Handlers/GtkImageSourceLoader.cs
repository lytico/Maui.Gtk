using System.Net.Http;
using Microsoft.Maui;

namespace Platform.Maui.Linux.Gtk4.Handlers;

internal static class GtkImageSourceLoader
{
	static readonly HttpClient HttpClient = new();

	public static async Task<Gdk.Texture?> LoadTextureAsync(IImageSource? source, CancellationToken cancellationToken)
	{
		if (source == null)
			return null;

		return source switch
		{
			IFontImageSource fontSource => LoadFromFontImageSource(fontSource),
			IFileImageSource fileSource => await LoadFromFileAsync(fileSource, cancellationToken),
			IUriImageSource uriSource => await LoadFromUriAsync(uriSource, cancellationToken),
			IStreamImageSource streamSource => await LoadFromStreamAsync(streamSource, cancellationToken),
			_ => null,
		};
	}

	static Gdk.Texture? LoadFromFontImageSource(IFontImageSource fontSource)
	{
		var glyph = fontSource.Glyph;
		if (string.IsNullOrEmpty(glyph))
			return null;

		var size = fontSource.Font.Size > 0 ? fontSource.Font.Size : 24;
		var pixelSize = (int)(size * 1.4); // account for glyph metrics
		int surfaceSize = Math.Max(pixelSize + 4, 16); // padding for descenders

		// Create Cairo surface and render the glyph via Pango
		var surface = new Cairo.ImageSurface(Cairo.Format.Argb32, surfaceSize, surfaceSize);
		var cr = new Cairo.Context(surface);

		// Set the glyph color
		var color = fontSource.Color;
		if (color != null)
			Cairo.Internal.Context.SetSourceRgba(cr.Handle, color.Red, color.Green, color.Blue, color.Alpha);
		else
			Cairo.Internal.Context.SetSourceRgba(cr.Handle, 0, 0, 0, 1);

		// Create Pango layout with the font
		var layout = PangoCairo.Functions.CreateLayout(cr);
		var fontDesc = Pango.FontDescription.New();

		var fontFamily = fontSource.Font.Family;
		if (!string.IsNullOrEmpty(fontFamily))
			fontDesc.SetFamily(fontFamily);

		fontDesc.SetAbsoluteSize(size * Pango.Constants.SCALE);
		layout.SetFontDescription(fontDesc);
		layout.SetText(glyph, -1);

		// Center the glyph in the surface
		layout.GetPixelSize(out int textW, out int textH);
		double offsetX = (surfaceSize - textW) / 2.0;
		double offsetY = (surfaceSize - textH) / 2.0;
		Cairo.Internal.Context.MoveTo(cr.Handle, offsetX, offsetY);

		PangoCairo.Functions.ShowLayout(cr, layout);

		// Flush surface and read pixel data
		Cairo.Internal.Surface.Flush(surface.Handle);

		var data = surface.GetData();
		if (data.IsEmpty)
			return null;

		// Cairo ARGB32 = Gdk A8R8G8B8 premultiplied (native byte order)
		var glibBytes = GLib.Bytes.New(data);
		int stride = surface.Stride;

		var builder = Gdk.MemoryTextureBuilder.New();
		builder.SetWidth(surfaceSize);
		builder.SetHeight(surfaceSize);
		builder.SetFormat(Gdk.MemoryFormat.A8r8g8b8Premultiplied);
		builder.SetBytes(glibBytes);
		builder.SetStride((uint)stride);

		return builder.Build();
	}

	static Task<Gdk.Texture?> LoadFromFileAsync(IFileImageSource fileSource, CancellationToken cancellationToken)
	{
		cancellationToken.ThrowIfCancellationRequested();
		if (string.IsNullOrWhiteSpace(fileSource.File))
			return Task.FromResult<Gdk.Texture?>(null);

		var filePath = ResolveFilePath(fileSource.File);
		if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath))
			return Task.FromResult<Gdk.Texture?>(null);

		return Task.FromResult<Gdk.Texture?>(Gdk.Texture.NewFromFilename(filePath));
	}

	static async Task<Gdk.Texture?> LoadFromUriAsync(IUriImageSource uriSource, CancellationToken cancellationToken)
	{
		var uri = uriSource.Uri;
		if (uri == null)
			return null;

		if (uri.IsFile && !string.IsNullOrWhiteSpace(uri.LocalPath) && File.Exists(uri.LocalPath))
		{
			return Gdk.Texture.NewFromFilename(uri.LocalPath);
		}

		var bytes = await HttpClient.GetByteArrayAsync(uri, cancellationToken);
		if (bytes.Length == 0)
			return null;

		var glibBytes = GLib.Bytes.New(bytes.AsSpan());
		return Gdk.Texture.NewFromBytes(glibBytes);
	}

	static async Task<Gdk.Texture?> LoadFromStreamAsync(IStreamImageSource streamSource, CancellationToken cancellationToken)
	{
		await using var stream = await streamSource.GetStreamAsync(cancellationToken);
		if (stream == null)
			return null;

		await using var ms = new MemoryStream();
		await stream.CopyToAsync(ms, cancellationToken);
		var bytes = ms.ToArray();
		if (bytes.Length == 0)
			return null;

		var glibBytes = GLib.Bytes.New(bytes.AsSpan());
		return Gdk.Texture.NewFromBytes(glibBytes);
	}

	static string ResolveFilePath(string source)
	{
		if (Path.IsPathRooted(source))
			return source;

		var appBasePath = Path.Combine(AppContext.BaseDirectory, source);
		if (File.Exists(appBasePath))
			return appBasePath;

		var cwdPath = Path.GetFullPath(source);
		if (File.Exists(cwdPath))
			return cwdPath;

		return source;
	}
}
