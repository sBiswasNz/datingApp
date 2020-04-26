using System.Threading.Tasks;
using datingApp.api.Models;

namespace datingApp.api.Data
{
    public interface IAuthRepository
    {
         Task<User> Register (User user, string passwerd);
         Task<User> Login (string username, string passwerd);
         Task<bool> UserExists(string username);
    }
}