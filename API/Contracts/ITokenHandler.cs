using System.Security.Claims;
using System.Collections.Generic;

namespace API.Contracts;
public interface ITokenHandler
{
    string GenerateToken(List<Claim> claims);
}

