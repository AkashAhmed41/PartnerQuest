using BackendWebApi.Models;

namespace BackendWebApi.Interfaces
{
    public interface ITokenService
    {
        string CreateToken(User user);
    }
}