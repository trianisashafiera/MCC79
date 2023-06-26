using API.Contracts;
using API.Models;
using API.Utilities.Enums;
using API.DTOs.Bookings;
using API.Services;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace API.Controllers;

    [ApiController]
    [Route("api/bookings")]
public class BookingController : ControllerBase
{
    private readonly BookingService _service;

    public BookingController(BookingService service)
    {
        _service = service;
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        var entities = _service.GetBooking();

        if (!entities.Any())
        {
            return NotFound(new ResponseHandler<NewBookingDto>
            {
                Code = StatusCodes.Status404NotFound,
                Status = HttpStatusCode.NotFound.ToString(),
                Message = "Data not found"
            });
        }

        return Ok(new ResponseHandler<IEnumerable<NewBookingDto>>
        {
            Code = StatusCodes.Status200OK,
            Status = HttpStatusCode.OK.ToString(),
            Message = "Data found",
            Data = entities
        });
    }

    [HttpGet("{guid}")]
    public IActionResult GetByGuid(Guid guid)
    {
        var booking = _service.GetBooking(guid);
        if (booking is null)
        {
            return NotFound(new ResponseHandler<NewBookingDto>
            {
                Code = StatusCodes.Status404NotFound,
                Status = HttpStatusCode.NotFound.ToString(),
                Message = "Data not found"
            });
        }

        return Ok(new ResponseHandler<NewBookingDto>
        {
            Code = StatusCodes.Status200OK,
            Status = HttpStatusCode.OK.ToString(),
            Message = "Data found",
            Data = booking
        });
    }

    [HttpPost]
    public IActionResult Create(NewBookingDto newBookingDto)
    {
        var createdBooking = _service.CreateBooking(newBookingDto);
        if (createdBooking is null)
        {
            return BadRequest(new ResponseHandler<NewBookingDto>
            {
                Code = StatusCodes.Status400BadRequest,
                Status = HttpStatusCode.BadRequest.ToString(),
                Message = "Data not created"
            });
        }

        return Ok(new ResponseHandler<NewBookingDto>
        {
            Code = StatusCodes.Status200OK,
            Status = HttpStatusCode.OK.ToString(),
            Message = "Successfully created",
            Data = createdBooking
        });
    }

    [HttpPut]
    public IActionResult Update(NewBookingDto updateBookingDto)
    {
        var update = _service.UpdateBooking(updateBookingDto);
        if (update is -1)
        {
            return NotFound(new ResponseHandler<NewBookingDto>
            {
                Code = StatusCodes.Status404NotFound,
                Status = HttpStatusCode.NotFound.ToString(),
                Message = "Id not found"
            });
        }
        if (update is 0)
        {
            return BadRequest(new ResponseHandler<NewBookingDto>
            {
                Code = StatusCodes.Status500InternalServerError,
                Status = HttpStatusCode.InternalServerError.ToString(),
                Message = "Check your data"
            });
        }
        return Ok(new ResponseHandler<NewBookingDto>
        {
            Code = StatusCodes.Status200OK,
            Status = HttpStatusCode.OK.ToString(),
            Message = "Successfully updated"
        });
    }

    [HttpDelete]
    public IActionResult Delete(Guid guid)
    {
        var delete = _service.DeleteBooking(guid);

        if (delete is -1)
        {
            return NotFound(new ResponseHandler<NewBookingDto>
            {
                Code = StatusCodes.Status404NotFound,
                Status = HttpStatusCode.NotFound.ToString(),
                Message = "Id not found"
            });
        }
        if (delete is 0)
        {
            return BadRequest(new ResponseHandler<NewBookingDto>
            {
                Code = StatusCodes.Status500InternalServerError,
                Status = HttpStatusCode.InternalServerError.ToString(),
                Message = "Check connection to database"
            });
        }

        return Ok(new ResponseHandler<NewBookingDto>
        {
            Code = StatusCodes.Status200OK,
            Status = HttpStatusCode.OK.ToString(),
            Message = "Successfully deleted"
        });
    }

   /* [HttpGet("Name")]
    public IActionResult GetByName(string name)
    {
        var booking = _service.GetBooking(name);
        if (!booking.Any())
        {
            return NotFound(new ResponseHandler<NewBookingDto>
            {
                Code = StatusCodes.Status404NotFound,
                Status = HttpStatusCode.NotFound.ToString(),
                Message = "No booking found with the given name"
            });
        }

        return Ok(new ResponseHandler<IEnumerable<NewBookingDto>>
        {
            Code = StatusCodes.Status200OK,
            Status = HttpStatusCode.OK.ToString(),
            Message = "Booking found",
            Data = booking
        });
    }*/
}

