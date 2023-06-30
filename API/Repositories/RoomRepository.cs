using API.Contracts;
using API.Data;
using API.DTOs.Rooms;
using API.Models;
using System.Linq;

namespace API.Repositories;
    public class RoomRepository : GeneralRepository<Room>, IRoomRepository
    {
        public RoomRepository(BookingDbContext context) : base(context) { }

    public ICollection<NewRoomDto>? GetByDateNow()
    {
        return _context.Set<Room>()

           .Join(
               _context.Set<Booking>(),
               room => room.Guid,
               booking => booking.Guid,
               (room, booking) => new { Room = room, Booking = booking }
           )
           .Where(joinedData => joinedData.Booking.StartDate <= DateTime.Now && joinedData.Booking.EndDate >= DateTime.Now)
        .Select(joinedData => new NewRoomDto
        {
               Guid = joinedData.Room.Guid,
               Name = joinedData.Room.Name,
               Floor = joinedData.Room.Floor,
               Capacity = joinedData.Room.Capacity
           })
           .ToList();
    }
}
