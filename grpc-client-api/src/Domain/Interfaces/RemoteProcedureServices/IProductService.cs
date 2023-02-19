using Domain.Entities;
using Domain.Models.Requests.v1;

namespace Domain.Interfaces.RemoteProcedureServices;

public interface IProductService
{
    Task<List<ProductModel>> GetProductsPagedAsync(int limit, string searchAfter, CancellationToken cancellationToken = default);

    Task<ProductModel> GetProductByIdAsync(string id, CancellationToken cancellationToken = default);

    Task<ProductModel> AddProductAsync(AddProductDto product, CancellationToken cancellationToken = default);

    Task<ProductModel> UpdateProductAsync(string id, UpdateProductDto product, CancellationToken cancellationToken = default);

    Task DeleteProductAsync(string id, CancellationToken cancellationToken = default);
}
