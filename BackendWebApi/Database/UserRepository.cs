using AutoMapper;
using AutoMapper.QueryableExtensions;
using BackendWebApi.Dataflow;
using BackendWebApi.Helpers;
using BackendWebApi.Interfaces;
using BackendWebApi.Models;
using Microsoft.EntityFrameworkCore;

namespace BackendWebApi.Database
{
    public class UserRepository : IUserRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        public UserRepository(DataContext context, IMapper mapper)
        {
            _mapper = mapper;
            _context = context;
        }
        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _context.Users.Include(p => p.Photos).ToListAsync();
        }

        public async Task<MemberDataflow> GetMemberAsync(string username)
        {
            return await _context.Users.Where(x => x.UserName == username).ProjectTo<MemberDataflow>(_mapper.ConfigurationProvider).SingleOrDefaultAsync();
        }

        public async Task<PaginatedList<MemberDataflow>> GetMembersAsync(UserParamsForPagination userParamsForPagination)
        {
            var query = _context.Users.AsQueryable();

            query = query.Where(u => u.UserName != userParamsForPagination.CurrentUsername);
            query = query.Where(u => u.Gender == userParamsForPagination.Gender);

            var minDob = DateOnly.FromDateTime(DateTime.Today.AddYears(-userParamsForPagination.MaxAge - 1));
            var maxDob = DateOnly.FromDateTime(DateTime.Today.AddYears(-userParamsForPagination.MinAge));

            query = query.Where(u => u.DateOfBirth <= maxDob && u.DateOfBirth >= minDob);

            query = userParamsForPagination.OrderBy switch
            {
                "created" => query.OrderByDescending(u => u.Created),
                _ => query.OrderByDescending(u => u.LastActive)
            };

            return await PaginatedList<MemberDataflow>.CreatePageAsync(query.AsNoTracking().ProjectTo<MemberDataflow>(_mapper.ConfigurationProvider), userParamsForPagination.PageNumber, userParamsForPagination.PageSize);
        }

        public async Task<User> GetUserByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<User> GetUserByUsernameAsync(string username)
        {
            return await _context.Users.Include(p => p.Photos).FirstOrDefaultAsync(x => x.UserName == username);
        }

        public async Task<string> GetUserGender(string username)
        {
            return await _context.Users.Where(x => x.UserName == username).Select(x => x.Gender).FirstOrDefaultAsync();
        }

        public void Update(User user)
        {
            _context.Entry(user).State = EntityState.Modified;
        }
    }
}