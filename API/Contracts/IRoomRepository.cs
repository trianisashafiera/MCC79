using API.DTOs.Rooms;
using API.Models;

namespace API.Contracts;
public interface IRoomRepository : IGeneralRepository<Room>
{
    ICollection<NewRoomDto> GetByDateNow();
}
