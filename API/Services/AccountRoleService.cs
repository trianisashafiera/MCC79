using API.Contracts;
using API.DTOs.AccountRoles;
using API.Models;

namespace API.Services;
    public class AccountRoleService
    {
    private readonly IAccountRoleRepository _accountRoleRepository;
    public AccountRoleService(IAccountRoleRepository accountRoleRepository)
    {
        _accountRoleRepository = accountRoleRepository;
    }

    public IEnumerable<NewAccountRoleDto> GetAccountRole()
    {
        var accountRoles = _accountRoleRepository.GetAll();
        if (!accountRoles.Any())
        {
            return null;
        }
        var toDto = accountRoles.Select(accountRole =>
                                           new NewAccountRoleDto
                                           {
                                               Guid = accountRole.Guid,
                                               AccountGuid = accountRole.AccountGuid,
                                               RoleGuid = accountRole.RoleGuid
                                           }).ToList();

        return toDto;

    }
    public NewAccountRoleDto? GetAccountRole(Guid guid)
    {
        var accountRole = _accountRoleRepository.GetByGuid(guid);
        if (accountRole is null)
        {
            return null; // Booking not found
        }

        var toDto = new NewAccountRoleDto
        {
            Guid = accountRole.Guid,
            AccountGuid = accountRole.AccountGuid,
            RoleGuid = accountRole.RoleGuid
        };

        return toDto; // Booking found
    }

    public NewAccountRoleDto? CreateAccountRole(NewAccountRoleDto newAccountRoleDto)
    {
        var accountRole = new AccountRole
        {
            Guid = new Guid(),
            AccountGuid = newAccountRoleDto.AccountGuid,
            RoleGuid = newAccountRoleDto.RoleGuid,
            CreatedDate = DateTime.Now,
            ModifiedDate = DateTime.Now
        };

        var createdAccountRole = _accountRoleRepository.Create(accountRole);
        if (createdAccountRole is null)
        {
            return null; // Booking not created
        }

        var toDto = new NewAccountRoleDto
        {
            Guid = createdAccountRole.Guid,
            AccountGuid = createdAccountRole.AccountGuid,
            RoleGuid = createdAccountRole.RoleGuid
        };

        return toDto; // Booking created
    }

    public int UpdateAccountRole(NewAccountRoleDto updateAccountRoleDto)
    {
        var isExist = _accountRoleRepository.IsExist(updateAccountRoleDto.Guid);
        if (!isExist)
        {
            // Booking not found
            return -1;
        }

        var getAccountRole = _accountRoleRepository.GetByGuid(updateAccountRoleDto.Guid);

        var accountRole = new AccountRole
        {
            Guid = updateAccountRoleDto.Guid,
            AccountGuid = updateAccountRoleDto.AccountGuid,
            RoleGuid = updateAccountRoleDto.RoleGuid,
            ModifiedDate = DateTime.Now,
            CreatedDate = getAccountRole!.CreatedDate
        };

        var isUpdate = _accountRoleRepository.Update(accountRole);
        if (!isUpdate)
        {
            return 0; // Booking not updated
        }

        return 1;
    }

    public int DeleteAccountRole(Guid guid)
    {
        var isExist = _accountRoleRepository.IsExist(guid);
        if (!isExist)
        {
            return -1; // Booking not found
        }

        var accountRole = _accountRoleRepository.GetByGuid(guid);
        var isDelete = _accountRoleRepository.Delete(accountRole!);
        if (!isDelete)
        {
            return 0; // Booking not deleted
        }

        return 1;
    }

   
    }
    

