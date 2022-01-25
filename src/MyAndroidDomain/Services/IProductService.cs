using System.Collections.Generic;

public interface IProductService : IBaseApiService
{
	string GetImageFileName(ProductItem productItem);
	List<ProductItem> GetProductsFromApi();
	List<ProductItem> GetProductsFromFile();
	bool UpdateProductsFile(List<ProductItem> values);

	ProductItem GetProductByIdFromApi(long id);
	ProductItem GetProductByIdFromCacheFile(long productId);
	void UpdateProductsFile(ProductItem product);
}