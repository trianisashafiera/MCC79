using API.Contracts;
using API.Models;
using API.Utilities.Enums;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace API.Controllers;

[ApiController]
[Route("api/universities")]
public class UniversityController : GeneralController<IUniversityRepository, University>
{
    public UniversityController(IUniversityRepository repository) : base(repository)
    {
    }

    [HttpGet("Name")]
    public IActionResult GetByName(string name)
    {
        var universities = _repository.GetByName(name);
        if(!universities.Any())
        {
            return NotFound(new ResponseHandler<University>
            {
                Code = StatusCodes.Status404NotFound,
                Status = HttpStatusCode.NotFound.ToString(),
                Message = "No universities found with the given name"
            });
        }
        return Ok(new ResponseHandler<IEnumerable<University>>
        {
            Code = StatusCodes.Status200OK,
            Status = HttpStatusCode.OK.ToString(),
            Message = "Universities found",
            Data = universities
        });
    }

 }
