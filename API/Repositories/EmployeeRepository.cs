using API.Contracts;
using API.Data;
using API.Models;

namespace API.Repositories;
public class EmployeeRepository : GeneralRepository<Employee>, IEmployeeRepository
{
    private readonly BookingDbContext _context;
    public EmployeeRepository(BookingDbContext context) : base(context) 
    {
        _context = context;
    }

    //Login
    public Employee? GetAccountByEmail(string email)
    {
        return _context.Set<Employee>().SingleOrDefault(e => e.Email == email);
    }

    public Employee? GetByEmailAndPhoneNumber(string data)
    {
        return _context.Set<Employee>().FirstOrDefault(e => e.PhoneNumber == data || e.Email == data);
    }
    public Employee? CheckEmail(string email)
    {
        return _context.Set<Employee>().FirstOrDefault(e => e.Email == email);
    }
    public string? GetLastEmployeeNik()
    {
        return _context.Set<Employee>().ToList().Select(e => e.Nik).LastOrDefault();
    }
   
}
