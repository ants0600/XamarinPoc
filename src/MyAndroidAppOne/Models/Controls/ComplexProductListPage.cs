using Android.Graphics;
using Android.Widget;
using AndroidX.AppCompat.App;
using Autofac;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MyAndroidAppOne.Models.Controls
{
	/// <summary>
	/// code behind for complex product list page
	/// </summary>
	public class ComplexProductListPage
	{
		private readonly IProductService _productService;
		private EditText txMainSearch;
		private ListView lsProduct;
		private Button btMainSearch;
		private BaseAdapter productListAdapter;
		private bool _isOnline;
		private string _filterText;
		private readonly AppCompatActivity _parent;
		private readonly Stream _defaultProductImageStream;
		private readonly byte[] _defaultProductImageBytes = null;
		private readonly Bitmap _defaultProductImageBitmap;
		private readonly IFileService _fileService;
		private readonly ICartService _cartService;

		public List<ProductItem> DataSource { get; private set; }

		public ComplexProductListPage(AppCompatActivity parent)
		{
			this._cartService = GlobalObjects.MainActivity.Resolve<ICartService>();
			this._fileService = GlobalObjects.MainActivity.Resolve<IFileService>();
			this._productService = GlobalObjects.MainActivity.Resolve<IProductService>();

			_parent = parent;
			this._defaultProductImageStream = _parent.Assets.Open(ConstantValues.PATH_DEFAULT_PRODUCT_IMAGE);
			this._defaultProductImageBytes = this._fileService.ReadBytesFromStream(this._defaultProductImageStream);
			this._defaultProductImageBitmap = this._fileService.GetBitmapFromBytes(this._defaultProductImageBytes);
			this.DataSource = new List<ProductItem>();
		}

		/// <summary>
		/// Ping firts.
		/// get from API,
		/// then write file.
		/// If error, get from file.
		/// </summary>
		public List<ProductItem> ReadProductList()
		{
			this._isOnline = true;
			var values = new List<ProductItem>();
			try
			{
				this._productService.TestConnectivity();
				values = this._productService.GetProductsFromApi();
				bool isUpdated = this._productService.UpdateProductsFile(values);
			}
			catch (TimeoutException x)
			{
				this._isOnline = false;
				UiHelper.PopupApiTimeoutError();
				values = this._productService.GetProductsFromFile();
			}
			catch (Exception x)
			{
				UiHelper.HandleUiException(x, this._parent);
			}
			return values;
		}

		private void SetEvents()
		{
			this.btMainSearch.Click -= OnClick;
			this.btMainSearch.Click += OnClick;
		}

		private void OnClick(object sender, EventArgs e)
		{
			try
			{
				if (sender == this.btMainSearch)
				{
					this.RefreshData();
					return;
				}
			}
			catch (Exception x)
			{
				UiHelper.HandleUiException(x, this._parent);
			}
		}

		/// <summary>
		/// refresh product list.
		/// refresh total price.
		/// </summary>
		private async void RefreshData()
		{
			this.FilterProductList();
			this.RefreshCartTotalPrice();
		}

		private async Task FilterProductList()
		{
			this._filterText = $"{this.txMainSearch.Text}".Trim();

			//invoke filter
			var arrayAdapter = this.productListAdapter as ArrayAdapter;
			if (arrayAdapter != null)
			{
				arrayAdapter.Filter.InvokeFilter(_filterText);
				return;
			}

			var complexProductListAdapter = this.productListAdapter as ComplexProductListAdapter;
			if (complexProductListAdapter != null)
			{
				this.SetAdapters(true);
			}
		}

		private async Task RefreshCartTotalPrice()
		{
			try
			{
				var total = this._cartService.GetCartTotalPriceFromApi(GlobalObjects.CurrentUserName);
				GlobalObjects.MainActivity.TotalPrice = total;
				GlobalObjects.CartPage.CurrentCart.TotalPrice = total;
				this._cartService.SaveCartToCacheFile(GlobalObjects.CartPage.CurrentCart);
			}
			catch (TimeoutException)
			{

			}
			catch (Exception x)
			{
				UiHelper.HandleUiException(x, this._parent);
			}


		}

		/// <summary>
		/// get all controls in entire application
		/// </summary>
		private void GetControls()
		{
			this.txMainSearch = this._parent.FindViewById<EditText>(Resource.Id.txMainSearch);
			this.lsProduct = this._parent.FindViewById<ListView>(Resource.Id.lsProduct);
			this.btMainSearch = this._parent.FindViewById<Button>(Resource.Id.btMainSearch);
		}

		/// <summary>
		/// todo: set data source and view.
		/// </summary>
		private void SetAdapters(bool isRefreshingData)
		{
			////for simple listing
			//this.productListAdapter = new ArrayAdapter(this, Resource.Layout.SimpleProductListLayout, Resource.Id.lbProductListItemName, this._dummyData);

			//for complex listing
			if (isRefreshingData)
			{
				this.DataSource = this.ReadProductList();
			}

			//perform filter by name, code
			if (!string.IsNullOrEmpty(this._filterText))
			{
				DataSource = DataSource.Where(a =>
				{
					var code = $"{a.Code}";
					var name = $"{a.Name}";

					var isCodeExist = code.IndexOf(this._filterText, StringComparison.OrdinalIgnoreCase) >= 0;
					var isNameExist = name.IndexOf(this._filterText, StringComparison.OrdinalIgnoreCase) >= 0;
					return isCodeExist || isNameExist;
				}).ToList();
			}

			var adapter = new ComplexProductListAdapter(this._parent, DataSource, this._defaultProductImageBitmap, this._isOnline);
			this.productListAdapter = adapter;

			//invoke data binding
			this.lsProduct.Adapter = this.productListAdapter;
		}

		/// <summary>
		/// starting point
		/// </summary>
		public void LoadPage(bool isRefreshingData)
		{
			try
			{
				this.GetControls();
				this.SetEvents();
				this.SetAdapters(isRefreshingData);
			}
			catch (Exception x)
			{
				UiHelper.HandleUiException(x, GlobalObjects.MainActivity);
			}
		}
	}
}