using System;

public interface IExceptionService : IService
{
	string GetErrorMessage(Exception x);
}