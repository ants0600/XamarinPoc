using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using Autofac;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyAndroidAppOne.Models.Controls
{
	public class CartPage
	{
		private readonly AppCompatActivity _parent;
		private Button btCartBack;
		private ListView lsCart;
		private readonly ICartService _cartService;
		public Cart CurrentCart { get; private set; }
		public CartPage(AppCompatActivity parent)
		{
			_parent = parent;
			_cartService = GlobalObjects
				.MainActivity
				.Resolve<ICartService>();

		}

		/// <summary>
		/// starting point of cart page
		/// </summary>
		public void LoadPage()
		{
			try
			{
				this._parent.SetContentView(Resource.Layout.CartPageLayout);

				this.GetControls();
				this.SetEvents();

				this.DisplayCartList();
			}
			catch (Exception x)
			{
				UiHelper.HandleUiException(x, this._parent);
			}
		}

		/// <summary>
		/// Refresh from API or cache file.
		/// display all cart items
		/// </summary>
		private void DisplayCartList()
		{
			this.RefreshCart();
			this.lsCart.Adapter = new CartPageItemAdapter(this._parent, this.CurrentCart.CartList);
		}

		/// <summary>
		/// try get from API.
		/// </summary>
		private bool RefreshCart()
		{
			try
			{
				this.CurrentCart = this._cartService.GetCartFromApi(GlobalObjects.CurrentUserName);
				this._cartService.SaveCartToCacheFile(this.CurrentCart);
				return true;
			}
			catch (TimeoutException)
			{
				UiHelper.PopupApiTimeoutError();
				this.CurrentCart = this._cartService.GetCartFromCacheFile();
				return true;
			}
			catch (Exception x)
			{
				UiHelper.HandleUiException(x, this._parent);
				return false;
			}
		}

		private void SetEvents()
		{
			this.btCartBack.Click -= OnClick;
			this.btCartBack.Click += OnClick;
		}

		private void OnClick(object sender, EventArgs e)
		{
			try
			{
				if (sender == this.btCartBack)
				{
					this.BackToMainPage();
				}
			}
			catch (Exception x)
			{
				UiHelper.HandleUiException(x, this._parent);
			}
		}

		private void BackToMainPage()
		{
			GlobalObjects.MainActivity.Show(false);
			GlobalObjects.MainActivity.TotalPrice = this.CurrentCart.TotalPrice;
		}

		private void GetControls()
		{
			this.btCartBack = this._parent.FindViewById<Button>(Resource.Id.btCartBack);
			this.lsCart = this._parent.FindViewById<ListView>(Resource.Id.lsCart);
		}

		public void RefreshCart(bool isRefreshingData)
		{
			if (!isRefreshingData)
			{
				return;
			}

			this.CurrentCart = this._cartService.GetCartFromCacheFile();
			this.SyncCartTotalPrice();
			this.SyncCartItems();
		}

		private bool SyncCartItems()
		{
			try
			{
				Cart currentCart = this.CurrentCart;
				if (currentCart == null)
				{
					return false;
				}

				currentCart.CartList = this._cartService.GetCartItemsFromApi(GlobalObjects.CurrentUserName);
				this._cartService.SaveCartToCacheFile(currentCart);
				return true;
			}
			catch (TimeoutException)
			{
				return true;
			}
			catch (Exception x)
			{
				UiHelper.HandleUiException(x, this._parent);
				return false;
			}
		}

		private void SyncCartTotalPrice()
		{
			double total = 0;
			Cart cart = this.CurrentCart;
			if (cart == null)
			{
				return;
			}

			try
			{
				total = cart.TotalPrice = this._cartService.GetCartTotalPriceFromApi(GlobalObjects.CurrentUserName);
				this._cartService.SaveCartToCacheFile(cart);
			}
			catch (TimeoutException)
			{
				total = cart.TotalPrice;
			}
			catch (Exception x)
			{
				UiHelper.HandleUiException(x, this._parent);
			}

			GlobalObjects.MainActivity.TotalPrice = total;
		}
	}
}