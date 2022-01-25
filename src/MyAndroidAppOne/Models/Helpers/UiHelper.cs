using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Autofac;
using FFImageLoading;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

public class UiHelper
{
	static readonly IExceptionService exceptionService;

	static UiHelper()
	{
		exceptionService = GlobalObjects.MainActivity.Resolve<IExceptionService>();
	}

	/// <summary>
	/// global behaviors for handling errors. Ex: popup.
	/// todo: popup message
	/// </summary>
	public static void HandleUiException(Exception x, Context source)
	{
		var errorMessage = exceptionService.GetErrorMessage(x);
		UiHelper.Popup(ConstantValues.TITLE_ERROR_MESSAGE, errorMessage, source);
	}

	public static void PopupApiTimeoutError()
	{
		Popup(ConstantValues.TITLE_ERROR_MESSAGE, ConstantValues.MESSAGE_CANNOT_CONNECT_API, GlobalObjects.MainActivity);
	}
	public static void Popup(string title, string message, Context source)
	{
		var alertDialog = new AlertDialog.Builder(source);
		alertDialog.SetTitle(title);
		alertDialog.SetMessage(message);
		alertDialog.SetNeutralButton(ConstantValues.LABEL_OK, delegate
		{
			alertDialog.Dispose();
		});
		alertDialog.Show();
	}

	public static Bitmap GetImageBitmapFromUrl(string url)
	{
		try
		{
			var req = WebRequest.Create(url);
			req.Timeout = ConstantValues.IMAGE_TIMEOUT;
			var response = req.GetResponse();
			var stream = response.GetResponseStream();
			var value = BitmapFactory.DecodeStream(stream);
			return value;
		}
		catch (Exception x)
		{
			return null;
		}
	}

	public static void ToastMessage(string message, Context control)
	{
		Toast.MakeText(control, message, ToastLength.Long).Show();
	}
}