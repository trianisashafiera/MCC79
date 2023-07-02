﻿using API.Contracts;
using API.DTOs.Bookings;
using API.Models;
using API.Repositories;


namespace API.Services;
    public class BookingService
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly IRoomRepository _roomRepository;
        private readonly IEmployeeRepository _employeeRepository;
    public BookingService(IBookingRepository bookingRepository,
                          IRoomRepository roomRepository,
                          IEmployeeRepository employeeRepository)
        {
             _bookingRepository = bookingRepository;
             _roomRepository = roomRepository;
             _employeeRepository = employeeRepository;
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

    public List<DetailBookingDto>? GetBookingDetails()
    {
        var bookings = _bookingRepository.GetBookingDetails();
        var bookingDetails = bookings.Select(b => new DetailBookingDto
        {
            Guid = b.Guid,
            BookedNik = b.BookedNik,
            BookedBy = b.BookedBy,
            RoomName = b.RoomName,
            StartDate = b.StartDate,
            EndDate = b.EndDate,
            Status = b.Status,
            Remarks = b.Remarks
        }).ToList();

        return bookingDetails;
    }

    public DetailBookingDto? GetBookingDetailsByGuid(Guid guid)
    {
        var relatedBooking = GetBookingDetails().SingleOrDefault(b => b.Guid == guid);
        return relatedBooking;
    }

    public IEnumerable<GetBookingDurationDto> BookingDuration()
    {
        var bookings = _bookingRepository.GetAll();
        var rooms = _roomRepository.GetAll();

        var entities = (from booking in bookings
                        join room in rooms on booking.RoomGuid equals room.Guid
                        select new
                        {
                            guid = room.Guid,
                            startDate = booking.StartDate,
                            endDate = booking.EndDate,
                            roomName = room.Name
                        }).ToList();

        var listBookingDurations = new List<GetBookingDurationDto>();

        foreach (var entity in entities)
        {
            TimeSpan duration = entity.endDate - entity.startDate;

            // Count the number of weekends within the duration
            int totalDays = (int)duration.TotalDays;
            int weekends = 0;

            for (int i = 0; i <= totalDays; i++)
            {
                var currentDate = entity.startDate.AddDays(i);
                if (currentDate.DayOfWeek == DayOfWeek.Saturday || currentDate.DayOfWeek == DayOfWeek.Sunday)
                {
                    weekends++;
                }
            }

            // Calculate the duration without weekends
            TimeSpan bookingLength = duration - TimeSpan.FromDays(weekends);

            var bookingDurationDto = new GetBookingDurationDto
            {
                RoomGuid = entity.guid,
                RoomName = entity.roomName,
                BookingLength = $"{bookingLength.Days} Hari"
            };

            listBookingDurations.Add(bookingDurationDto);
        }

        return listBookingDurations;
    }

    public IEnumerable<BookingRoomTodayDto> BookingNow()
    {
        var bookings = _bookingRepository.GetAll();
        if (bookings == null)
        {
            return null; // No Booking  found
        }


        // versi LINQ
        var employees = _employeeRepository.GetAll();
        var rooms = _roomRepository.GetAll();

        var bookingNow = (
            from booking in bookings
            join employee in employees on booking.EmployeeGuid equals employee.Guid
            join room in rooms on booking.RoomGuid equals room.Guid
            where booking.StartDate <= DateTime.Now.Date && booking.EndDate >= DateTime.Now
            select new BookingRoomTodayDto
            {
                BookingGuid = booking.Guid,
                RoomName = room.Name,
                Status = booking.Status,
                Floor = room.Floor,
                BookedBy = employee.FirstName + " " + employee.LastName,
            }
        ).ToList();

        return bookingNow;
    }

    public IEnumerable<DetailBookingDto>? BookingDetail()
    {
        var books = (from b in _bookingRepository.GetAll()
                     join e in _employeeRepository.GetAll() on b.EmployeeGuid equals e.Guid
                     join r in _roomRepository.GetAll() on b.RoomGuid equals r.Guid
                     select new DetailBookingDto
                     {
                         Guid = b.Guid,
                         BookedNik = e.Nik,
                         BookedBy = e.FirstName + " " + e.LastName,
                         StartDate = DateTime.Now,
                         EndDate = b.EndDate,
                         RoomName = r.Name,
                         Status = b.Status,
                         Remarks = b.Remarks,
                     }).ToList();
        if (!books.Any())
        {
            return null;
        }

        return books;
    }

    public DetailBookingDto? BookingDetail(Guid guid)
    {
        var books = BookingDetail();

        var bookByGuid = books!.FirstOrDefault(book => book.Guid == guid);

        return bookByGuid;

    }

}

