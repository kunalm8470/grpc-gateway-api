using MediatR;

namespace Application.Products.v1.Commands;

public class DeleteProductCommand : IRequest<Unit>
{
    public Guid Id { get; set; }
}
