using Android.Content;
using Android.Graphics;
using Android.Views;
using Android.Widget;
using Autofac;
using MyAndroidAppOne;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class ComplexProductListAdapter : BaseAdapter<ProductItem>
{
	private readonly ICartService _cartService;
	private readonly Context _mContext;
	private readonly List<ProductItem> _dataSource;
	private readonly Bitmap _defaultImageBitmap;
	private readonly IHttpService _httpService;
	private readonly IFileService _fileService;
	private readonly bool _isOnline;

	/// <summary>
	/// for flagging which row is loaded images.
	/// key: file name,
	/// value: bitmap
	/// </summary>
	protected readonly SortedList<string, Bitmap> _loadedCardImages = new SortedList<string, Bitmap>();
	public ComplexProductListAdapter(Context mContext, List<ProductItem> dataSource, Bitmap defaultImageBitmap, bool isOnline)
	{
		this._httpService = GlobalObjects.MainActivity.Resolve<IHttpService>();
		this._fileService = GlobalObjects.MainActivity.Resolve<IFileService>();
		this._cartService = GlobalObjects
			.MainActivity
			.Resolve<ICartService>();
		this._mContext = mContext;
		this._dataSource = dataSource ?? new List<ProductItem>();
		_defaultImageBitmap = defaultImageBitmap;
		_isOnline = isOnline;
		this._loadedCardImages.Clear();
	}

	public override int Count => _dataSource.Count;

	/// <summary>
	/// todo: convert data row to string
	/// </summary>
	public override ProductItem this[int position]
	{
		get { return this._dataSource[position]; }
	}
	public override long GetItemId(int position)
	{
		return position;
	}

	/// <summary>
	/// prevent double loading on scroll
	/// </summary>
	public override View GetView(int position, View convertView, ViewGroup parent)
	{
		//get data source
		if (convertView == null)
		{
			convertView = LayoutInflater.From(this._mContext).Inflate(Resource.Layout.ComplexProductListLayout, null, false);
		}

		var item = this[position];

		//get controls
		var lbComplexProductListItemCode = convertView.FindViewById<TextView>(Resource.Id.lbComplexProductListItemCode);
		var lbComplexProductListItemName = convertView.FindViewById<TextView>(Resource.Id.lbComplexProductListItemName);
		var lbComplexProductListItemPrice = convertView.FindViewById<TextView>(Resource.Id.lbComplexProductListItemPrice);
		var lbComplexProductListItemUnit = convertView.FindViewById<TextView>(Resource.Id.lbComplexProductListItemUnit);
		var btComplexProductListAddToCart = convertView.FindViewById<Button>(Resource.Id.btComplexProductListAddToCart);
		var lbComplexProductListItemProductId = convertView.FindViewById<TextView>(Resource.Id.lbComplexProductListItemProductId);
		var imgCard = convertView.FindViewById<ImageView>(Resource.Id.imgCard);

		//set values
		var price = $"{item.Price}";

		lbComplexProductListItemProductId.Text = $"{item.Id}";
		lbComplexProductListItemCode.Text = item.Code;
		lbComplexProductListItemName.Text = item.Name;
		lbComplexProductListItemPrice.Text = price;
		lbComplexProductListItemUnit.Text = item.UnitName;
		btComplexProductListAddToCart.Click -= OnClick;
		btComplexProductListAddToCart.Click += OnClick;

		imgCard.SetImageBitmap(this._defaultImageBitmap);

		this.DisplayImage(imgCard, item);

		return convertView;
	}

	/// <summary>
	/// dowmload first,
	/// save to file
	/// </summary>
	private async void DisplayImage(ImageView imgCard, ProductItem productItem)
	{
		await this.OnSettingImage(imgCard, productItem);
	}

	/// <summary>
	/// download image.
	/// </summary>
	private async Task OnSettingImage(ImageView imgCard, ProductItem productItem)
	{
		try
		{
			Bitmap bm;
			var fileName = productItem.MobileCardImageFileName;
			if (this._loadedCardImages.ContainsKey(fileName))
			{
				bm = this._loadedCardImages[fileName];
			}
			else if (this._isOnline)
			{
				bm = this._httpService.GetBitmapFromUrl(productItem.MobileCardImageUrl);
				this._fileService.ExportBitmapToCacheFile(bm, fileName);
			}
			else
			{
				bm = this._fileService.GetBitmapFromCacheFile(fileName);
			}

			imgCard.SetImageBitmap(bm);
			this._loadedCardImages[fileName] = bm;
		}
		catch (Exception x)
		{
			UiHelper.HandleUiException(x, this._mContext);
		}


	}

	private void OnClick(object sender, EventArgs e)
	{
		try
		{
			this.AddCartItem(sender, e);
		}
		catch (Exception x)
		{
			UiHelper.HandleUiException(x, GlobalObjects.MainActivity);
		}
	}

	/// <summary>
	/// prepare object to insert.
	/// Validate data.
	/// Then perform data insertion.
	/// </summary>
	private bool AddCartItem(object sender, EventArgs e)
	{
		var button = sender as Button;
		if (button == null)
		{
			return false;
		}

		if (button.Id != Resource.Id.btComplexProductListAddToCart)
		{
			return false;
		}

		//get controls
		var parent = (View)button.Parent;
		var txComplexProductListItemQuantity = parent.FindViewById<EditText>(Resource.Id.txComplexProductListItemQuantity);
		var lbComplexProductListItemProductId = parent.FindViewById<TextView>(Resource.Id.lbComplexProductListItemProductId);

		//get values
		var quantity = TypeService.ParseInteger(txComplexProductListItemQuantity.Text);
		var productId = TypeService.ParseLong(lbComplexProductListItemProductId.Text);

		//validate
		if (quantity <= 0)
		{
			UiHelper.Popup(ConstantValues.TITLE_INVALID_DATA, ConstantValues.ErrorMessageInvalidQuantity, GlobalObjects.MainActivity);
			txComplexProductListItemQuantity.SelectAll();
			return false;
		}

		//save to file
		var inserted = new CartItem()
		{
			ProductId = productId,
			Quantity = quantity
		};

		//refresh total price
		var value = this.InsertCart(inserted);
		if (value)
		{
			var cart = this._cartService.GetCartFromCacheFile();
			GlobalObjects.MainActivity.TotalPrice = cart.TotalPrice;
			UiHelper.Popup(ConstantValues.TITLE_MESSAGE, ConstantValues.TEXT_SUCCESS_ADDING_CART, GlobalObjects.MainActivity);
		}
		return value;


	}

	/// <summary>
	/// insert to cache file first.
	/// Then invoke API.
	/// </summary>
	private bool InsertCart(CartItem inserted)
	{
		try
		{
			//try invoke API
			var request = new AddCartItemRequest()
			{
				ProductId = inserted.ProductId,
				Quantity = inserted.Quantity,
				UserName = GlobalObjects.CurrentUserName
			};

			this._cartService.AddCartItemViaApi(request);

			//then refresh the cart
			var cart = this._cartService.GetCartFromApi(GlobalObjects.CurrentUserName);
			this._cartService.SaveCartToCacheFile(cart);
		}
		catch (TimeoutException)
		{
			//save in cache file first
			this._cartService.CopyProductProperties(inserted);
			var value = this._cartService.AddCartItemToCacheFile(inserted);

			//display last total price
			var cart = this._cartService.GetCartFromCacheFile();
			GlobalObjects.MainActivity.TotalPrice = cart.TotalPrice;
		}
		catch (Exception x)
		{
			UiHelper.HandleUiException(x, this._mContext);
			return false;
		}

		return true;
	}
}
