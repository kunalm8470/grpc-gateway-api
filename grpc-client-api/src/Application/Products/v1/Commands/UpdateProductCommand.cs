using Domain.Entities;
using Domain.Models.Requests.v1;
using MediatR;

namespace Application.Products.v1.Commands;

public class UpdateProductCommand : IRequest<ProductModel>
{
    public Guid Id { get; set; }
    public UpdateProductDto Payload { get; set; }
}
