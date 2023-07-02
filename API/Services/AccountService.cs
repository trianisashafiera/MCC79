using API.Contracts;
using API.Data;
using API.DTOs.Accounts;
using API.DTOs.AccountRoles;
using API.DTOs.Employees;
using API.Models;
using API.Repositories;
using API.Utilities.Enums;
using API.Utilities.Handlers;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace API.Services;

public class AccountService
{
        private readonly IAccountRepository _accountRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IEducationRepository _educationRepository;
        private readonly IUniversityRepository _universityRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IAccountRoleRepository _accountRoleRepository;
        private readonly ITokenHandler _tokenHandler;
        private readonly IEmailHandler _emailHandler;
        private readonly BookingDbContext _bookingdBContext;

    public AccountService(IAccountRepository accountRepository,
                          IEmployeeRepository employeeRepository,
                          IEducationRepository educationRepository,
                          IUniversityRepository universityRepository,
                          IRoleRepository roleRepository,
                          IAccountRoleRepository accountRoleRepository,
                          ITokenHandler tokenHandler,
                          IEmailHandler emailHandler,
                          BookingDbContext bookingDbContext)
    {
        _accountRepository = accountRepository;
        _employeeRepository = employeeRepository;
        _educationRepository = educationRepository;
        _universityRepository = universityRepository;
        _roleRepository = roleRepository;
        _accountRoleRepository = accountRoleRepository;
        _tokenHandler = tokenHandler;
        _emailHandler = emailHandler;
        _bookingdBContext = bookingDbContext;
    }

        public IEnumerable<NewAccountDto> GetAccount()
        {
            var accounts = _accountRepository.GetAll();
            if (!accounts.Any())
            {
                return null;
            }
            var toDto = accounts.Select(account =>
                                               new NewAccountDto
                                               {
                                                   Guid = account.Guid,
                                                   Password = account.Password,
                                                   Otp = account.Otp,
                                                   ExpiredTime = account.ExpiredTime,
                                                   IsDeleted = account.IsDeleted,
                                                   IsUsed = account.IsUsed
                                               }).ToList();

            return toDto;
        }
        public NewAccountDto? GetAccount(Guid guid)
        {
            var account = _accountRepository.GetByGuid(guid);
            if (account is null)
            {
                return null; // Account not found
            }

            var toDto = new NewAccountDto
            {
                Guid = account.Guid,
                Password = account.Password,
                Otp = account.Otp,
                ExpiredTime = account.ExpiredTime,
                IsDeleted = account.IsDeleted,
                IsUsed = account.IsUsed
            };

            return toDto; // Account found
    }

        public NewAccountDto? CreateAccount(NewAccountDto newAccountDto)
        {
            var account = new Account
            {
                Guid = newAccountDto.Guid,
                Password = Hashing.HashPassword(newAccountDto.Password),
                Otp= newAccountDto.Otp,
                ExpiredTime= newAccountDto.ExpiredTime,
                IsDeleted = newAccountDto.IsDeleted,
                IsUsed = newAccountDto.IsUsed,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now
            };

            var createdAccount = _accountRepository.Create(account);
            if (createdAccount is null)
            {
                return null; // Account not created
        }

            var toDto = new NewAccountDto
            {
                Guid = createdAccount.Guid,
                Password = createdAccount.Password,
                Otp = createdAccount.Otp,
                ExpiredTime = createdAccount.ExpiredTime,
                IsDeleted = createdAccount.IsDeleted,
                IsUsed = createdAccount.IsUsed
            };

            return toDto; // Account created
        }

        public int UpdateAccount(NewAccountDto updateAccountDto)
        {
            var isExist = _accountRepository.IsExist(updateAccountDto.Guid);
            if (!isExist)
            {
            // Account not found
            return -1;
            }

            var getAccount = _accountRepository.GetByGuid(updateAccountDto.Guid);

            var account = new Account
            {
                Guid = updateAccountDto.Guid,
                Password = Hashing.HashPassword(updateAccountDto.Password),
                Otp = updateAccountDto.Otp,
                ExpiredTime = updateAccountDto.ExpiredTime,
                IsUsed = updateAccountDto.IsUsed,
                IsDeleted = updateAccountDto.IsDeleted,
                ModifiedDate = DateTime.Now,
                CreatedDate = getAccount!.CreatedDate
            };

            var isUpdate = _accountRepository.Update(account);
            if (!isUpdate)
            {
                return 0; // Account not updated
        }

            return 1;
        }

        public int DeleteAccount(Guid guid)
        {
            var isExist = _accountRepository.IsExist(guid);
            if (!isExist)
            {
                return -1; // Account not found
        }

            var account = _accountRepository.GetByGuid(guid);
            var isDelete = _accountRepository.Delete(account!);
            if (!isDelete)
            {
                return 0; // Account not deleted
        }

            return 1;
        }

