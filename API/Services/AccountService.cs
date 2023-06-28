using API.Contracts;
using API.DTOs.Accounts;
using API.Models;
using API.Utilities.Enums;

namespace API.Services;

public class AccountService
{
        private readonly IAccountRepository _accountRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IEducationRepository _educationRepository;
        private readonly IUniversityRepository _universityRepository;

    public AccountService(IAccountRepository accountRepository,
                          IEmployeeRepository employeeRepository,
                          IEducationRepository educationRepository,
                          IUniversityRepository universityRepository)
    {
        _accountRepository = accountRepository;
        _employeeRepository = employeeRepository;
        _educationRepository = educationRepository;
        _universityRepository = universityRepository;
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

    public RegisterAccountDto? Register(RegisterAccountDto registerAccountDto)
    {
        EmployeeService employeService = new EmployeeService(_employeeRepository);
        Employee employee = new Employee
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

        var createdEmployee = _employeeRepository.Create(employee);
        if (createdEmployee is null)
        {
            return null;
        }
        University university = new University
        {
            Guid = new Guid(),
            Code = registerAccountDto.UniversityCode,
            Name = registerAccountDto.UniversityName
        };

        var createdUniversity = _universityRepository.Create(university);
        if (createdUniversity is null)
        {
            return null;
        }

        Education education = new Education
        {
            Guid = employee.Guid,
            Major = registerAccountDto.Major,
            Degree = registerAccountDto.Degree,
            Gpa = registerAccountDto.Gpa,
            UniversityGuid = university.Guid
        };

        var createdEducation = _educationRepository.Create(education);
        if (createdEducation is null)
        {
            return null;
        }

        Account account = new Account
        {
            Guid = employee.Guid,
            Password = Hashing.HashPassword(registerAccountDto.Password)

        };

        var createdAccount = _accountRepository.Create(account);
        if (createdAccount is null)
        {
            return null;
        }

        var toDto = new RegisterAccountDto
        {
            FirstName = createdEmployee.FirstName,
            LastName = createdEmployee.LastName,
            BirthDate = createdEmployee.BirthDate,
            Gender = createdEmployee.Gender,
            HiringDate = createdEmployee.HiringDate,
            Email = createdEmployee.Email,
            PhoneNumber = createdEmployee.PhoneNumber,
            Password = createdAccount.Password,
            Major = createdEducation.Major,
            Degree = createdEducation.Degree,
            Gpa = createdEducation.Gpa,
            UniversityCode = createdUniversity.Code,
            UniversityName = createdUniversity.Name
        };

        return toDto;
    }
}
    


