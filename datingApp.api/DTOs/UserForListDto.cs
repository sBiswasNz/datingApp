using System;

namespace datingApp.api.DTOs
{
    public class UserForListDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Gende { get; set; }
        public int Age { get; set; }
        public string KnownAs { get; set; }
        public DateTime Created { get; set; }
        public DateTime LastActivate { get; set; }
        public string City { get; set; }
        public string  Country { get; set; }
        public string PhotoUrl { get; set; }
        

    }
}