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
			IFileImageSource fileSource => await LoadFromFileAsync(fileSource, cancellationToken),
			IUriImageSource uriSource => await LoadFromUriAsync(uriSource, cancellationToken),
			IStreamImageSource streamSource => await LoadFromStreamAsync(streamSource, cancellationToken),
			_ => null,
		};
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
