using API.Contracts;
using API.Data;
using API.Models;

namespace API.Repositories;
public class EmployeeRepository : GeneralRepository<Employee>, IEmployeeRepository
{
    private readonly BookingDbContext _context;
    public EmployeeRepository(BookingDbContext context) : base(context) { }

    //Login
    public Employee? GetAccountByEmail(string email)
    {
        /*return _context.Set<Employee>().Where(e => e.Email.Single(email));*/
        return _context.Employees.SingleOrDefault(e => e.Email == email);
    }

    //OTP
    public IEnumerable<Employee> GetByEmail(string email)
    {
        return _context.Set<Employee>().Where(u => u.Email == email);
    }
    public IEnumerable<Employee> GetByName(string name)
    {
        return _context.Set<Employee>().Where(u => u.FirstName.Contains(name));
    }
    //change password
    public Employee? GetByEmailAndPhoneNumber(string data)
    {
        return _context.Set<Employee>().FirstOrDefault(e => e.PhoneNumber == data || e.Email == data);
    }
    public Employee? CheckEmail(string email)
    {
        return _context.Set<Employee>().FirstOrDefault(e => e.Email == email);
    }
}
