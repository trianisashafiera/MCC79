using API.Contracts;
using API.DTOs.Auth;
using API.Models;
using API.Utilities.Enums;

namespace API.Services;

    public class AuthService
    {
    private readonly IAccountRepository _accountRepository;
    private readonly IEmployeeRepository _employeeRepository;
 

    public AuthService(IAccountRepository accountRepository, IEmployeeRepository employeeRepository)
    {
        _accountRepository = accountRepository;
        _employeeRepository = employeeRepository;
      
    }

   
    public int ChangePassword(ChangePasswordDto changePasswordDto)
    {
        var isExist = _employeeRepository.CheckEmail(changePasswordDto.Email);
        if (isExist is null)
        {
            return -1; // Account not found
        }

        var getAccount = _accountRepository.GetByGuid(isExist.Guid);
        if (getAccount.Otp != changePasswordDto.Otp)
        {
            return 0;
        }

        if (getAccount.IsUsed == true)
        {
            return 1;
        }

        if (getAccount.ExpiredTime < DateTime.Now)
        {
            return 2;
        }

        var account = new Account
        {
            Guid = getAccount.Guid,
            IsUsed = getAccount.IsUsed,
            IsDeleted = getAccount.IsDeleted,
            ModifiedDate = DateTime.Now,
            CreatedDate = getAccount!.CreatedDate,
            Otp = getAccount.Otp,
            ExpiredTime = getAccount.ExpiredTime,
            Password = Hashing.HashPassword(changePasswordDto.NewPassword),
        };

        var isUpdate = _accountRepository.Update(account);
        if (!isUpdate)
        {
            return 0; // Account not updated
        }

        return 3;
    }
}

