using ConferenceBooking.Application.Rooms.Commands;
using ConferenceBooking.Application.Rooms.Commands.AddServiceToRoom;
using ConferenceBooking.Application.Rooms.Commands.DeleteRoom;
using ConferenceBooking.Application.Rooms.Dtos;
using ConferenceBooking.Application.Rooms.Queries.GetRoomById;
using ConferenceBooking.Application.Rooms.Queries.SearchAvailableRooms;
using Mediator;
using Microsoft.AspNetCore.Mvc;

namespace ConferenceBooking.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RoomsController : ControllerBase
{
    private readonly IMediator _mediator;

    public RoomsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Guid>> Create(CreateRoomCommand command, CancellationToken ct)
    {
        var id = await _mediator.Send(command, ct);
        return CreatedAtAction(nameof(GetById), new { id }, id);
    }


    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(RoomDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RoomDto>> GetById(Guid id, CancellationToken ct)
    {
        var room = await _mediator.Send(new GetRoomByIdQuery(id), ct);
        return Ok(room);
    }

  
    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateRoomRequest request, CancellationToken ct)
    {
        await _mediator.Send(
            new UpdateRoomCommand(id, request.Name, request.Capacity, request.BaseHourlyRate), ct);
        return NoContent();
    }

  
    [HttpPost("{id:guid}/services/{serviceId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AddService(Guid id, Guid serviceId, CancellationToken ct)
    {
        await _mediator.Send(new AddServiceToRoomCommand(id, serviceId), ct);
        return NoContent();
    }

    
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await _mediator.Send(new DeleteRoomCommand(id), ct);
        return NoContent();
    }

    
    [HttpGet("available")]
    [ProducesResponseType(typeof(List<RoomDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<RoomDto>>> SearchAvailable(
        [FromQuery] DateTime start,
        [FromQuery] DateTime end,
        [FromQuery] int capacity,
        CancellationToken ct)
    {
        var rooms = await _mediator.Send(new SearchAvailableRoomsQuery(start, end, capacity), ct);
        return Ok(rooms);
    }
}

public record UpdateRoomRequest(string Name, int Capacity, decimal BaseHourlyRate);