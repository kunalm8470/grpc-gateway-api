using Domain.Entities;
using Domain.Models.Requests.v1;
using MediatR;

namespace Application.Products.v1.Commands;

public class AddProductCommand : IRequest<ProductModel>
{
    public AddProductDto Payload { get; set; }
}
