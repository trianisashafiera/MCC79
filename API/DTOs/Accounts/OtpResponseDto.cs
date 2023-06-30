namespace API.DTOs.Accounts;
    public class OtpResponseDto
    {
        public string Email { get; set; }
        public Guid Guid { get; set; }
        public int Otp { get; set; }
        public DateTime ExpiredTime { get; set; }
    }

