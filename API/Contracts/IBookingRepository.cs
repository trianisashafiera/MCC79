using API.Models;
using API.DTOs.Bookings;

namespace API.Contracts;
public interface IBookingRepository : IGeneralRepository<Booking>
{
    IEnumerable<DetailBookingDto> GetBookingDetails();

}
