using System.Collections.Generic;

public interface ICartService : IBaseApiService
{

	Cart GetCartFromCacheFile();
	bool AddCartItemToCacheFile(CartItem inserted);

	bool AddCartItemViaApi(AddCartItemRequest inserted);
	void CopyProductProperties(CartItem inserted);

	double GetCartTotalPriceFromApi(string userName);
	bool SaveCartToCacheFile(Cart cart);
	CartItem[] GetCartItemsFromApi(string userName);
	Cart GetCartFromApi(string userName);
	double CalculateTotalPrice(Cart cart);
}