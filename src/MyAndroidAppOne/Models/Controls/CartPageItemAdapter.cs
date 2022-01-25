using Android.Content;
using Android.Views;
using Android.Widget;
using MyAndroidAppOne;

public class CartPageItemAdapter : BaseAdapter<CartItem>
{
	private readonly CartItem[] _cartItems;
	private readonly Context _context;

	public CartPageItemAdapter(Context context, CartItem[] cartItems)
	{
		this._cartItems = cartItems ?? new CartItem[0];
		_context = context;
	}
	public override CartItem this[int position]
	{
		get { return this._cartItems[position]; }
	}

	public override int Count { get { return this._cartItems.Length; } }

	public override long GetItemId(int position)
	{
		return this._cartItems[position].Id;
	}

	public override View GetView(int position, View convertView, ViewGroup parent)
	{
		//get data source
		if (convertView == null)
		{
			convertView = LayoutInflater.From(this._context).Inflate(Resource.Layout.CartPageItemLayout, null, false);
		}

		//get controls
		var lbCartItemCode = convertView.FindViewById<TextView>(Resource.Id.lbCartItemCode);
		var lbCartItemName = convertView.FindViewById<TextView>(Resource.Id.lbCartItemName);
		var lbCartItemQuantity = convertView.FindViewById<TextView>(Resource.Id.lbCartItemQuantity);
		var lbCartItemTotalPrice = convertView.FindViewById<TextView>(Resource.Id.lbCartItemTotalPrice);

		//set properties
		var item = this[position];
		lbCartItemCode.Text = item.Code;
		lbCartItemName.Text = item.Name;
		lbCartItemQuantity.Text = string.Format(ConstantValues.UNIT_PRICE_INFO_FORMAT, item.Quantity, item.Price, item.UnitName);
		lbCartItemTotalPrice.Text = $"{item.TotalPrice}";

		return convertView;
	}
}
