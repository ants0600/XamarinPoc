public static class ConstantValues
{
	public const string TOTAL_PRICE_FORMAT = "RM {0:#.00}";


	internal const string URL_GET_PRODUCTS = "/GetAllProducts?api-version=1.0";
	internal const string URL_GET_PRODUCT_BY_ID = "/GetProductById?id={0}&api-version=1.0";
	internal const string URL_GET_CART_TOTAL_PRICE_BY_USER_NAME = "/GetCartTotalPriceByUserName?userName={0}&api-version=1.0";
	internal const string URL_GET_CART_ITEMS_BY_USER_NAME = "/GetCartItemsByUserName?userName={0}&api-version=1.0";
	internal const string URL_POST_ADD_CART_ITEM = "/AddCartItem?api-version=1.0";


	public const string ERROR_MESSAGE_FORMAT = @"
EXCEPTION: {0}

MESSAGE: {1}

STACK TRACE: {2}
";
	public const string TITLE_ERROR_MESSAGE = "Error happens";

	public const string LABEL_OK = "OK";
	public const string PRODUCTS_FILE_NAME = "products.json";
	public const double PRODUCTS_API_TIMEOUT = 5;
	public const string MESSAGE_CANNOT_CONNECT_API = "Cannot connect API, system will load last saved data";
	public const int IMAGE_TIMEOUT = 5000;
	public const string PATH_APPSETTINGS = "appsettings.json";
	public const string PATH_DEFAULT_PRODUCT_IMAGE = "default-product-card.png";
	public const string PNG_EXTENSION = ".png";
	public const string FIELD_QUANTITY = "Quantity";
	public const string ERROR_INVALID_FIELD_FORMAT = "[{0}] is invalid";
	public static readonly string ErrorMessageInvalidQuantity = string.Format(ERROR_INVALID_FIELD_FORMAT, FIELD_QUANTITY);
	internal const double PING_TIMEOUT_SECONDS = 2;
	public const string CART_FILE_NAME = "cart.json";
	public const string TITLE_INVALID_DATA = "The data is invalid";
	public const string TEXT_SUCCESS_ADDING_CART = "Product is added to cart";
	public const string TITLE_MESSAGE = "Message";
	public const string JSON_CONTENT_TYPE = "application/json";
	public const string UNIT_PRICE_INFO_FORMAT = "{0} (RM {1:#.00} / {2})";
}
