using Android.Graphics;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Text;

public class HttpService : IHttpService
{
	public static readonly JsonSerializerSettings JsonSettings = new JsonSerializerSettings()
	{
		MaxDepth = int.MaxValue
	};

	public HttpClient GetHttpClient(double timeOutSecond)
	{
		var value = new HttpClient();
		value.Timeout = TimeSpan.FromSeconds(timeOutSecond);
		return value;
	}

	public T Download<T>(string url, double timeOutSecond)
	{
		var s = this.HttpGet(url, timeOutSecond);
		return JsonConvert.DeserializeObject<T>(s, HttpService.JsonSettings);
	}

	public string HttpGet(string url, double timeOutSecond)
	{
		using (var client = this.GetHttpClient(timeOutSecond))
		{
			return client.GetAsync(url).Result.Content.ReadAsStringAsync().Result;
		}
	}

	public bool Ping(string ip)
	{
		try
		{
			Ping p = new Ping();
			PingReply rep = p.SendPingAsync(ip).Result;
			return rep.Status == IPStatus.Success;
		}
		catch (Exception ex)
		{
			throw new TimeoutException(ex.Message);
		}
	}

	public Bitmap GetBitmapFromUrl(string url)
	{
		using (var webClient = new WebClient())
		{
			var downloadData = webClient.DownloadData(url);
			if (downloadData == null)
			{
				return null;
			}

			if (downloadData.Length <= 0)
			{
				return null;
			}

			return BitmapFactory.DecodeByteArray(downloadData, 0, downloadData.Length);
		}
	}

	public Bitmap GetBitmapFromUrlWebRequest(string url, int timeOutMiliSecond)
	{
		WebRequest req = WebRequest.Create(url);
		req.Timeout = timeOutMiliSecond;
		WebResponse response = req.GetResponse();
		System.IO.Stream stream = response.GetResponseStream();
		var value = BitmapFactory.DecodeStream(stream);
		return value;
	}

	public string HttpPostWithJson(string url, string jsonContent, double timeOutSecond)
	{
		using (var client = this.GetHttpClient(timeOutSecond))
		{
			var content = new StringContent(jsonContent, Encoding.UTF8, ConstantValues.JSON_CONTENT_TYPE);
			var httpResponseMessage = client.
				PostAsync(url, content).
				Result;
			return httpResponseMessage.Content.ReadAsStringAsync().Result;
		}
	}

	public T HttpPostWithJson<T>(string url, string jsonContent, double timeOutInSecond)
	{
		var s = this.HttpPostWithJson(url, jsonContent, timeOutInSecond);
		return JsonConvert.DeserializeObject<T>(s);
	}
}
