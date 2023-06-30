using API.Contracts;
using API.DTOs.Rooms;
using API.Models;
using API.Utilities.Enums;

namespace API.Services;
public class RoomService
{
    private readonly IRoomRepository _roomRepository;
    private readonly IBookingRepository _bookingRepository;

    public RoomService(IRoomRepository roomRepository,
                       IBookingRepository bookingRepository)
    {
        _roomRepository = roomRepository;
        _bookingRepository = bookingRepository;
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
            return null; // Room not found
        }

        var toDto = new NewRoomDto
        {
            Guid = room.Guid,
            Name = room.Name,
            Floor = room.Floor,
            Capacity = room.Capacity
        };

        return toDto; // Room found
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
            return null; // Room not created
        }

        var toDto = new NewRoomDto
        {
            Guid = createdRoom.Guid,
            Name = createdRoom.Name,
            Floor = createdRoom.Floor,
            Capacity = createdRoom.Capacity
        };

        return toDto; // Room created
    }

    public int UpdateRoom(NewRoomDto updatRoomDto)
    {
        var isExist = _roomRepository.IsExist(updatRoomDto.Guid);
        if (!isExist)
        {
            // Room not found
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
            return 0; // Room not updated
        }

        return 1;
    }

    public int DeleteRoom(Guid guid)
    {
        var isExist = _roomRepository.IsExist(guid);
        if (!isExist)
        {
            return -1; // Room not found
        }

        var room = _roomRepository.GetByGuid(guid);
        var isDelete = _roomRepository.Delete(room!);
        if (!isDelete)
        {
            return 0; // Room not deleted
        }

        return 1;
    }

    public IEnumerable<UnUsedRoomDto> GetUnusedRoom()
    {
        var today = DateTime.Today;

        var rooms = _roomRepository.GetAll().ToList();


        var usedRooms = from room in _roomRepository.GetAll()
                        join booking in _bookingRepository.GetAll()
                        on room.Guid equals booking.RoomGuid
                        where booking.Status == StatusLevel.OnGoing || (booking.StartDate.DayOfYear == today.DayOfYear && booking.Status == StatusLevel.UpComing)
                        select new UnUsedRoomDto
                        {
                            RoomGuid = room.Guid,
                            RoomName = room.Name,
                            Floor = room.Floor,
                            Capacity = room.Capacity,
                        };
        int i = 0;
        List<Room> tmpRooms = new List<Room>(rooms);

        foreach (var room in rooms)
        {

            foreach (var usedRoom in usedRooms)
            {
                if (room.Guid == usedRoom.RoomGuid)
                {
                    tmpRooms.RemoveAt(i);
                    break;
                }
            }
            i++;
        }

        var unusedRooms = from room in tmpRooms
                          select new UnUsedRoomDto
                          {
                              RoomGuid = room.Guid,
                              RoomName = room.Name,
                              Floor = room.Floor,
                              Capacity = room.Capacity
                          };

        return unusedRooms;


    }
}


