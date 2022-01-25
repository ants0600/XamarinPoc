using Android.Content;
using System;

public class ExceptionService : IExceptionService
{
	public string GetErrorMessage(Exception x)
	{
		return string.Format(ConstantValues.ERROR_MESSAGE_FORMAT, x, x.Message, x.StackTrace);
	}
}
