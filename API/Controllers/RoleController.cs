using API.Contracts;
using API.Models;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

    [ApiController]
    [Route("api/role")]
    public class RoleController : GeneralController<Role>
    {
        public RoleController(IRoleRepository repository) : base(repository) { }
    }
