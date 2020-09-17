using System;
using System.ComponentModel.DataAnnotations;

namespace datingApp.api.DTOs
{
    public class UserForRegisterDTO
    {
        [Required]
        public string Name { get; set; }
        [Required]
        [StringLength(8, MinimumLength=4, ErrorMessage="Choose password between 8 to 4 characters.")]
        public string  Password { get; set; }
        [Required]
        public string Gender { get; set; }
        [Required]
        public string KnownAs { get; set; }
        [Required]
        public DateTime DateOfBirth { get; set; }
        [Required]
        public string City { get; set; }
        [Required]
        public string Country { get; set; }
        public DateTime Created { get; set; }
        public DateTime LastActive { get; set; }

        public UserForRegisterDTO()
        {
            Created = DateTime.Now;
            LastActive = DateTime.Now;
        }
    }
}