using System.Collections.Generic;
using System.Linq;
using datingApp.api.Models;
using Newtonsoft.Json;

namespace datingApp.api.Data
{
    public class Seed
    {
        public static void SeedUser(DataContext context)
        {
            if (! context.Users.Any())
            {
                var userData = System.IO.File.ReadAllText("Data/UserSeedData.json");
                var users = JsonConvert.DeserializeObject<List<User>>(userData);
                
                foreach (var user in users)
                {
                    byte[] passwordHash, passwordSalt; 
                    CreatePasswordHash("password", out passwordHash, out passwordSalt); 

                    user.PasswordHash = passwordHash;
                    user.PasswordSalt = passwordSalt; 
                    user.Name = user.Name.ToLower();
                    context.Users.Add(user);
                }
                context.SaveChanges();
                
            
            }

        }

        private static void CreatePasswordHash(string password, out byte[] passwerdHash, out byte[] passwerdSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA256())
            {
                passwerdSalt = hmac.Key;
                passwerdHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password)); 
            }
        }
    }
}