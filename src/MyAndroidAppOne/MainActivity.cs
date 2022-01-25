using Android.App;
using Android.OS;
using Android.Runtime;
using Android.Widget;
using AndroidX.AppCompat.App;
using Autofac;
using Newtonsoft.Json;
using System;

namespace MyAndroidAppOne
{
	/// <summary>
	/// startup page
	/// </summary>
	[Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
	public class MainActivity : AppCompatActivity
	{
		private readonly ContainerBuilder _builder = new ContainerBuilder();
		private readonly IContainer _container;
		private readonly ILifetimeScope _diScope;
		private TextView lbTotalPrice;
		private Button btShowCart;
		private double _totalPrice;
		private readonly IFileService _fileService;

		public void ReadSettings()
		{
			var stream = this.Assets.Open(ConstantValues.PATH_APPSETTINGS);
			string s = this._fileService.ReadTextFromStream(stream);
			var value = JsonConvert.DeserializeObject<ApplicationSettings>(s);

			DomainStaticMembers.CurrentSettings = value;
		}

		/// <summary>
		/// text of total price label
		/// </summary>
		public double TotalPrice
		{
			get
			{
				var view = this.lbTotalPrice;
				return this._totalPrice;
			}
			set
			{
				this._totalPrice = value;

				//set text
				var view = this.lbTotalPrice;
				if (view == null)
				{
					return;
				}
				view.Text = string.Format(ConstantValues.TOTAL_PRICE_FORMAT, value);
			}
		}

		/// <summary>
		/// setup DI functions
		/// </summary>
		public MainActivity()
		{
			GlobalObjects.MainActivity = this;

			//setup dependency injection
			UiAutofacInjector.Initialize(this._builder);
			this._container = this._builder.Build();
			this._diScope = this._container.BeginLifetimeScope();

			//resolve types
			this._fileService = this.Resolve<IFileService>();
		}

		public T Resolve<T>()
		{
			return this._container.Resolve<T>();
		}
		protected override void OnCreate(Bundle savedInstanceState)
		{
			try
			{
				base.OnCreate(savedInstanceState);
				Xamarin.Essentials.Platform.Init(this, savedInstanceState);

				this.ReadSettings();
				this.Show(true);

			}
			catch (Exception x)
			{
				UiHelper.HandleUiException(x, this);
			}
		}

		/// <summary>
		/// set active layout.
		/// load product list page.
		/// display total price.
		/// </summary>
		public void Show(bool isRefreshingData)
		{
			SetContentView(Resource.Layout.activity_main);
			this.LoadPage();
			GlobalObjects.ComplexProductListPage.LoadPage(isRefreshingData);
			GlobalObjects.CartPage.RefreshCart(isRefreshingData);
		}

		private void LoadPage()
		{
			this.GetControls();
			this.SetEvents();
		}



		private void SetEvents()
		{
			this.btShowCart.Click -= OnClick;
			this.btShowCart.Click += OnClick;
		}

		private void OnClick(object sender, EventArgs e)
		{
			try
			{
				if (sender == this.btShowCart)
				{
					this.ShowCartPage();
				}
			}
			catch (Exception x)
			{
				UiHelper.HandleUiException(x, GlobalObjects.MainActivity);
			}
		}

		/// <summary>
		/// show cart page
		/// </summary>
		private void ShowCartPage()
		{
			//load cart page
			GlobalObjects.CartPage.LoadPage();
		}

		private void GetControls()
		{
			this.lbTotalPrice = this.FindViewById<TextView>(Resource.Id.lbTotalPrice);
			this.btShowCart = this.FindViewById<Button>(Resource.Id.btShowCart);
		}

		public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
		{
			Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

			base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
		}



	}
}