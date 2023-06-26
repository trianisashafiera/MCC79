﻿using API.Utilities.Enums;

namespace API.DTOs.Bookings
{
    public class NewBookingDto
    {
        public Guid Guid { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public StatusLevel Status { get; set; }
        public string? Remarks { get; set; }
        public Guid RoomGuid { get; set; }
        public Guid EmployeeGuid { get; set; }

    }
}
