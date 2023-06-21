using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace API.Models
{
    [Table("tb_tr_bookings")]
    public class Booking
    {
        [Key]
        [Column("guid")]
        public Guid Guid { get; set; }

        [Column ("start_Date")]
        public DateTime StartDate { get; set; }

        [Column ("end_date")]
        public DateTime EndDate { get; set; }

        [Column("remarks", TypeName = "nvarchar(255)")]
        public string Remarks { get; set; }

        [Column("room_guid")]
        public Guid RoomGuid { get; set; }

        [Column("employee_id")]
        public Guid EmployeeId { get; set; }

        [Column("status")]
        public int Status { get; set; }

        [Column("created_date")]
        public DateTime CreatedDate { get; set; }
        [Column("modified_date")]
        public DateTime ModifiedDate { get; set; }
    }
}
