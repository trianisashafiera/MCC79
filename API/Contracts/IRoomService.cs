using API.DTOs.Rooms;

namespace API.Contracts
{
    public interface IRoomService
    {
        IEnumerable<NewRoomDto> GetRoom();
    }
}
