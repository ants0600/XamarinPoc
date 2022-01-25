using Android.Graphics;
using Newtonsoft.Json;
using System.IO;
using Xamarin.Essentials;

public class FileService : IFileService
{
	public string ReadCacheFile(string fileName)
	{
		fileName = this.GetCacheFileName(fileName);
		return File.Exists(fileName) ? File.ReadAllText(fileName) : string.Empty;
	}

	public byte[] ReadBytesFromCacheFile(string fileName)
	{
		fileName = this.GetCacheFileName(fileName);
		return File.Exists(fileName) ? File.ReadAllBytes(fileName) : new byte[0];
	}

	public string ReadFile(string fileName)
	{
		using (var stream = FileSystem.OpenAppPackageFileAsync(fileName).Result)
		{
			using (var reader = new StreamReader(stream))
			{
				return reader.ReadToEnd();
			}
		}
	}

	public byte[] ReadBytesFromStream(Stream source)
	{
		using (var ms = new MemoryStream())
		{
			source.CopyTo(ms);
			return ms.ToArray();
		}
	}

	/// <summary>
	/// write to cache
	/// </summary>
	public bool WriteCacheFile(string fileName, string content)
	{
		fileName = this.GetCacheFileName(fileName);
		File.WriteAllText(fileName, content);
		return true;
	}

	/// <summary>
	/// get relative path of cache file name
	/// </summary>
	public string GetCacheFileName(string fileName)
	{
		var value = System.IO.Path.Combine(FileSystem.CacheDirectory, fileName);
		return value;
	}

	public Bitmap GetBitmapFromBytes(byte[] imageData)
	{
		var bmpOutput = BitmapFactory.DecodeByteArray(imageData, 0, imageData.Length);
		return bmpOutput;
	}

	public Bitmap GetBitmapFromCacheFile(string fileName)
	{
		fileName = this.GetCacheFileName(fileName);
		var value = BitmapFactory.DecodeFile(fileName);
		return value;
	}

	public void ExportBitmapToCacheFile(Bitmap bitmap, string fileName)
	{
		fileName = this.GetCacheFileName(fileName);
		using (var stream = new FileStream(fileName, FileMode.Create))
		{
			bitmap.Compress(Bitmap.CompressFormat.Png, 100, stream);
		}
	}

	public T DeserializeCacheFileContent<T>(string fileName)
	{
		var fileContent = this.ReadCacheFile(fileName);
		if (string.IsNullOrEmpty(fileContent))
		{
			return default;
		}

		T value = JsonConvert.DeserializeObject<T>(fileContent, HttpService.JsonSettings);
		return value;
	}

	public string ReadTextFromStream(Stream source)
	{
		using (var reader = new StreamReader(source))
		{
			string value = reader.ReadToEnd();
			return value;
		}
	}
}
