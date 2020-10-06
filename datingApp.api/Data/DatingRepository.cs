using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.api.Helpers;
using datingApp.api.Models;
using Microsoft.EntityFrameworkCore;

namespace datingApp.api.Data
{
    public class DatingRepository : IDatingRepository
    {
        private readonly DataContext _context;

        public DatingRepository(DataContext context)
        {
            _context = context;

        }
        
        public void Add<T>(T entity) where T : class
        {
            _context.Add(entity);
        }

        public void Delete<T>(T entity) where T : class
        {
            _context.Remove(entity);
        }

        public async Task<User> GetUser(int id)
        {
            var user = await _context.Users.Include(p => p.Photos).FirstOrDefaultAsync(u=>u.Id == id);
            return user;
        }

        public async Task<PagedList<User>> GetUsers(UserParams userParam)
        {
            var users = _context.Users.Include(p=>p.Photos).OrderByDescending(u => u.LastActive).AsQueryable();
            users = users.Where(u => u.Id != userParam.UserId);
            users = users.Where(g =>g.Gender == userParam.Gender); 
            if ( userParam.MaxAge != 18 || userParam.MinAge != 99)
            {
                var minDob = DateTime.Today.AddYears(-userParam.MaxAge);
                var maxDob = DateTime.Today.AddYears(-userParam.MinAge);
                users = users.Where(u => u.DateOfBirth >= minDob && u.DateOfBirth <= maxDob);
            }

            if (!string.IsNullOrEmpty(userParam.OrderBy))
            {
                switch (userParam.OrderBy)
                {
                    case "created":
                        users = users.OrderByDescending(u => u.Created);
                        break;
                    case "age":
                        users = users.OrderByDescending(u => u.DateOfBirth);
                        break;
                    default:
                        users = users.OrderByDescending(u => u.LastActive);
                        break;
                }
            }

            return await PagedList<User>.CreateAsync(users, userParam.PageNumber, userParam.PageSize);
        }

        public async Task<bool> SaveAll()
        {
            return await _context.SaveChangesAsync() > 0 ;
        }

        public async Task<Photo> GetPhoto(int id)
        {
            var photo = await _context.Photos.FirstOrDefaultAsync(p => p.Id == id);
            return photo; 
        }

        public async Task<Photo> GetMianPhotoForUser(int userId)
        {
            return await _context.Photos.Where(p=>p.UserId == userId).FirstOrDefaultAsync(p=>p.IsMain);
        }
    }
}