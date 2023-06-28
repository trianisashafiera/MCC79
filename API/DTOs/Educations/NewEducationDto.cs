﻿using System.ComponentModel.DataAnnotations;

namespace API.DTOs.Educations
{
    public class NewEducationDto
    {
        [Required]
        public Guid Guid { get; set; }
        [Required]
        public string Major { get; set; }
        [Required]
        public string Degree { get; set; }
        [Required]
        [Range(0,4, ErrorMessage = "GPA must be between 0 and 4")]
        public double Gpa { get; set; }
        [Required]
        public Guid UniversityGuid { get; set; }
    }
}
