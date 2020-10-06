using System.Collections.Generic;
using System.Threading.Tasks;
using datingApp.api.Helpers;
using datingApp.api.Models;

namespace datingApp.api.Data
{
    public interface IDatingRepository
    {
        void Add<T>(T entity) where T: class;
        void Delete<T>(T entity) where T: class; 
        Task<bool> SaveAll();
        Task<PagedList<User>> GetUsers(UserParams userParams);
        Task<User> GetUser(int id);
        Task<Photo> GetPhoto(int id);
        Task<Photo> GetMianPhotoForUser(int userId);
    }
}