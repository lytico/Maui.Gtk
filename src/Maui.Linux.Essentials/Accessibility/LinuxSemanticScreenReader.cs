using Microsoft.Maui.Accessibility;

namespace Maui.Linux.Essentials.Accessibility;

public class LinuxSemanticScreenReader : ISemanticScreenReader
{
	public void Announce(string text)
	{
		// Best-effort: try to use AT-SPI2 via speech-dispatcher
		try
		{
			System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo("spd-say", $"\"{text}\"")
				{ UseShellExecute = false, RedirectStandardOutput = true, RedirectStandardError = true });
		}
		catch { }
	}
}
