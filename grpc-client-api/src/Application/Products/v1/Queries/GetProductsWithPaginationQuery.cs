using Domain.Common;
using Domain.Entities;
using MediatR;

namespace Application.Products.v1.Queries;

public class GetProductsWithPaginationQuery : IRequest<PagedEntity<ProductModel>>
{
    public string SearchAfter { get; set; }

    public int Limit { get; set; }
}
