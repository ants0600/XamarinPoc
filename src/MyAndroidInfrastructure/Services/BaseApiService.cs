using System;
public abstract class BaseApiService : IBaseApiService
{
	protected readonly IHttpService _httpService;

	public BaseApiService(IHttpService httpService)
	{
		_httpService = httpService;
	}

	/// <summary>
	/// simple download string from API.
	/// will throw timeout exception if not reachable.
	/// </summary>
	public string TestConnectivity()
	{
		try
		{
			return this._httpService.HttpGet(DomainStaticMembers.ApiEndpoint, DomainStaticMembers.ApiConnectionTimeout);
		}
		catch (Exception x)
		{
			throw new TimeoutException(x.Message);
		}
	}
}
