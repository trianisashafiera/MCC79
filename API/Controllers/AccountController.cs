using API.Contracts;
using API.DTOs.Accounts;
using API.Models;
using API.Services;
using API.Utilities.Enums;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net;

namespace API.Controllers;

    [ApiController]
    [Route("api/accounts")]
public class AccountController : ControllerBase
{
    private readonly AccountService _service;
    private readonly EmployeeService _employeeService;
    private readonly EducationService _educationService;
    private readonly UniversityService _universityService;


    public AccountController(AccountService service, 
                             EmployeeService employeeService, 
                             EducationService educationService, 
                             UniversityService universityService)
    {
        _service = service;
        _employeeService = employeeService;
        _educationService = educationService;
        _universityService = universityService;
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        var entities = _service.GetAccount();

        if (!entities.Any())
        {
            return NotFound(new ResponseHandler<NewAccountDto>
            {
                Code = StatusCodes.Status404NotFound,
                Status = HttpStatusCode.NotFound.ToString(),
                Message = "Data not found"
            });
        }

        return Ok(new ResponseHandler<IEnumerable<NewAccountDto>>
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
        var account = _service.GetAccount(guid);
        if (account is null)
        {
            return NotFound(new ResponseHandler<NewAccountDto>
            {
                Code = StatusCodes.Status404NotFound,
                Status = HttpStatusCode.NotFound.ToString(),
                Message = "Data not found"
            });
        }

        return Ok(new ResponseHandler<NewAccountDto>
        {
            Code = StatusCodes.Status200OK,
            Status = HttpStatusCode.OK.ToString(),
            Message = "Data found",
            Data = account
        });
    }

    [HttpPost]
    public IActionResult Create(NewAccountDto newAccountDto)
    {
        var createdAccount = _service.CreateAccount(newAccountDto);
        if (createdAccount is null)
        {
            return BadRequest(new ResponseHandler<NewAccountDto>
            {
                Code = StatusCodes.Status400BadRequest,
                Status = HttpStatusCode.BadRequest.ToString(),
                Message = "Data not created"
            });
        }

        return Ok(new ResponseHandler<NewAccountDto>
        {
            Code = StatusCodes.Status200OK,
            Status = HttpStatusCode.OK.ToString(),
            Message = "Successfully created",
            Data = createdAccount
        });
    }

    [HttpPut]
    public IActionResult Update(NewAccountDto updateAccountDto)
    {
        var update = _service.UpdateAccount(updateAccountDto);
        if (update is -1)
        {
            return NotFound(new ResponseHandler<NewAccountDto>
            {
                Code = StatusCodes.Status404NotFound,
                Status = HttpStatusCode.NotFound.ToString(),
                Message = "Id not found"
            });
        }
        if (update is 0)
        {
            return BadRequest(new ResponseHandler<NewAccountDto>
            {
                Code = StatusCodes.Status500InternalServerError,
                Status = HttpStatusCode.InternalServerError.ToString(),
                Message = "Check your data"
            });
        }
        return Ok(new ResponseHandler<NewAccountDto>
        {
            Code = StatusCodes.Status200OK,
            Status = HttpStatusCode.OK.ToString(),
            Message = "Successfully updated"
        });
    }

    [HttpDelete]
    public IActionResult Delete(Guid guid)
    {
        var delete = _service.DeleteAccount(guid);

        if (delete is -1)
        {
            return NotFound(new ResponseHandler<NewAccountDto>
            {
                Code = StatusCodes.Status404NotFound,
                Status = HttpStatusCode.NotFound.ToString(),
                Message = "Id not found"
            });
        }
        if (delete is 0)
        {
            return BadRequest(new ResponseHandler<NewAccountDto>
            {
                Code = StatusCodes.Status500InternalServerError,
                Status = HttpStatusCode.InternalServerError.ToString(),
                Message = "Check connection to database"
            });
        }

        return Ok(new ResponseHandler<NewAccountDto>
        {
            Code = StatusCodes.Status200OK,
            Status = HttpStatusCode.OK.ToString(),
            Message = "Successfully deleted"
        });
    }

    [Route("register")]
    [HttpPost]
    public IActionResult Register(RegisterAccountDto registerAccountDto)
    {
        var createdRegisterAccount = _service.Register(registerAccountDto);
        if (createdRegisterAccount is null)
        {
            return BadRequest(new ResponseHandler<RegisterAccountDto>
            {
                Code = StatusCodes.Status400BadRequest,
                Status = HttpStatusCode.BadRequest.ToString(),
                Message = "Data not created"
            });
        }

        return Ok(new ResponseHandler<RegisterAccountDto>
        {
            Code = StatusCodes.Status200OK,
            Status = HttpStatusCode.OK.ToString(),
            Message = "Successfully created",
            Data = createdRegisterAccount
        });
    }

    [Route("login")]
    [HttpPost]
    public IActionResult Login(LoginAccountDto loginAccountDto)
    {
        LoginAccountDto loginSuccess = new LoginAccountDto();
        try
        {
            loginSuccess = _service.Login(loginAccountDto);

        }
        catch (Exception ex)
        {
            if (ex.Message.ToLower().Contains("not found"))
            {
                return NotFound(new ResponseHandler<LoginAccountDto>
                {
                    Code = StatusCodes.Status404NotFound,
                    Status = HttpStatusCode.BadRequest.ToString(),
                    Message = ex.Message
                });
            }
            else
            {
                return BadRequest(new ResponseHandler<LoginAccountDto>
                {
                    Code = StatusCodes.Status400BadRequest,
                    Status = HttpStatusCode.BadRequest.ToString(),
                    Message = ex.Message
                });
            }
        }

        return Ok(new ResponseHandler<LoginAccountDto>
        {
            Code = StatusCodes.Status200OK,
            Status = HttpStatusCode.OK.ToString(),
            Message = "Successfully login",
            Data = loginSuccess
        });   
    }

    [HttpPost("forget-password")]
    public IActionResult ForgetPassword(ForgetPasswordDto forgetPasswordDto)
    {
        // Get Employee By Email
        var getEmployee = _employeeService.GetByEmail(forgetPasswordDto.Email)!;
        if (getEmployee is null)
        {
            return NotFound(new ResponseHandler<ForgetPasswordDto>
            {
                Code = StatusCodes.Status400BadRequest,
                Status = HttpStatusCode.NotFound.ToString(),
                Message = "No Account Found"
            });
        }

        // Generate OTP
        int generatedOtp = _service.GenerateOtp();

        // Get Account By Employee.Guid
        var getAccount = _service.GetAccount(getEmployee.Guid);

        // Update Otp, Expired Time, isUsed in Account
        var otpExpiredTime = DateTime.Now.AddMinutes(5);
        var updateAccountDto = new NewAccountDto
        {
            Guid = getAccount!.Guid,
            Password = getAccount.Password,
            IsDeleted = (bool)getAccount.IsDeleted,
            Otp = generatedOtp,
            IsUsed = false,
            ExpiredTime = otpExpiredTime
        };

        var updateResult = _service.UpdateAccount(updateAccountDto);
        if (updateResult == 0)
        {
            return NotFound(new ResponseHandler<ForgetPasswordDto>
            {
                Code = StatusCodes.Status500InternalServerError,
                Status = HttpStatusCode.InternalServerError.ToString(),
                Message = "Failed to Setting OTP in Related Account"
            });
        }

        // Success to Create OTP and Update the Account Model
        return Ok(new ResponseHandler<IEnumerable<OtpResponseDto>>
        {
            Code = StatusCodes.Status200OK,
            Status = HttpStatusCode.OK.ToString(),
            Message = "OTP Sent",
            Data = new List<OtpResponseDto> { new OtpResponseDto {
                    Guid = getAccount.Guid,
                    Email = getEmployee.Email,
                    Otp = generatedOtp,
                    ExpiredTime = otpExpiredTime
                } }
        });
    }

}