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
/// todo: POCO class.
/// Used to deserialize JSON.
/// </summary>
public class ProductItem
{
	public long Id { get; set; }
	public string Code { get; set; }
	public string Name { get; set; }

	public double Price { get; set; }
	public string UnitName { get; set; }
	public string MobileCardImageUrl { get; set; }
	public string MobileCardImageFileName { get; set; }
}
