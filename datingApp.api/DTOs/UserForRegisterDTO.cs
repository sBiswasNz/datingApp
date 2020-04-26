using System.ComponentModel.DataAnnotations;

namespace datingApp.api.DTOs
{
    public class UserForRegisterDTO
    {
        [Required]
        public string username { get; set; }

        [Required]
        public string  password { get; set; }

    }
}