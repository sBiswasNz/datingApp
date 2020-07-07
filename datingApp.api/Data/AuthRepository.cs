using System;
using System.Threading.Tasks;
using datingApp.api.Models;
using Microsoft.EntityFrameworkCore;

namespace datingApp.api.Data
{
    public class AuthRepository : IAuthRepository
    {
        private readonly DataContext _context;
        public AuthRepository(DataContext context)
        {
            _context = context;

        }
        public async Task<User> Login(string username, string passwerd)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Name == username); 
            
            if( user == null)
                return null; 

            if ( ! VarifyPasswordHash( passwerd, user.PasswordHash, user.PasswordSalt))
                return null; 

            return user; 
        }

        private bool VarifyPasswordHash(string passwerd, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA256(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(passwerd)); 
                for (int i = 0; i<computedHash.Length; i++)
                {
                    if (computedHash[i] != passwordHash[i]) return false; 
                }
            }
            return true; 
        }

        public async Task<User> Register(User user, string passwerd)
        {
            byte [] passwerdHash, passwerdSalt;
            CreatePasswordHash(passwerd, out passwerdHash, out passwerdSalt);

            user.PasswordHash = passwerdHash;
            user.PasswordSalt = passwerdSalt;

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();  
 
            return user;
        }

        private void CreatePasswordHash(string password, out byte[] passwerdHash, out byte[] passwerdSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA256())
            {
                passwerdSalt = hmac.Key;
                passwerdHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password)); 
            }
        }

        public async Task<bool> UserExists(string username)
        {
            if (await _context.Users.AnyAsync(x => x.Name == username))
                return true;
            return false; 
        }
    }
}