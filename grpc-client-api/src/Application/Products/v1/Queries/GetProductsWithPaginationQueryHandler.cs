using Domain.Common;
using Domain.Entities;
using Domain.Interfaces.RemoteProcedureServices;
using MediatR;
using System.Globalization;

namespace Application.Products.v1.Queries;

public class GetProductsWithPaginationQueryHandler : IRequestHandler<GetProductsWithPaginationQuery, PagedEntity<ProductModel>>
{
    private readonly IProductService _productService;

    public GetProductsWithPaginationQueryHandler(IProductService productService)
    {
        _productService = productService;
    }

    public async Task<PagedEntity<ProductModel>> Handle(GetProductsWithPaginationQuery request, CancellationToken cancellationToken)
    {
        List<ProductModel> products = await _productService.GetProductsPagedAsync(request.Limit, request.SearchAfter, cancellationToken);

        string prev = request.SearchAfter;

        string next = products[^1]?.CreatedAt.ToString("o", CultureInfo.InvariantCulture);

        return new(products, prev, next);
    }
}
