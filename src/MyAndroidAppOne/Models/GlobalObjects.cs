using MyAndroidAppOne;
using MyAndroidAppOne.Models.Controls;
using Newtonsoft.Json;

/// <summary>
/// to access static objects. Ex:
/// all pages
/// </summary>
public static class GlobalObjects
{
	private static ComplexProductListPage _complexProductListPage = null;
	private static CartPage _cartPage = null;

	public static MainActivity MainActivity { get; internal set; }

	/// <summary>
	/// get global complex product list page control.
	/// lazy load
	/// </summary>
	public static ComplexProductListPage ComplexProductListPage
	{
		get
		{
			_complexProductListPage = _complexProductListPage == null ? new ComplexProductListPage(GlobalObjects.MainActivity) : _complexProductListPage;
			return _complexProductListPage;
		}
	}

	/// <summary>
	/// todo: use dummy for now
	/// </summary>
	public static string CurrentUserName { get; internal set; } = "johndoe";
	public static CartPage CartPage
	{
		get
		{
			if (_cartPage == null)
			{
				_cartPage = new CartPage(GlobalObjects.MainActivity);
			}
			return _cartPage;
		}
	}
}
