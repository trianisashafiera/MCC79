using API.Models;

namespace API.Contracts;
public interface IRoleRepository : IGeneralRepository<Role>
{
    /* Role? GetRoleByEmail(string email);*/
    Role? GetByName(string name);
}