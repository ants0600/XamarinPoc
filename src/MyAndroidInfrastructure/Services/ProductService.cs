using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using Xamarin.Essentials;

public class ProductService : BaseApiService, IProductService
{
	private readonly IFileService _fileService;

	public ProductService(IHttpService httpService, IFileService fileService) : base(httpService)
	{
		_fileService = fileService;
	}

	/// <summary>
	/// invoke API, then deserialize from JSON.
	/// Then set image file name.
	/// Throw timeout exception when system cant reach API.
	/// </summary>
	public List<ProductItem> GetProductsFromApi()
	{
		try
		{
			var values = base._httpService.Download<List<ProductItem>>(DomainStaticMembers.UrlGetProducts, ConstantValues.PRODUCTS_API_TIMEOUT);
			values = values ?? new List<ProductItem>();
			foreach (var item in values)
			{
				item.MobileCardImageFileName = this.GetImageFileName(item);
			}
			return values;
		}
		catch (Exception)
		{
			throw new TimeoutException();
		}
	}

	public List<ProductItem> GetProductsFromFile()
	{
		var fileContent = this._fileService.ReadCacheFile(ConstantValues.PRODUCTS_FILE_NAME);
		return JsonConvert.DeserializeObject<List<ProductItem>>(fileContent, HttpService.JsonSettings);
	}

	public bool UpdateProductsFile(List<ProductItem> values)
	{
		values = values ?? new List<ProductItem>();
		var json = JsonConvert.SerializeObject(values);
		return this._fileService.WriteCacheFile(ConstantValues.PRODUCTS_FILE_NAME, json);
	}

	public string GetImageFileName(ProductItem productItem)
	{
		return $"{productItem.Code}{ConstantValues.PNG_EXTENSION}";
	}

	public ProductItem GetProductByIdFromApi(long id)
	{
		try
		{
			this.TestConnectivity();
			var url = string.Format(DomainStaticMembers.UrlGetProductById, id);
			return this._httpService.Download<ProductItem>(url, ConstantValues.PRODUCTS_API_TIMEOUT);
		}
		catch (Exception)
		{
			throw new TimeoutException();
		}
	}

	public ProductItem GetProductByIdFromCacheFile(long productId)
	{
		var products = this.GetProductsFromFile();
		return products.FirstOrDefault(a => a.Id == productId);
	}

	public void UpdateProductsFile(ProductItem updated)
	{
		var products = this.GetProductsFromFile();
		var found = products.FirstOrDefault(a => a.Id == updated.Id);
		if (found == null)
		{
			products.Add(updated);
		}
		else
		{
			found = updated;
		}
		this.UpdateProductsFile(products);
	}
}
