using API.DTOs.Educations;

namespace API.Contracts
{
    public interface IEducationService
    {
        IEnumerable<NewEducationDto> GetEducation();
    }
}
