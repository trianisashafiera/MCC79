using API.Contracts;
using API.Data;
using API.Models;

namespace API.Repositories;
    public class UniversityRepository : GeneralRepository<University>, IUniversityRepository
    {
        public UniversityRepository(BookingDbContext context) : base(context) { }
        public IEnumerable<University> GetByName (string name)
    {
        return _context.Set<University>().Where(u => u.Name.Contains(name));
    }
    public University? GetByCodeAndName(string code, string name)
    {
        return _context.Set<University>().FirstOrDefault(u => u.Name.ToLower() == name.ToLower() && u.Code.ToLower() == code.ToLower());
    }
}
        
    
