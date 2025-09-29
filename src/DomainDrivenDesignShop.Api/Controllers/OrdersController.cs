using System.Collections.Generic;
using DomainDrivenDesignShop.Application;
using DomainDrivenDesignShop.Domain.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace DomainDrivenDesignShop.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class OrdersController(
    CreateOrderHandler createHandler,
    AddProductToOrderHandler addProductHandler,
    GetOrderHandler getHandler,
    ListOrdersHandler listHandler,
    UpdateOrderHandler updateHandler,
    DeleteOrderHandler deleteHandler) : ControllerBase
{
    private readonly CreateOrderHandler _createHandler = createHandler;
    private readonly AddProductToOrderHandler _addProductHandler = addProductHandler;
    private readonly GetOrderHandler _getHandler = getHandler;
    private readonly ListOrdersHandler _listHandler = listHandler;
    private readonly UpdateOrderHandler _updateHandler = updateHandler;
    private readonly DeleteOrderHandler _deleteHandler = deleteHandler;

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<OrderDto>>> GetAllAsync(CancellationToken ct)
    {
        var result = await _listHandler.HandleAsync(new ListOrdersQuery(), ct);
        return Ok(result);
    }


    [HttpGet("{id:guid}")]
    public async Task<ActionResult<OrderDto>> GetByIdAsync(Guid id, CancellationToken ct)
    {
        try
        {
            var result = await _getHandler.HandleAsync(new GetOrderQuery(id), ct);
            return Ok(result);
        }
        catch (DomainNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }


    [HttpPost]
    public async Task<ActionResult<CreateOrderResult>> CreateAsync([FromBody] CreateOrderRequest request, CancellationToken ct)
    {
        try
        {
            var result = await _createHandler.HandleAsync(new CreateOrderCommand(request.Currency), ct);
            return CreatedAtAction(nameof(GetByIdAsync), new { id = result.OrderId }, result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }


    [HttpPost("{orderId:guid}/lines")]
    public async Task<IActionResult> AddLineAsync(Guid orderId, [FromBody] AddLineRequest request, CancellationToken ct)
    {
        try
        {
            await _addProductHandler.HandleAsync(new AddProductToOrderCommand(orderId, request.ProductId, request.Quantity), ct);
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
    }


    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateAsync(Guid id, [FromBody] UpdateOrderRequest request, CancellationToken ct)
    {
        try
        {
            await _updateHandler.HandleAsync(new UpdateOrderCommand(id, request.Currency), ct);
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
            await _deleteHandler.HandleAsync(new DeleteOrderCommand(id), ct);
            return NoContent();
        }
        catch (DomainNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }
}


public sealed record CreateOrderRequest(string Currency);
public sealed record UpdateOrderRequest(string Currency);
public sealed record AddLineRequest(Guid ProductId, int Quantity);
