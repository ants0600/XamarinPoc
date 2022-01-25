using Android.Graphics;
using System.Net.Http;

public interface IHttpService : IService
{
	T Download<T>(string url, double timeOutSecond);
	string HttpGet(string url, double timeOutSecond);
	Bitmap GetBitmapFromUrl(string url);
	Bitmap GetBitmapFromUrlWebRequest(string url, int timeOutMiliSecond);
	HttpClient GetHttpClient(double timeOutSecond);
	bool Ping(string ip);
	string HttpPostWithJson(string url, string jsonContent, double timeOutSecond);
	T HttpPostWithJson<T>(string url, string jsonContent, double pRODUCTS_API_TIMEOUT);
}