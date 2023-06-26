using API.Contracts;
using API.DTOs.Bookings;
using API.Models;

namespace API.Services;
    public class BookingService
    {
        private readonly IBookingRepository _bookingRepository;
        public BookingService(IBookingRepository bookingRepository)
        {
             _bookingRepository = bookingRepository;
        }

        public IEnumerable<NewBookingDto> GetBooking()
        {
            var bookings = _bookingRepository.GetAll();
            if (!bookings.Any())
            {
                return null;
            }
            var toDto = bookings.Select(booking =>
                                               new NewBookingDto
                                               {
                                                   Guid = booking.Guid,
                                                   StartDate = booking.StartDate,
                                                   EndDate = booking.EndDate,
                                                   Status = booking.Status,
                                                   Remarks = booking.Remarks,
                                                   RoomGuid = booking.RoomGuid,
                                                   EmployeeGuid = booking.EmployeeGuid

                                               }).ToList();

            return toDto;

        }
        public NewBookingDto? GetBooking(Guid guid)
        {
            var booking = _bookingRepository.GetByGuid(guid);
            if (booking is null)
            {
                return null; // Booking not found
        }

            var toDto = new NewBookingDto
            {
                Guid = booking.Guid,
                StartDate = booking.StartDate,
                EndDate = booking.EndDate,
                Status = booking.Status,
                Remarks = booking.Remarks,
                RoomGuid = booking.RoomGuid,
                EmployeeGuid = booking.EmployeeGuid
            };

            return toDto; // Booking found
    }

        public NewBookingDto? CreateBooking(NewBookingDto newBookingDto)
        {
            var booking = new Booking
            {
                Guid = new Guid(),
                StartDate = newBookingDto.StartDate,
                EndDate = newBookingDto.EndDate,
                Status = newBookingDto.Status,
                Remarks = newBookingDto.Remarks,
                RoomGuid = newBookingDto.RoomGuid,
                EmployeeGuid = newBookingDto.EmployeeGuid,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now
            };

            var createdBooking = _bookingRepository.Create(booking);
            if (createdBooking is null)
            {
                return null; // Booking not created
        }

            var toDto = new NewBookingDto
            {
                Guid = createdBooking.Guid,
                StartDate = createdBooking.StartDate,
                EndDate = createdBooking.EndDate,
                Status = createdBooking.Status,
                Remarks = createdBooking.Remarks,
                RoomGuid = createdBooking.RoomGuid,
                EmployeeGuid = createdBooking.EmployeeGuid
            };

            return toDto; // Booking created
        }

        public int UpdateBooking(NewBookingDto updateBookingDto)
        {
            var isExist = _bookingRepository.IsExist(updateBookingDto.Guid);
            if (!isExist)
            {
                // Booking not found
                return -1;
            }

            var getBooking = _bookingRepository.GetByGuid(updateBookingDto.Guid);

            var booking = new Booking
            {
                Guid = updateBookingDto.Guid,
                StartDate = updateBookingDto.StartDate,
                EndDate = updateBookingDto.EndDate,
                Status = updateBookingDto.Status,
                Remarks = updateBookingDto.Remarks,
                RoomGuid = updateBookingDto.RoomGuid,
                EmployeeGuid = updateBookingDto.EmployeeGuid,
                ModifiedDate = DateTime.Now,
                CreatedDate = getBooking!.CreatedDate
            };

            var isUpdate = _bookingRepository.Update(booking);
            if (!isUpdate)
            {
                return 0; // Booking not updated
            }

            return 1;
        }

        public int DeleteBooking(Guid guid)
        {
            var isExist = _bookingRepository.IsExist(guid);
            if (!isExist)
            {
                return -1; // Booking not found
            }

            var booking = _bookingRepository.GetByGuid(guid);
            var isDelete = _bookingRepository.Delete(booking!);
            if (!isDelete)
            {
                return 0; // Booking not deleted
            }

            return 1;
        }

        internal object GetBooking(string name)
        {
            throw new NotImplementedException();
        }
    }

