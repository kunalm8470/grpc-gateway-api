using Api.Protos;
using AutoMapper;
using Domain.Entities;
using Domain.Exceptions;
using Domain.Interfaces;
using Domain.Models.Requests;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.AspNetCore.Http;
using System.Threading;
using System.Threading.Tasks;
using static Api.Protos.ProductsService;

namespace Api.Services;

public class ProductsService : ProductsServiceBase
{
    private readonly IMapper _mapper;

    private readonly IProductsRepository _productsRepository;

    public ProductsService(IMapper mapper, IProductsRepository productsRepository)
    {
        _mapper = mapper;
        
        _productsRepository = productsRepository;
    }

    public override async Task GetProducts(ProductPaginationFilter request, IServerStreamWriter<Product> responseStream, ServerCallContext context)
    {
        CancellationToken cancellationToken = context.CancellationToken;

        await foreach (ProductModel product in _productsRepository.GetProductsPaginatedAsync(request.SearchAfter, request.Limit).WithCancellation(cancellationToken))
        {
            await responseStream.WriteAsync(_mapper.Map<Product>(product), cancellationToken);
        }
    }

    public override async Task<Product> GetProductById(ProductIdFilter request, ServerCallContext context)
    {
        ProductModel found = await _productsRepository.GetProductsByIdAsync(request.Id, context.CancellationToken);

        if (found is null)
        {
            throw new ProductNotFoundException($"Product with Id {request.Id} not found");
        }

        return _mapper.Map<Product>(found);
    }

    public override async Task<Product> AddProduct(AddProductPayload request, ServerCallContext context)
    {
        AddProductDto newProduct = _mapper.Map<AddProductDto>(request);

        ProductModel createdProduct = await _productsRepository.AddProductAsync(newProduct, context.CancellationToken);

        return _mapper.Map<Product>(createdProduct);
    }

    public override async Task<Product> UpdateProduct(UpdateProductPayload request, ServerCallContext context)
    {
        UpdateProductDto replaceProduct = _mapper.Map<UpdateProductDto>(request.Payload);

        ProductModel updatedProduct = await _productsRepository.UpdateProductAsync(request.Id, replaceProduct, context.CancellationToken);

        return _mapper.Map<Product>(updatedProduct);
    }

    public override async Task<Empty> DeleteProduct(ProductIdFilter request, ServerCallContext context)
    {
        bool isProductDeleted = await _productsRepository.DeleteProductAsync(request.Id, context.CancellationToken);

        return new Empty();
    }
}
