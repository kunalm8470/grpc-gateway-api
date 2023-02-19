using Application.Products.v1.Commands;
using Application.Products.v1.Queries;
using Domain.Common;
using Domain.Entities;
using Domain.Models.Requests.v1;
using FluentValidation;
using FluentValidation.AspNetCore;
using FluentValidation.Results;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.v1;

[Route("api/[controller]")]
[ApiController]
[ApiVersion("1.0")]
public class ProductsController : ControllerBase
{
    private readonly IMediator _mediator;

    private readonly IValidator<GetProductsPaginationQuery> _getProductsValidator;

    private readonly IValidator<AddProductDto> _addProductsValidator;
    
    private readonly IValidator<UpdateProductDto> _updateProductsValidator;

    public ProductsController(
        IMediator mediator,
        IValidator<GetProductsPaginationQuery> getProductsValidator,
        IValidator<AddProductDto> addProductsValidator,
        IValidator<UpdateProductDto> updateProductsValidator
    )
    {
        _mediator = mediator;

        _getProductsValidator = getProductsValidator;

        _addProductsValidator = addProductsValidator;

        _updateProductsValidator = updateProductsValidator;
    }

    [HttpGet]
    public async Task<ActionResult<PagedEntity<ProductModel>>> GetProductsPagedAsync([FromQuery] GetProductsPaginationQuery productsFilter, CancellationToken cancellationToken)
    {
        ValidationResult result = _getProductsValidator.Validate(productsFilter);

        if (!result.IsValid)
        {
            result.AddToModelState(ModelState);

            return ValidationProblem(ModelState);
        }

        PagedEntity<ProductModel> products = await _mediator.Send(new GetProductsWithPaginationQuery
        {
            SearchAfter = productsFilter.SearchAfter,
            Limit = productsFilter.Limit
        }, cancellationToken);

        return Ok(products);
    }

    [HttpGet("{id:Guid}")]
    public async Task<ActionResult<ProductModel>> GetProductByIdAsync([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        ProductModel found = await _mediator.Send(new GetProductByIdQuery { Id = id }, cancellationToken);

        return Ok(found);
    }

    [HttpPost]
    public async Task<ActionResult<ProductModel>> AddProductAsync([FromBody] AddProductDto product, CancellationToken cancellationToken)
    {
        ValidationResult result = _addProductsValidator.Validate(product);

        if (!result.IsValid)
        {
            result.AddToModelState(ModelState);

            return ValidationProblem(ModelState);
        }

        ProductModel added = await _mediator.Send(new AddProductCommand { Payload = product }, cancellationToken);

        return CreatedAtRoute("GetProductById", new { id = added.Id }, added);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ProductModel>> UpdateProductAsync([FromRoute] Guid id, [FromBody] UpdateProductDto product, CancellationToken cancellationToken)
    {
        ValidationResult result = _updateProductsValidator.Validate(product);

        if (!result.IsValid)
        {
            result.AddToModelState(ModelState);

            return ValidationProblem(ModelState);
        }

        ProductModel replaced = await _mediator.Send(new UpdateProductCommand { Payload = product, Id = id }, cancellationToken);

        return Ok(replaced);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteProductAsync([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        await _mediator.Send(new DeleteProductCommand { Id = id }, cancellationToken);

        return NoContent();
    }
}
