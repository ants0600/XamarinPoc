using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/// <summary>
/// todo: exception, use static member here
/// </summary>
public class DomainStaticMembers
{
	public static ApplicationSettings CurrentSettings { get; set; }
	public static string ApiEndpoint
	{
		get
		{
			if (CurrentSettings == null)
			{
				return string.Empty;
			}

			string value = CurrentSettings.ApiEndpoint;
			return value;
		}
	}

	public static double ApiConnectionTimeout
	{
		get
		{
			if (CurrentSettings == null)
			{
				return ConstantValues.PING_TIMEOUT_SECONDS;
			}

			var value = CurrentSettings.ApiTimeOutSeconds;
			return value;
		}
	}

	public static string UrlGetProducts => ApiEndpoint + ConstantValues.URL_GET_PRODUCTS;
	public static string UrlGetProductById => ApiEndpoint + ConstantValues.URL_GET_PRODUCT_BY_ID;



	public static string UrlGetCartTotalPriceByUserName => ApiEndpoint + ConstantValues.URL_GET_CART_TOTAL_PRICE_BY_USER_NAME;
	public static string UrlGetCartItemsByUserName => ApiEndpoint + ConstantValues.URL_GET_CART_ITEMS_BY_USER_NAME;
	public static string UrlPostAddCartItem => ApiEndpoint + ConstantValues.URL_POST_ADD_CART_ITEM;

}
