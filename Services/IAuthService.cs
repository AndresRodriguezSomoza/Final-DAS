using DAS_Final.Models;
using System.Security.Claims;

namespace DAS_Final.Services
{
    public interface IAuthService
    {
        Task<Usuario?> Authenticate(string email, string password);
        Task<bool> UserExists(string email);
    }
}
