using System.Collections.Generic;
using DomainDrivenDesignShop.Application;
using DomainDrivenDesignShop.Domain.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace DomainDrivenDesignShop.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class ProductsController(
    CreateProductHandler createHandler,
    UpdateProductHandler updateHandler,
    DeleteProductHandler deleteHandler,
    GetProductHandler getHandler,
    ListProductsHandler listHandler) : ControllerBase
{
    private readonly CreateProductHandler _createHandler = createHandler;
    private readonly UpdateProductHandler _updateHandler = updateHandler;
    private readonly DeleteProductHandler _deleteHandler = deleteHandler;
    private readonly GetProductHandler _getHandler = getHandler;
    private readonly ListProductsHandler _listHandler = listHandler;

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ProductDto>>> GetAllAsync(CancellationToken ct)
    {
        var result = await _listHandler.HandleAsync(new ListProductsQuery(), ct);
        return Ok(result);
    }


    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ProductDto>> GetByIdAsync(Guid id, CancellationToken ct)
    {
        try
        {
            var result = await _getHandler.HandleAsync(new GetProductQuery(id), ct);
            return Ok(result);
        }
        catch (DomainNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }


    [HttpPost]
    public async Task<ActionResult<CreateProductResult>> CreateAsync([FromBody] UpsertProductRequest request, CancellationToken ct)
    {
        try
        {
            var result = await _createHandler.HandleAsync(
                new CreateProductCommand(request.Name, request.Amount, request.Currency),
                ct);

            return CreatedAtAction(nameof(GetByIdAsync), new { id = result.ProductId }, result);
        }
        catch (DomainRuleViolationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }


    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateAsync(Guid id, [FromBody] UpsertProductRequest request, CancellationToken ct)
    {
        try
        {
            await _updateHandler.HandleAsync(new UpdateProductCommand(id, request.Name, request.Amount, request.Currency), ct);
            return NoContent();
        }
        catch (DomainNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (DomainRuleViolationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }


    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteAsync(Guid id, CancellationToken ct)
    {
        try
        {
            await _deleteHandler.HandleAsync(new DeleteProductCommand(id), ct);
            return NoContent();
        }
        catch (DomainNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }


    [HttpPost("seed")]
    public async Task<ActionResult<IEnumerable<Guid>>> SeedAsync(CancellationToken ct)
    {
        var product1 = await _createHandler.HandleAsync(new CreateProductCommand("Café 250g", 5.90m, "EUR"), ct);
        var product2 = await _createHandler.HandleAsync(new CreateProductCommand("Té Verde 100g", 3.50m, "EUR"), ct);

        return Ok(new[] { product1.ProductId, product2.ProductId });
    }
}


public sealed record UpsertProductRequest(string Name, decimal Amount, string Currency);
