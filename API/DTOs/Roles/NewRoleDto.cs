using System.ComponentModel.DataAnnotations;

namespace API.DTOs.Roles
{
    public class NewRoleDto
    {
        public Guid Guid { get; set; }
        [Required]
        public string Name { get; set; }
    }
}
