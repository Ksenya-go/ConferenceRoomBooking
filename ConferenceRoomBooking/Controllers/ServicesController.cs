using ConferenceBooking.Application.Services.Commands;
using ConferenceBooking.Application.Services.Dtos;
using ConferenceBooking.Application.Services.Queries.GetAllServices;
using ConferenceBooking.Application.Services.Queries.GetServiceById;
using Mediator;
using Microsoft.AspNetCore.Mvc;

namespace ConferenceBooking.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ServicesController : ControllerBase
{
    private readonly IMediator _mediator;

    public ServicesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Guid>> Create(CreateServiceCommand command, CancellationToken ct)
    {
        var id = await _mediator.Send(command, ct);
        return CreatedAtAction(nameof(GetById), new { id }, id);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ServiceDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ServiceDto>> GetById(Guid id, CancellationToken ct)
    {
        var service = await _mediator.Send(new GetServiceByIdQuery(id), ct);
        return Ok(service);
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<ServiceDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<ServiceDto>>> GetAll(
        [FromQuery] bool activeOnly = true, CancellationToken ct = default)
    {
        var services = await _mediator.Send(new GetAllServicesQuery(activeOnly), ct);
        return Ok(services);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateServiceRequest request, 
        CancellationToken ct)
    {
        await _mediator.Send(new UpdateServiceCommand(id, request.Name, request.Price), ct);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await _mediator.Send(new DeleteServiceCommand(id), ct);
        return NoContent();
    }
}

public record UpdateServiceRequest(string Name, decimal Price);