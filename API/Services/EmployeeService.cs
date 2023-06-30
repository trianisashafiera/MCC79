using API.Contracts;
using API.DTOs.Accounts;
using API.DTOs.Employees;
using API.Models;

namespace API.Services;

    public class EmployeeService
    {
    private readonly IEmployeeRepository _employeeRepository;
    public EmployeeService(IEmployeeRepository employeeRepository)
    {
        _employeeRepository = employeeRepository;
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
            Nik = GenerateNik(),
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
}

