using Domain.Entities;
using Domain.Interfaces.RemoteProcedureServices;
using MediatR;

namespace Application.Products.v1.Commands;

public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, ProductModel>
{
    private readonly IProductService _productService;

    public UpdateProductCommandHandler(IProductService productService)
    {
        _productService = productService;
    }

    public async Task<ProductModel> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        return await _productService.UpdateProductAsync(request.Id.ToString(), request.Payload, cancellationToken);
    }
}
