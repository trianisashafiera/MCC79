using API.Models;

namespace API.Contracts;
public interface IEmployeeRepository : IGeneralRepository<Employee>
{
    public Employee? GetAccountByEmail(string email);
  
    public Employee? GetByEmailAndPhoneNumber(string data);
    public Employee? CheckEmail(string email);
    string? GetLastEmployeeNik();
}