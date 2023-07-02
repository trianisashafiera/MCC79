using API.Contracts;
using API.DTOs.Accounts;
using API.DTOs.Employees;
using API.Models;
using API.Repositories;
using API.Utilities.Handlers;

namespace API.Services;

    public class EmployeeService
    {
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IEducationRepository _educationRepository;
    private readonly IUniversityRepository _universityRepository;
    private readonly IAccountRepository _accountRepository;
    private readonly IAccountRoleRepository _accountRoleRepository;
    private readonly IRoleRepository _roleRepository;
    public EmployeeService(IEmployeeRepository employeeRepository,
                           IEducationRepository educationRepository,
                           IUniversityRepository universityRepository,
                           IAccountRepository accountRepository,
                           IAccountRoleRepository accountRoleRepository,
                           IRoleRepository roleRepository)
    {
        _employeeRepository = employeeRepository;
        _educationRepository = educationRepository;
        _universityRepository = universityRepository;
        _accountRepository = accountRepository;
        _accountRoleRepository = accountRoleRepository;
        _roleRepository = roleRepository;
    }

    public IEnumerable<NewEmployeeDto> GetEmployee()
    {
        var employees = _employeeRepository.GetAll();
        if (!employees.Any())
        {
            return null;
        }
        var toDto = employees.Select(employee =>
                                           new NewEmployeeDto
                                           {
                                               Guid = employee.Guid,
                                               Nik = employee.Nik,
                                               FirstName = employee.FirstName,
                                               LastName = employee.LastName,
                                               BirthDate = employee.BirthDate,
                                               Gender = employee.Gender,
                                               HiringDate = employee.HiringDate,
                                               Email = employee.Email,
                                               PhoneNumber = employee.PhoneNumber
                                           }).ToList();

        return toDto;

    }
    public NewEmployeeDto? GetEmployee(Guid guid)
    {
        var employee = _employeeRepository.GetByGuid(guid);
        if (employee is null)
        {
            return null; // Employee not found
        }

        var toDto = new NewEmployeeDto
        {
            Guid = employee.Guid,
            Nik = employee.Nik,
            FirstName = employee.FirstName,
            LastName = employee.LastName,
            BirthDate = employee.BirthDate,
            Gender = employee.Gender,
            HiringDate = employee.HiringDate,
            Email = employee.Email,
            PhoneNumber = employee.PhoneNumber
        };

        return toDto; // Employee found
    }

    public NewEmployeeDto? CreateEmployee(NewEmployeeDto newEmployeeDto)
    {
        var employee = new Employee
        {
            Guid = new Guid(),
            FirstName = newEmployeeDto.FirstName,
            LastName = newEmployeeDto.LastName,
            BirthDate = newEmployeeDto.BirthDate,
            Gender = newEmployeeDto.Gender,
            HiringDate = newEmployeeDto.HiringDate,
            Email = newEmployeeDto.Email,
            PhoneNumber = newEmployeeDto.PhoneNumber,
            CreatedDate = DateTime.Now,
            ModifiedDate = DateTime.Now
        };
        employee.Nik = GenerateHandler.Nik(_employeeRepository.GetLastEmployeeNik());
        var createdEmployee = _employeeRepository.Create(employee);
        if (createdEmployee is null)
        {
            return null; // Employee not created
        }

        var toDto = new NewEmployeeDto
        {
            Guid = createdEmployee.Guid,
            Nik = createdEmployee.Nik,
            FirstName = createdEmployee.FirstName,
            LastName = createdEmployee.LastName,
            BirthDate = createdEmployee.BirthDate,
            Gender = createdEmployee.Gender,
            HiringDate = createdEmployee.HiringDate,
            Email = createdEmployee.Email,
            PhoneNumber = createdEmployee.PhoneNumber
        };

        return toDto; // Employee created
    }

    public int UpdateEmployee(NewEmployeeDto updateEmployeeDto)
    {
        var isExist = _employeeRepository.IsExist(updateEmployeeDto.Guid);
        if (!isExist)
        {
            // Employee not found
            return -1;
        }

        var getEmployee = _employeeRepository.GetByGuid(updateEmployeeDto.Guid);

        var employee = new Employee
        {
            Guid = updateEmployeeDto.Guid,
            Nik = updateEmployeeDto.Nik,
            FirstName = updateEmployeeDto.FirstName,
            LastName = updateEmployeeDto.LastName,
            BirthDate = updateEmployeeDto.BirthDate,
            Gender = updateEmployeeDto.Gender,
            HiringDate = updateEmployeeDto.HiringDate,
            Email = updateEmployeeDto.Email,
            PhoneNumber = updateEmployeeDto.PhoneNumber,
            ModifiedDate = DateTime.Now,
            CreatedDate = getEmployee!.CreatedDate
        };

        var isUpdate = _employeeRepository.Update(employee);
        if (!isUpdate)
        {
            return 0; // Employee not updated
        }

        return 1;
    }

    public int DeleteEmployee(Guid guid)
    {
        var isExist = _employeeRepository.IsExist(guid);
        if (!isExist)
        {
            return -1; // Employee not found
        }

        var employee = _employeeRepository.GetByGuid(guid);
        var isDelete = _employeeRepository.Delete(employee!);
        if (!isDelete)
        {
            return 0; // Employee not deleted
        }

        return 1;
    }

    public string GenerateNik()
    {
        var employees = _employeeRepository.GetAll();
        if (!employees.Any())
        {
            // Jika data employee kosong
            return "1111";
        }
        var lastEmployee = employees.Last();

        int lastNik = int.Parse(lastEmployee.Nik);
        int newNik = lastNik + 1;

        string nik = newNik.ToString();

        return nik;
    }
    public OtpResponseDto? GetByEmail(string email)
    {
        var account = _employeeRepository.GetAll()
            .FirstOrDefault(e => e.Email.Contains(email));

        if (account != null)
        {
            return new OtpResponseDto
            {
                Email = account.Email,
                Guid = account.Guid
            };
        }

        return null;
    }

    public IEnumerable<GetAllMasterDto>? GetMaster()
    {
        var master = (from e in _employeeRepository.GetAll()
                      join education in _educationRepository.GetAll() on e.Guid equals education.Guid
                      join u in _universityRepository.GetAll() on education.UniversityGuid equals u.Guid
                      join a in _accountRepository.GetAll() on e.Guid equals a.Guid
                      join ar in _accountRoleRepository.GetAll() on a.Guid equals ar.AccountGuid
                      join r in _roleRepository.GetAll() on ar.RoleGuid equals r.Guid


                      select new GetAllMasterDto
                      {
                          Guid = e.Guid,
                          FullName = e.FirstName + " " + e.LastName,
                          Nik = e.Nik,
                          BirthDate = e.BirthDate,
                          Email = e.Email,
                          HiringDate = e.HiringDate,
                          PhoneNumber = e.PhoneNumber,
                          Major = education.Major,
                          Degree = education.Degree,
                          Gpa = education.Gpa,
                          UniversityName = u.Name,
                          Role = r.Name,
                      }).ToList();

        if (!master.Any())
        {
            return null;
        }
        return master;
    }

    public GetAllMasterDto? GetMasterByGuid(Guid guid)
    {
        var master = GetMaster();

        var masterByGuid = master.FirstOrDefault(master => master.Guid == guid);

        return masterByGuid;
    }
}

