using Domain.Entities;
using Domain.Models.Requests;

namespace Domain.Interfaces;

public interface IProductsRepository
{
    public IAsyncEnumerable<ProductModel> GetProductsPaginatedAsync(string searchAfter, int limit, CancellationToken cancellationToken = default);

    public Task<ProductModel> GetProductsByIdAsync(string id, CancellationToken cancellationToken = default);

    public Task<ProductModel> AddProductAsync(AddProductDto productDto, CancellationToken cancellationToken = default);

    public Task<ProductModel> UpdateProductAsync(string id, UpdateProductDto productDto, CancellationToken cancellationToken = default);

    public Task<bool> DeleteProductAsync(string id, CancellationToken cancellationToken = default);
}
