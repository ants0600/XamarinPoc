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

public class Cart
{
	public Cart()
	{
		this.UserName = string.Empty;
		this.CartList = new CartItem[0];
		this.TotalPrice = 0;
	}

	public long Id { get; set; }
	public string UserName { get; set; }
	public CartItem[] CartList { get; set; }
	public double TotalPrice { get; set; }
}