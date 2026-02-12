using Microsoft.Maui.Storage;

namespace Maui.Linux.Essentials.Storage;

public class LinuxFilePicker : IFilePicker
{
	public async Task<FileResult?> PickAsync(PickOptions? options)
	{
		var results = await PickInternalAsync(false, options);
		return results?.FirstOrDefault();
	}

	public async Task<IEnumerable<FileResult>?> PickMultipleAsync(PickOptions? options)
	{
		return await PickInternalAsync(true, options);
	}

	private async Task<List<FileResult>?> PickInternalAsync(bool multiple, PickOptions? options)
	{
		try
		{
			var dialog = Gtk.FileDialog.New();
			dialog.SetTitle(options?.PickerTitle ?? "Select file");

			if (options?.FileTypes is not null)
			{
				var filterList = Gio.ListStore.New(Gtk.FileFilter.GetGType());
				var filter = Gtk.FileFilter.New();
				foreach (var ft in options.FileTypes.Value)
					filter.AddPattern($"*{ft}");
				filterList.Append(filter);
				dialog.SetFilters(filterList);
			}

			var window = (Gtk.Application.GetDefault() as Gtk.Application)?.GetActiveWindow();
			if (window is null) return null;

			if (multiple)
			{
				var files = await dialog.OpenMultipleAsync(window);
				var results = new List<FileResult>();
				for (uint i = 0; i < files.GetNItems(); i++)
				{
					var file = (Gio.File)files.GetObject(i)!;
					var path = file.GetPath();
					if (path is not null)
						results.Add(new FileResult(path));
				}
				return results;
			}
			else
			{
				var file = await dialog.OpenAsync(window);
				var path = file?.GetPath();
				return path is not null ? new List<FileResult> { new FileResult(path) } : null;
			}
		}
		catch { return null; }
	}
}
