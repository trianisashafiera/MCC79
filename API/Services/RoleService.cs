using API.Contracts;
using API.DTOs.Roles;
using API.Models;

namespace API.Services;
public class RoleService
{
    private readonly IRoleRepository _roleRepository;
    public RoleService(IRoleRepository roleRepository)
    {
        _roleRepository = roleRepository;
    }

    public IEnumerable<NewRoleDto> GetRole()
    {
        var roles = _roleRepository.GetAll();
        if (!roles.Any())
        {
            return null;
        }
        var toDto = roles.Select(role =>
                                           new NewRoleDto
                                           {
                                               Guid = role.Guid,
                                               Name = role.Name
                                           }).ToList();

        return toDto;

    }
    public NewRoleDto? GetRole(Guid guid)
    {
        var role = _roleRepository.GetByGuid(guid);
        if (role is null)
        {
            return null; // Booking not found
        }

        var toDto = new NewRoleDto
        {
            Guid = role.Guid,
            Name = role.Name
        };

        return toDto; // Booking found
    }

    public NewRoleDto? CreateRole(NewRoleDto newRoleDto)
    {
        var role = new Role
        {
            Guid = new Guid(),
            Name = newRoleDto.Name,
            CreatedDate = DateTime.Now,
            ModifiedDate = DateTime.Now
        };

        var createdRole = _roleRepository.Create(role);
        if (createdRole is null)
        {
            return null; // Booking not created
        }

        var toDto = new NewRoleDto
        {
            Guid = createdRole.Guid,
            Name = createdRole.Name
        };

        return toDto; // Booking created
    }

    public int UpdateRole(NewRoleDto updatRoleDto)
    {
        var isExist = _roleRepository.IsExist(updatRoleDto.Guid);
        if (!isExist)
        {
            // Booking not found
            return -1;
        }

        var getRole = _roleRepository.GetByGuid(updatRoleDto.Guid);

        var role = new Role
        {
            Guid = updatRoleDto.Guid,
            Name = updatRoleDto.Name,
            ModifiedDate = DateTime.Now,
            CreatedDate = getRole!.CreatedDate
        };

        var isUpdate = _roleRepository.Update(role);
        if (!isUpdate)
        {
            return 0; // Booking not updated
        }

        return 1;
    }

    public int DeleteRole(Guid guid)
    {
        var isExist = _roleRepository.IsExist(guid);
        if (!isExist)
        {
            return -1; // Booking not found
        }

        var role = _roleRepository.GetByGuid(guid);
        var isDelete = _roleRepository.Delete(role!);
        if (!isDelete)
        {
            return 0; // Booking not deleted
        }

        return 1;
    }

}

