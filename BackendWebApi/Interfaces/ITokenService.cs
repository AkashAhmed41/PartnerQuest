using BackendWebApi.Models;

namespace BackendWebApi.Interfaces;

public interface ITokenService
{
    Task<string> CreateToken(User user);
}