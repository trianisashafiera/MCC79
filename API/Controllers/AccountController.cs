﻿using API.Contracts;
using API.Models;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

    [ApiController]
    [Route("api/accounts")]
public class AccountController : GeneralController<Account>
{
    public AccountController(IAccountRepository repository) : base(repository) { }
}