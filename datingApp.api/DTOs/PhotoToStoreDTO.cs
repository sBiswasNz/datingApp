using System;
using Microsoft.AspNetCore.Http;

namespace datingApp.api.DTOs
{
    public class PhotoToStoreDTO
    {
        public string Url { get; set; }
        public IFormFile File { get; set; }
        public string Description { get; set; }
        public DateTime DateAdded { get; set; }
        public string PublicId { get; set; }
        

        public PhotoToStoreDTO()
        {
            DateAdded = DateTime.Now;
        }
        
    }
}