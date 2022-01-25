using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

public class CartService : BaseApiService, ICartService
{
	private readonly IFileService _fileService;
	private readonly IProductService _productService;

	public CartService(IHttpService httpService, IProductService productService, IFileService fileService) : base(httpService)
	{
		_fileService = fileService;
		_productService = productService;
	}

	public bool AddCartItemToCacheFile(CartItem inserted)
	{
		var cart = this.GetCartFromCacheFile();
		this.AddCartItem(inserted, cart.CartList);
		cart.TotalPrice += inserted.TotalPrice;
		return this.SaveCartToCacheFile(cart);
	}

	/// <summary>
	/// update if exist.
	/// Or add if not exist.
	/// </summary>
	protected void AddCartItem(CartItem inserted, CartItem[] updated)
	{
		//convert to dictionary; product id, item
		var dic = new SortedList<long, CartItem>();
		foreach (var item in updated)
		{
			dic[item.ProductId] = item;
		}
		var insertedProductId = inserted.ProductId;
		var insertedQuantity = inserted.Quantity;
		if (dic.ContainsKey(insertedProductId))
		{
			dic[insertedProductId].Quantity += insertedQuantity;
		}
		else
		{
			dic[insertedProductId] = inserted;
		}
		updated = dic.Values.ToArray();
	}

	/// <summary>
	/// refresh price for 1 product
	/// </summary>
	public void CalculateTotalPrice(CartItem inserted)
	{
		var productId = inserted.ProductId;
		ProductItem product;
		try
		{
			product = this._productService.GetProductByIdFromApi(productId);
			this._productService.UpdateProductsFile(product);
		}
		catch (TimeoutException)
		{
			product = this._productService.GetProductByIdFromCacheFile(productId);
		}
		if (product != null)
		{
			inserted.TotalPrice = inserted.Quantity * product.Price;
		}
	}

	/// <summary>
	/// get current cart.
	/// Assuming a mobile app can have only 1 cart.
	/// </summary>
	public Cart GetCartFromCacheFile()
	{
		var value = this._fileService.DeserializeCacheFileContent<Cart>(ConstantValues.CART_FILE_NAME);
		value = value ?? new Cart();
		return value;
	}

	public CartItem[] GetCartItemsFromApi(string userName)
	{
		try
		{
			string url = string.Format(DomainStaticMembers.UrlGetCartItemsByUserName, userName);
			return this._httpService.Download<CartItem[]>(url, ConstantValues.PRODUCTS_API_TIMEOUT);
		}
		catch (Exception)
		{
			throw new TimeoutException();
		}
	}

	public double GetCartTotalPriceFromApi(string userName)
	{
		try
		{
			this.TestConnectivity();
			string url = string.Format(DomainStaticMembers.UrlGetCartTotalPriceByUserName, userName);
			return this._httpService.Download<double>(url, ConstantValues.PRODUCTS_API_TIMEOUT);
		}
		catch (Exception)
		{
			throw new TimeoutException();
		}
	}

	/// <summary>
	/// add cart item to server
	/// </summary>
	public bool AddCartItemViaApi(AddCartItemRequest inserted)
	{
		try
		{
			this.TestConnectivity();

			string jsonContent = JsonConvert.SerializeObject(inserted);
			string url = DomainStaticMembers.UrlPostAddCartItem;
			var value = this._httpService.HttpPostWithJson<bool>(url, jsonContent, ConstantValues.PRODUCTS_API_TIMEOUT);
			return value;
		}
		catch (Exception)
		{
			throw new TimeoutException();
		}
	}

	public bool SaveCartToCacheFile(Cart cart)
	{
		var content = JsonConvert.SerializeObject(cart);
		return this._fileService.WriteCacheFile(ConstantValues.CART_FILE_NAME, content);
	}

	public Cart GetCartFromApi(string userName)
	{
		var value = new Cart()
		{
			UserName = userName
		};

		try
		{
			this.TestConnectivity();
			value.CartList = this.GetCartItemsFromApi(userName);
			value.TotalPrice = this.GetCartTotalPriceFromApi(userName);
			return value;
		}
		catch (Exception)
		{
			throw new TimeoutException();
		}
	}

}