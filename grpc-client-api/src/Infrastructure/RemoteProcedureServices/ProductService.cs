using AutoMapper;
using Domain.Constants;
using Domain.Entities;
using Domain.Exceptions;
using Domain.Interfaces.RemoteProcedureServices;
using Domain.Models.Requests.v1;
using Grpc.Core;
using Grpc.Net.Client;
using Infrastructure.Protos;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;
using System.Reflection.PortableExecutable;
using static Infrastructure.Protos.ProductsService;

namespace Infrastructure.RemoteProcedureServices;

public class ProductService : ProductsServiceClient, IProductService
{
    private readonly ProductsServiceClient _client;

    private readonly IMapper _mapper;

    private readonly string _correlationIdHeaderValue;

    public ProductService(
        GrpcChannel channel,
        IMapper mapper,
        IHttpContextAccessor httpContextAccessor)
    {
        _client = new ProductsServiceClient(channel);

        _mapper = mapper;

        _correlationIdHeaderValue = httpContextAccessor.HttpContext.Request.Headers[CorrelationIdConstants.CORRELATIONID_HEADER].FirstOrDefault();
    }

    public async Task<ProductModel> AddProductAsync(AddProductDto product, CancellationToken cancellationToken = default)
    {
        Metadata headers = new()
        {
            { CorrelationIdConstants.CORRELATIONID_HEADER, _correlationIdHeaderValue }
        };

        AddProductPayload newProduct = _mapper.Map<AddProductPayload>(product);

        Product created = await _client.AddProductAsync(newProduct, headers: headers, cancellationToken: cancellationToken);

        return _mapper.Map<ProductModel>(created);
    }

    public async Task DeleteProductAsync(string id, CancellationToken cancellationToken = default)
    {
        Metadata headers = new()
        {
            { CorrelationIdConstants.CORRELATIONID_HEADER, _correlationIdHeaderValue }
        };

        ProductIdFilter deleteRequest = new()
        {
            Id = id
        };

        await _client.DeleteProductAsync(deleteRequest, headers: headers, cancellationToken: cancellationToken);
    }

    public async Task<ProductModel> GetProductByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        try
        {
            Metadata headers = new()
            {
                { CorrelationIdConstants.CORRELATIONID_HEADER, _correlationIdHeaderValue }
            };

            ProductIdFilter getProductRequest = new()
            {
                Id = id
            };

            Product product = await _client.GetProductByIdAsync(getProductRequest, headers: headers, cancellationToken: cancellationToken);

            return _mapper.Map<ProductModel>(product);
        }
        catch (RpcException ex) when (ex.Status.StatusCode == StatusCode.NotFound)
        {
            throw new ProductNotFoundException(ex.Message);
        }
    }

    public async Task<List<ProductModel>> GetProductsPagedAsync(int limit, string searchAfter, CancellationToken cancellationToken = default)
    {
        Metadata headers = new()
        {
            { CorrelationIdConstants.CORRELATIONID_HEADER, _correlationIdHeaderValue }
        };

        ProductPaginationFilter filter = new()
        {
            Limit = limit,
            SearchAfter = searchAfter ?? string.Empty
        };

        List<ProductModel> products = new(limit);

        using AsyncServerStreamingCall<Product> call = _client.GetProducts(filter, deadline: DateTime.UtcNow.AddSeconds(60), headers: headers, cancellationToken: cancellationToken);

        await foreach (Product product in call.ResponseStream.ReadAllAsync(cancellationToken: cancellationToken))
        {
            products.Add(_mapper.Map<ProductModel>(product));
        }

        return products;
    }

    public async Task<ProductModel> UpdateProductAsync(string id, UpdateProductDto product, CancellationToken cancellationToken = default)
    {
        Metadata headers = new()
        {
            { CorrelationIdConstants.CORRELATIONID_HEADER, _correlationIdHeaderValue }
        };

        UpdateProductPayload payload = new()
        {
            Id = id,
            Payload = _mapper.Map<UpdateProductPayloadObject>(product)
        };

        Product updated = await _client.UpdateProductAsync(payload, headers: headers, cancellationToken: cancellationToken);

        return _mapper.Map<ProductModel>(updated);
    }
}
