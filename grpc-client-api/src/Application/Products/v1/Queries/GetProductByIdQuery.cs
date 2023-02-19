using Domain.Entities;
using MediatR;

namespace Application.Products.v1.Queries;

public class GetProductByIdQuery : IRequest<ProductModel>
{
    public Guid Id { get; set; }
}
