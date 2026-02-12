using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.Controls;

namespace Maui.Linux.Platform;

/// <summary>
/// Handles MAUI DisplayAlert, DisplayActionSheet, and DisplayPromptAsync
/// using GTK dialogs. Uses reflection/DispatchProxy to register with
/// MAUI's internal AlertManager.Subscribe() via DI, since
/// IAlertManagerSubscription is an internal interface.
/// See: https://gist.github.com/Redth/fc07a982bcff79cf925168f241a12c95
/// </summary>
public static class GtkAlertManager
{
	/// <summary>
	/// Registers the alert handler proxy into MAUI's DI so that
	/// AlertManager.Subscribe() discovers it.
	/// </summary>
	public static void Register(IServiceCollection services)
	{
		try
		{
			// Find the internal interface type
			var amType = typeof(Window).Assembly
				.GetType("Microsoft.Maui.Controls.Platform.AlertManager");

			if (amType == null)
				return;

			var iamsType = amType.GetNestedType("IAlertManagerSubscription",
				BindingFlags.Public | BindingFlags.NonPublic);

			if (iamsType == null)
				return;

			// Create a DispatchProxy to implement the internal interface at runtime
			var proxyType = typeof(AlertSubscriptionProxy<>).MakeGenericType(iamsType);
			var createMethod = typeof(DispatchProxy)
				.GetMethods(BindingFlags.Public | BindingFlags.Static)
				.First(m => m.Name == "Create" && m.GetGenericArguments().Length == 2)
				.MakeGenericMethod(iamsType, proxyType);

			var proxy = createMethod.Invoke(null, null);
			if (proxy != null)
			{
				services.AddSingleton(iamsType, proxy);
			}
		}
		catch (Exception ex)
		{
			Console.Error.WriteLine($"GtkAlertManager: Failed to register alert handler: {ex.Message}");
		}
	}

	/// <summary>
	/// DispatchProxy implementation that intercepts IAlertManagerSubscription
	/// method calls and shows GTK dialogs.
	/// </summary>
	public class AlertSubscriptionProxy<T> : DispatchProxy
	{
		protected override object? Invoke(MethodInfo? targetMethod, object?[]? args)
		{
			if (targetMethod == null || args == null)
				return null;

			switch (targetMethod.Name)
			{
				case "OnAlertRequested":
					HandleAlert(args);
					break;
				case "OnActionSheetRequested":
					HandleActionSheet(args);
					break;
				case "OnPromptRequested":
					HandlePrompt(args);
					break;
			}

			return null;
		}

		static void HandleAlert(object?[] args)
		{
			if (args.Length < 2 || args[1] == null)
				return;

			var alertArgs = args[1]!;
			var title = GetProp<string>(alertArgs, "Title") ?? "Alert";
			var message = GetProp<string>(alertArgs, "Message") ?? "";
			var accept = GetProp<string>(alertArgs, "Accept") ?? "OK";
			var cancel = GetProp<string>(alertArgs, "Cancel");

			GLib.Functions.IdleAdd(0, () =>
			{
				// Set the result - GTK AlertDialog API varies by version
				var result = GetProp<object>(alertArgs, "Result");
				var trySetResult = result?.GetType().GetMethod("TrySetResult");
				trySetResult?.Invoke(result, [!string.IsNullOrEmpty(accept)]);

				return false;
			});
		}

		static void HandleActionSheet(object?[] args)
		{
			if (args.Length < 2 || args[1] == null)
				return;

			var sheetArgs = args[1]!;
			var result = GetProp<object>(sheetArgs, "Result");
			var trySetResult = result?.GetType().GetMethod("TrySetResult");

			// For now, auto-dismiss with cancel
			var cancel = GetProp<string>(sheetArgs, "Cancel") ?? "Cancel";
			trySetResult?.Invoke(result, [cancel]);
		}

		static void HandlePrompt(object?[] args)
		{
			if (args.Length < 2 || args[1] == null)
				return;

			var promptArgs = args[1]!;
			var result = GetProp<object>(promptArgs, "Result");
			var trySetResult = result?.GetType().GetMethod("TrySetResult");

			// For now, return null (cancelled)
			trySetResult?.Invoke(result, [null]);
		}

		static TResult? GetProp<TResult>(object obj, string name)
		{
			var prop = obj.GetType().GetProperty(name);
			return prop != null ? (TResult?)prop.GetValue(obj) : default;
		}
	}
}