    public GetRegisterAccountDto? Register(RegisterAccountDto registerAccountDto)
    {
    /*    EmployeeService employeService = new EmployeeService(_employeeRepository);*/
        using var transaction = _bookingdBContext.Database.BeginTransaction();
        try
        {
            var employee = new Employee
            {
                Guid = new Guid(),
                FirstName = registerAccountDto.FirstName,
                LastName = registerAccountDto.LastName,
                BirthDate = registerAccountDto.BirthDate,
                Gender = registerAccountDto.Gender,
                HiringDate = registerAccountDto.HiringDate,
                Email = registerAccountDto.Email,
                PhoneNumber = registerAccountDto.PhoneNumber,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now
            };
            employee.Nik = GenerateHandler.Nik(_employeeRepository.GetLastEmployeeNik());
            var createdEmployee = _employeeRepository.Create(employee);
            if (createdEmployee is null)
            {
                return null;
            }

            var universityEntity = _universityRepository.GetByCodeAndName(registerAccountDto.UniversityCode, registerAccountDto.UniversityName);
            var university = new University
            {
             Code = registerAccountDto.UniversityCode,
             Name = registerAccountDto.UniversityName,
             Guid = new Guid(),
             CreatedDate = DateTime.Now,
             ModifiedDate = DateTime.Now,
            };
             var createdUniversity = _universityRepository.Create(university);
             if (createdUniversity is null)
             {
                 return null;
             }

             var education = new Education
             {
                    Guid = employee.Guid,
                    Major = registerAccountDto.Major,
                    Degree = registerAccountDto.Degree,
                    Gpa = registerAccountDto.Gpa,
                    UniversityGuid = university.Guid,
                    CreatedDate = DateTime.Now,
                    ModifiedDate = DateTime.Now,
             };

             var createdEducation = _educationRepository.Create(education);
             if (createdEducation is null)
             {
                    return null;
             }

             var account = new Account
             {
                    Guid = employee.Guid,
                    Password = Hashing.HashPassword(registerAccountDto.Password),
                    IsDeleted = false,
                    IsUsed = false,
                    Otp = 0,
                    CreatedDate = DateTime.Now,
                    ModifiedDate = DateTime.Now,
                    ExpiredTime = DateTime.Now.AddYears(5),
             };
             var createdAccount = _accountRepository.Create(account);
             if (createdAccount is null)
             {
                return null;
             }
            var getRoleUser = _roleRepository.GetByName("User");
            _accountRoleRepository.Create(new NewAccountRoleDto
            {
                AccountGuid = account.Guid,
                RoleGuid = getRoleUser.Guid
            });

            var toDto = new GetRegisterAccountDto
             {
                    Guid = employee.Guid,
                    Email = employee.Email,
             };

              transaction.Commit();
              return toDto;
            }
        catch
        {
            transaction.Rollback();
            return null;
        }
    }

    public string Login(LoginAccountDto loginAccountDto)
    {
        var employee = _employeeRepository.GetAccountByEmail(loginAccountDto.Email);
        if (employee is null)
        {
            return "0";
        }

        var account = _accountRepository.GetByGuid(employee.Guid);

        if (account is null)
        { 
            return "0";
        }
       if (!Hashing.ValidatePassword(loginAccountDto.Password, account!.Password))
        {
            return "-1";
        }

        try
        {
            var claims = new List<Claim>() 
        {
            new Claim("NIK", employee.Nik),
            new Claim("FullName", $"{employee.FirstName} {employee.LastName}"),
            new Claim("Email", loginAccountDto.Email) 
        };
        var getAccountRole = _accountRoleRepository.GetAccountRolesByAccountGuid(employee.Guid);
        var getRoleNameByAccountRole = from ar in getAccountRole
                                       join r in _roleRepository.GetAll() on ar.RoleGuid equals r.Guid
                                       select r.Name;

        claims.AddRange(getRoleNameByAccountRole.Select(role => new Claim(ClaimTypes.Role, role)));

        var getToken = _tokenHandler.GenerateToken(claims);
        return getToken;
        }
        catch
        {
            return "-2";
        }

        /*var toDto = new LoginAccountDto
       {
           Email = loginAccountDto.Email,
           Password = loginAccountDto.Password,
       };

       return toDto;*/
    }
    public int ForgetPassword(ForgetPasswordDto forgotPassword)
    {
        var employee = _employeeRepository.GetAccountByEmail(forgotPassword.Email);
        if (employee is null)
        {
            return 0; // Email not found
        }
        var account = _accountRepository.GetByGuid(employee.Guid);
        if (account is null)
            return -1;

        var otp = new Random().Next(111111, 999999);
        var isUpdated = _accountRepository.Update(new Account
        {
            Guid = account.Guid,
            Password = account.Password,
            IsDeleted = account.IsDeleted,
            Otp = otp,
            ExpiredTime = DateTime.Now.AddMinutes(5),
            IsUsed = false,
            CreatedDate = account.CreatedDate,
            ModifiedDate = DateTime.Now
        });

        if (!isUpdated)
            return -1;

        _emailHandler.SendEmail(forgotPassword.Email,
                                "Forgot Password",
                                $"Your OTP is {otp}");

        return 1;
    }

    public int ChangePassword(ChangePasswordDto changePasswordDto)
    {
        var isExist = _employeeRepository.CheckEmail(changePasswordDto.Email);
        if (isExist is null)
        {
            return -1;
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

        var isUpdated = _accountRepository.Update(new Account
        {
            Guid = getAccount.Guid,
            IsUsed = true,
            IsDeleted = getAccount.IsDeleted,
            ModifiedDate = DateTime.Now,
            CreatedDate = getAccount!.CreatedDate,
            Otp = changePasswordDto.Otp,
            ExpiredTime = getAccount.ExpiredTime,
            Password = Hashing.HashPassword(changePasswordDto.NewPassword)
        });
        if (!isUpdated)
        {
            return 0;
        }
        return 3;
    }

}



