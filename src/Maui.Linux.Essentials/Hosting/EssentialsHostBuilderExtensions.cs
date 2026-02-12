using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Maui.Hosting;
using Maui.Linux.Essentials.AppModel;
using Maui.Linux.Essentials.Communication;
using Maui.Linux.Essentials.DataTransfer;
using Maui.Linux.Essentials.Devices;
using Maui.Linux.Essentials.Sensors;
using Maui.Linux.Essentials.Media;
using Maui.Linux.Essentials.Networking;
using Maui.Linux.Essentials.Storage;
using Maui.Linux.Essentials.Accessibility;
using Maui.Linux.Essentials.Authentication;

namespace Maui.Linux.Essentials.Hosting;

public static class EssentialsHostBuilderExtensions
{
	public static MauiAppBuilder AddLinuxEssentials(this MauiAppBuilder builder)
	{
		// Tier 1 — Pure .NET
		builder.Services.TryAddSingleton<Microsoft.Maui.ApplicationModel.IAppInfo, LinuxAppInfo>();
		builder.Services.TryAddSingleton<Microsoft.Maui.Devices.IDeviceInfo, LinuxDeviceInfo>();
		builder.Services.TryAddSingleton<Microsoft.Maui.Storage.IFileSystem, LinuxFileSystem>();
		builder.Services.TryAddSingleton<Microsoft.Maui.Storage.IPreferences, LinuxPreferences>();
		builder.Services.TryAddSingleton<Microsoft.Maui.ApplicationModel.IVersionTracking, LinuxVersionTracking>();
		builder.Services.TryAddSingleton<Microsoft.Maui.ApplicationModel.ILauncher, LinuxLauncher>();
		builder.Services.TryAddSingleton<Microsoft.Maui.ApplicationModel.IBrowser, LinuxBrowser>();
		builder.Services.TryAddSingleton<Microsoft.Maui.ApplicationModel.IMap, LinuxMap>();
		builder.Services.TryAddSingleton<Microsoft.Maui.ApplicationModel.Communication.IEmail, LinuxEmail>();

		// Tier 2 — GTK4
		builder.Services.TryAddSingleton<Microsoft.Maui.ApplicationModel.DataTransfer.IClipboard, LinuxClipboard>();
		builder.Services.TryAddSingleton<Microsoft.Maui.Devices.IDeviceDisplay, LinuxDeviceDisplay>();
		builder.Services.TryAddSingleton<Microsoft.Maui.Storage.IFilePicker, LinuxFilePicker>();
		builder.Services.TryAddSingleton<Microsoft.Maui.Media.IMediaPicker, LinuxMediaPicker>();
		builder.Services.TryAddSingleton<Microsoft.Maui.Media.IScreenshot, LinuxScreenshot>();

		// Tier 3 — DBus
		builder.Services.TryAddSingleton<Microsoft.Maui.Devices.IBattery, LinuxBattery>();
		builder.Services.TryAddSingleton<Microsoft.Maui.Networking.IConnectivity, LinuxConnectivity>();
		builder.Services.TryAddSingleton<Microsoft.Maui.Storage.ISecureStorage, LinuxSecureStorage>();
		builder.Services.TryAddSingleton<Microsoft.Maui.Devices.Sensors.IGeolocation, LinuxGeolocation>();
		builder.Services.TryAddSingleton<Microsoft.Maui.Media.ITextToSpeech, LinuxTextToSpeech>();

		// Tier 4 — Best-effort
		builder.Services.TryAddSingleton<Microsoft.Maui.ApplicationModel.DataTransfer.IShare, LinuxShare>();
		builder.Services.TryAddSingleton<Microsoft.Maui.ApplicationModel.IAppActions, LinuxAppActions>();
		builder.Services.TryAddSingleton<Microsoft.Maui.Authentication.IWebAuthenticator, LinuxWebAuthenticator>();

		// Tier 5 — Stubs
		builder.Services.TryAddSingleton<Microsoft.Maui.Devices.IFlashlight, LinuxFlashlight>();
		builder.Services.TryAddSingleton<Microsoft.Maui.Devices.IHapticFeedback, LinuxHapticFeedback>();
		builder.Services.TryAddSingleton<Microsoft.Maui.Devices.IVibration, LinuxVibration>();
		builder.Services.TryAddSingleton<Microsoft.Maui.ApplicationModel.Communication.IPhoneDialer, LinuxPhoneDialer>();
		builder.Services.TryAddSingleton<Microsoft.Maui.ApplicationModel.Communication.ISms, LinuxSms>();
		builder.Services.TryAddSingleton<Microsoft.Maui.ApplicationModel.Communication.IContacts, LinuxContacts>();
		builder.Services.TryAddSingleton<Microsoft.Maui.Devices.Sensors.IAccelerometer, LinuxAccelerometer>();
		builder.Services.TryAddSingleton<Microsoft.Maui.Devices.Sensors.IBarometer, LinuxBarometer>();
		builder.Services.TryAddSingleton<Microsoft.Maui.Devices.Sensors.ICompass, LinuxCompass>();
		builder.Services.TryAddSingleton<Microsoft.Maui.Devices.Sensors.IGyroscope, LinuxGyroscope>();
		builder.Services.TryAddSingleton<Microsoft.Maui.Devices.Sensors.IMagnetometer, LinuxMagnetometer>();
		builder.Services.TryAddSingleton<Microsoft.Maui.Devices.Sensors.IOrientationSensor, LinuxOrientationSensor>();
		builder.Services.TryAddSingleton<Microsoft.Maui.Devices.Sensors.IGeocoding, LinuxGeocoding>();
		builder.Services.TryAddSingleton<Microsoft.Maui.Accessibility.ISemanticScreenReader, LinuxSemanticScreenReader>();

		return builder;
	}
}
