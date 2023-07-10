using BackendWebApi.Dataflow;
using BackendWebApi.Helpers;
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
        Task <PaginatedList<MemberDataflow>> GetMembersAsync(UserParamsForPagination userParamsForPagination);
        Task <MemberDataflow> GetMemberAsync(string username);
    }
}