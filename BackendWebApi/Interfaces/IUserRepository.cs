using BackendWebApi.Dataflow;
using BackendWebApi.Models;

namespace BackendWebApi.Interfaces
{
    public interface IUserRepository
    {
        void Update (User user);
        Task <bool> SaveAllAsync();
        Task <IEnumerable<User>> GetAllUsersAsync();
        Task <User> GetUserByIdAsync(int id);
        Task <User> GetUserByUsernameAsync(string username);
        Task <IEnumerable<MemberDataflow>> GetMembersAsync();
        Task <MemberDataflow> GetMemberAsync(string username);
    }
}