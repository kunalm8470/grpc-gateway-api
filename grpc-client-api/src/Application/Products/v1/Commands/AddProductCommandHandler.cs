using Domain.Entities;
using Domain.Interfaces.RemoteProcedureServices;
using MediatR;

namespace Application.Products.v1.Commands;

public class AddProductCommandHandler : IRequestHandler<AddProductCommand, ProductModel>
{
    private readonly IProductService _productService;

    public AddProductCommandHandler(IProductService productService)
    {
        _productService = productService;
    }

    public async Task<ProductModel> Handle(AddProductCommand request, CancellationToken cancellationToken)
    {
        return await _productService.AddProductAsync(request.Payload, cancellationToken);
    }
}
