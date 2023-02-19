using Domain.Interfaces.RemoteProcedureServices;
using MediatR;

namespace Application.Products.v1.Commands;

public class DeleteProductHandler : IRequestHandler<DeleteProductCommand, Unit>
{
    private readonly IProductService _productService;

    public DeleteProductHandler(IProductService productService)
    {
        _productService = productService;
    }

    public async Task<Unit> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        await _productService.DeleteProductAsync(request.Id.ToString(), cancellationToken);

        return Unit.Value;
    }
}
