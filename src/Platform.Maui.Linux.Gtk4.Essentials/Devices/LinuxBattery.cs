using Microsoft.Maui.Devices;

namespace Platform.Maui.Linux.Gtk4.Essentials.Devices;

public class LinuxBattery : IBattery
{
	private EventHandler<BatteryInfoChangedEventArgs>? _batteryInfoChanged;
	private EventHandler<EnergySaverStatusChangedEventArgs>? _energySaverStatusChanged;

	public double ChargeLevel => ReadChargeLevel();
	public BatteryState State => ReadBatteryState();
	public BatteryPowerSource PowerSource => ReadPowerSource();
	public EnergySaverStatus EnergySaverStatus => EnergySaverStatus.Off;

	public event EventHandler<BatteryInfoChangedEventArgs>? BatteryInfoChanged
	{
		add => _batteryInfoChanged += value;
		remove => _batteryInfoChanged -= value;
	}

	public event EventHandler<EnergySaverStatusChangedEventArgs>? EnergySaverStatusChanged
	{
		add => _energySaverStatusChanged += value;
		remove => _energySaverStatusChanged -= value;
	}

	private static double ReadChargeLevel()
	{
		try
		{
			var path = FindBatteryPath("capacity");
			if (path is null) return 1.0;
			var value = File.ReadAllText(path).Trim();
			return int.TryParse(value, out var pct) ? pct / 100.0 : 1.0;
		}
		catch { return 1.0; }
	}

	private static BatteryState ReadBatteryState()
	{
		try
		{
			var path = FindBatteryPath("status");
			if (path is null) return BatteryState.NotPresent;
			var status = File.ReadAllText(path).Trim().ToLowerInvariant();
			return status switch
			{
				"charging" => BatteryState.Charging,
				"discharging" => BatteryState.Discharging,
				"full" => BatteryState.Full,
				"not charging" => BatteryState.NotCharging,
				_ => BatteryState.Unknown
			};
		}
		catch { return BatteryState.Unknown; }
	}

	private static BatteryPowerSource ReadPowerSource()
	{
		try
		{
			var state = ReadBatteryState();
			return state switch
			{
				BatteryState.Charging or BatteryState.Full => BatteryPowerSource.AC,
				BatteryState.Discharging => BatteryPowerSource.Battery,
				BatteryState.NotPresent => BatteryPowerSource.AC,
				_ => BatteryPowerSource.Unknown
			};
		}
		catch { return BatteryPowerSource.Unknown; }
	}

	private static string? FindBatteryPath(string file)
	{
		var baseDir = "/sys/class/power_supply";
		if (!Directory.Exists(baseDir)) return null;
		var batteryPaths = new List<string>();
		foreach (var dir in Directory.GetDirectories(baseDir))
		{
			var typePath = Path.Combine(dir, "type");
			if (File.Exists(typePath) && File.ReadAllText(typePath).Trim().Equals("Battery", StringComparison.OrdinalIgnoreCase))
				batteryPaths.Add(dir);
		}
		if (batteryPaths.Count == 0) return null;
		string matchingPath = batteryPaths[0];	// as a default, but maybe we can do better...
		if (batteryPaths.Count > 1)				// if there are multiple batteries, try to find best match
		{										// for example exclude batteries of wireless mouse or keyboard
			var matchingPaths = batteryPaths.Where(x => x.EndsWith("BAT0", StringComparison.OrdinalIgnoreCase));
			if (matchingPaths.Count() == 1)
				matchingPath = matchingPaths.First();	// ThinkPads or systems with multiple batteries BAT0 + BAT1
			else										// assume BAT0 as primary
			{
				matchingPaths = batteryPaths.Where(x => x.EndsWith("BAT", StringComparison.OrdinalIgnoreCase));
				if (matchingPaths.Count() == 1)
					matchingPath = matchingPaths.First();  // on ASUS and some ThinkPads the primary battery is BAT1
				else
				{
					matchingPaths = batteryPaths.Where(x => x.EndsWith("BA", StringComparison.OrdinalIgnoreCase));
					if (matchingPaths.Count() == 1)
						matchingPath = matchingPaths.First();  // BAPM or some other naming scheme
					else
						matchingPath = batteryPaths[0];  // give up, use the first one found

					// TODO: If the BA(T(0)) pattern doesn't yield a single match, we could try a secondary heuristic
					//       and read the "model_name", "technology" or "manufacturer" files in each battery path and
					//       use that to determine which one is the main battery. Usually the main battery will have
					//       more such information available than wireless devices.
				}
			}
		}

		var filePath = Path.Combine(matchingPath, file);
		if (File.Exists(filePath))
			return filePath;

		return null;
	}
}
