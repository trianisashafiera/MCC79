using API.Contracts;
using API.DTOs.Universities;

namespace API.Services;
    public class UniversityService
    {
        private readonly IUniversityRepository _universityRepository;
        public UniversityService(IUniversityRepository universityRepository)
        {
            _universityRepository = universityRepository;
        }

        public IEnumerable<GetUniversityDto> GetUniversity()
    {
        var universities = _universityRepository.GetAll();
        if (!universities.Any())
        {
            return null;
        }
        var toDto = universities.Select(university => new GetUniversityDto
                     { Guid = university.Guid,
                        Code = university.Code,
                        Name = university.Name}).ToList();
        return toDto;
    }


    }

