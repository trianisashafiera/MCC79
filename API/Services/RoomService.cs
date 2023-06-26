using API.Contracts;
using API.DTOs.Rooms;
using API.Models;

namespace API.Services;
public class RoomService
{
    private readonly IRoomRepository _roomRepository;
    public RoomService(IRoomRepository roomRepository)
    {
        _roomRepository = roomRepository;
    }

    public IEnumerable<NewRoomDto> GetRoom()
    {
        var rooms = _roomRepository.GetAll();
        if (!rooms.Any())
        {
            return null;
        }
        var toDto = rooms.Select(room =>
                                           new NewRoomDto
                                           {
                                               Guid = room.Guid,
                                               Name = room.Name,
                                               Floor = room.Floor,
                                               Capacity = room.Capacity
                                           }).ToList();

        return toDto;

    }
    public NewRoomDto? GetRoom(Guid guid)
    {
        var room = _roomRepository.GetByGuid(guid);
        if (room is null)
        {
            return null; // Booking not found
        }

        var toDto = new NewRoomDto
        {
            Guid = room.Guid,
            Name = room.Name,
            Floor = room.Floor,
            Capacity = room.Capacity
        };

        return toDto; // Booking found
    }

    public NewRoomDto? CreateRoom(NewRoomDto newRoomDto)
    {
        var room = new Room
        {
            Guid = new Guid(),
            Name = newRoomDto.Name,
            Floor = newRoomDto.Floor,
            Capacity = newRoomDto.Capacity,
            CreatedDate = DateTime.Now,
            ModifiedDate = DateTime.Now
        };

        var createdRoom = _roomRepository.Create(room);
        if (createdRoom is null)
        {
            return null; // Booking not created
        }

        var toDto = new NewRoomDto
        {
            Guid = createdRoom.Guid,
            Name = createdRoom.Name,
            Floor = createdRoom.Floor,
            Capacity = createdRoom.Capacity
        };

        return toDto; // Booking created
    }

    public int UpdateRoom(NewRoomDto updatRoomDto)
    {
        var isExist = _roomRepository.IsExist(updatRoomDto.Guid);
        if (!isExist)
        {
            // Booking not found
            return -1;
        }

        var getRoom = _roomRepository.GetByGuid(updatRoomDto.Guid);

        var room = new Room
        {
            Guid = updatRoomDto.Guid,
            Name = updatRoomDto.Name,
            Floor = updatRoomDto.Floor,
            Capacity = updatRoomDto.Capacity,
            ModifiedDate = DateTime.Now,
            CreatedDate = getRoom!.CreatedDate
        };

        var isUpdate = _roomRepository.Update(room);
        if (!isUpdate)
        {
            return 0; // Booking not updated
        }

        return 1;
    }

    public int DeleteRoom(Guid guid)
    {
        var isExist = _roomRepository.IsExist(guid);
        if (!isExist)
        {
            return -1; // Booking not found
        }

        var room = _roomRepository.GetByGuid(guid);
        var isDelete = _roomRepository.Delete(room!);
        if (!isDelete)
        {
            return 0; // Booking not deleted
        }

        return 1;
    }
}

