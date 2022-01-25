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

public class AddCartItemRequest
{
	public string UserName { get; set; }
	public long ProductId { get; set; }
	public int Quantity { get; set; }
}
