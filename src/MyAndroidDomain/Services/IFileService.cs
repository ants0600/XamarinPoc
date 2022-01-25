using Android.Graphics;
using System.IO;

public interface IFileService : IService
{
	void ExportBitmapToCacheFile(Bitmap bitmap, string fileName);
	Bitmap GetBitmapFromBytes(byte[] imageData);
	Bitmap GetBitmapFromCacheFile(string fileName);
	string GetCacheFileName(string fileName);
	byte[] ReadBytesFromCacheFile(string fileName);
	string ReadCacheFile(string fileName);
	string ReadFile(string fileName);
	byte[] ReadBytesFromStream(Stream source);
	bool WriteCacheFile(string fileName, string content);
	T DeserializeCacheFileContent<T>(string fileName);
	string ReadTextFromStream(Stream stream);
}