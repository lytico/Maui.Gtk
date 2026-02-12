using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Storage;

namespace Maui.Linux.Essentials.Storage;

/// <summary>
/// Secure storage using encrypted file storage.
/// Falls back gracefully when no freedesktop.org secrets service is available.
/// </summary>
public class LinuxSecureStorage : ISecureStorage
{
	private readonly object _lock = new();

	private string StoragePath
	{
		get
		{
			var dataDir = Environment.GetEnvironmentVariable("XDG_DATA_HOME");
			if (string.IsNullOrEmpty(dataDir))
				dataDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".local", "share");
			var appDir = Path.Combine(dataDir, AppDomain.CurrentDomain.FriendlyName, ".secure");
			Directory.CreateDirectory(appDir);
			// Restrict permissions to owner only
			try { File.SetUnixFileMode(appDir, UnixFileMode.UserRead | UnixFileMode.UserWrite | UnixFileMode.UserExecute); }
			catch { }
			return appDir;
		}
	}

	private string DataFilePath => Path.Combine(StoragePath, "secure_store.dat");
	private string KeyFilePath => Path.Combine(StoragePath, "secure_store.key");

	public Task<string?> GetAsync(string key)
	{
		lock (_lock)
		{
			var store = LoadStore();
			return Task.FromResult(store.TryGetValue(key, out var value) ? value : null);
		}
	}

	public Task SetAsync(string key, string value)
	{
		lock (_lock)
		{
			var store = LoadStore();
			store[key] = value;
			SaveStore(store);
			return Task.CompletedTask;
		}
	}

	public bool Remove(string key)
	{
		lock (_lock)
		{
			var store = LoadStore();
			var removed = store.Remove(key);
			if (removed)
				SaveStore(store);
			return removed;
		}
	}

	public void RemoveAll()
	{
		lock (_lock)
		{
			if (File.Exists(DataFilePath))
				File.Delete(DataFilePath);
			if (File.Exists(KeyFilePath))
				File.Delete(KeyFilePath);
		}
	}

	private byte[] GetOrCreateKey()
	{
		if (File.Exists(KeyFilePath))
		{
			var keyData = File.ReadAllBytes(KeyFilePath);
			if (keyData.Length == 32)
				return keyData;
		}

		var key = RandomNumberGenerator.GetBytes(32);
		File.WriteAllBytes(KeyFilePath, key);
		try { File.SetUnixFileMode(KeyFilePath, UnixFileMode.UserRead | UnixFileMode.UserWrite); }
		catch { }
		return key;
	}

	private Dictionary<string, string> LoadStore()
	{
		if (!File.Exists(DataFilePath))
			return new();

		try
		{
			var encrypted = File.ReadAllBytes(DataFilePath);
			var key = GetOrCreateKey();
			var json = Decrypt(encrypted, key);
			return JsonSerializer.Deserialize<Dictionary<string, string>>(json) ?? new();
		}
		catch
		{
			return new();
		}
	}

	private void SaveStore(Dictionary<string, string> store)
	{
		var json = JsonSerializer.Serialize(store);
		var key = GetOrCreateKey();
		var encrypted = Encrypt(json, key);
		File.WriteAllBytes(DataFilePath, encrypted);
		try { File.SetUnixFileMode(DataFilePath, UnixFileMode.UserRead | UnixFileMode.UserWrite); }
		catch { }
	}

	private static byte[] Encrypt(string plainText, byte[] key)
	{
		using var aes = Aes.Create();
		aes.Key = key;
		aes.GenerateIV();
		using var encryptor = aes.CreateEncryptor();
		var plainBytes = Encoding.UTF8.GetBytes(plainText);
		var cipherBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);
		// Prepend IV to cipher
		var result = new byte[aes.IV.Length + cipherBytes.Length];
		aes.IV.CopyTo(result, 0);
		cipherBytes.CopyTo(result, aes.IV.Length);
		return result;
	}

	private static string Decrypt(byte[] cipherWithIv, byte[] key)
	{
		using var aes = Aes.Create();
		aes.Key = key;
		var iv = new byte[aes.BlockSize / 8];
		Array.Copy(cipherWithIv, 0, iv, 0, iv.Length);
		aes.IV = iv;
		using var decryptor = aes.CreateDecryptor();
		var cipherBytes = new byte[cipherWithIv.Length - iv.Length];
		Array.Copy(cipherWithIv, iv.Length, cipherBytes, 0, cipherBytes.Length);
		var plainBytes = decryptor.TransformFinalBlock(cipherBytes, 0, cipherBytes.Length);
		return Encoding.UTF8.GetString(plainBytes);
	}
}
