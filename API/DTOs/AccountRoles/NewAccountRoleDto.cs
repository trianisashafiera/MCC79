using API.Models;

namespace API.DTOs.AccountRoles;

    public class NewAccountRoleDto
    {
        public Guid Guid { get; set; }
        public Guid AccountGuid { get; set; }
        public Guid RoleGuid { get; set; }

    public static implicit operator AccountRole(NewAccountRoleDto accountRoleDto)
    {
        return new()
        {
            Guid = accountRoleDto.Guid,
            AccountGuid = accountRoleDto.AccountGuid,
            RoleGuid = accountRoleDto.RoleGuid
        };
    }

    public static explicit operator NewAccountRoleDto(AccountRole accountRoleDto)
    {
        return new()
        {
            Guid = accountRoleDto.Guid,
            AccountGuid = accountRoleDto.AccountGuid,
            RoleGuid = accountRoleDto.RoleGuid
        };
    }
}

