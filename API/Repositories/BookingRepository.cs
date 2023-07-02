using API.Contracts;
using API.Data;
using API.DTOs.Bookings;
using API.Models;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories;
    public class BookingRepository : GeneralRepository<Booking>, IBookingRepository
    {
        public BookingRepository(BookingDbContext context) : base(context) { }
    public IEnumerable<DetailBookingDto> GetBookingDetails()
    {
        var bookings = _context.Bookings.Include(b => b.Room)
            .Include(b => b.Employee)
            .Where(b => b.Room.Guid == b.Room.Guid && b.Employee.Guid == b.Employee.Guid).ToList();

        var bookingDetails = bookings.Select(b => new DetailBookingDto
        {
            Guid = b.Guid,
            BookedNik = b.Employee.Nik,
            BookedBy = b.Employee.FirstName + "" + b.Employee.LastName,
            RoomName = b.Room.Name,
            StartDate = b.StartDate,
            EndDate = b.EndDate,
            Status = b.Status,
            Remarks = b.Remarks
        });
        return bookingDetails;
    }

    public ICollection<BookingRoomTodayDto>? GetByDateNow()
    {
        return _context.Set<Booking>()
           .Join(
               _context.Set<Employee>(),
               booking => booking.EmployeeGuid,
               employee => employee.Guid,
               (booking, employees) => new { Bookings = booking, Employees = employees }
           )
           .Join(
               _context.Set<Room>(),
               joinedData => joinedData.Bookings.RoomGuid,
               room => room.Guid,
               (joinedData, room) => new { joinedData.Employees, joinedData.Bookings, Rooms = room }
           )
           .Where(b => b.Bookings.StartDate <= DateTime.Now).Where(b => b.Bookings.EndDate >= DateTime.Now)
           .Select(joinData => new BookingRoomTodayDto
           {

               BookingGuid = joinData.Bookings.Guid,
               RoomName = joinData.Rooms.Name,
               Status = joinData.Bookings.Status,
               Floor = joinData.Rooms.Floor,
               BookedBy = joinData.Employees.FirstName + " " + joinData.Employees.LastName
           })
           .ToList();
    }
}

