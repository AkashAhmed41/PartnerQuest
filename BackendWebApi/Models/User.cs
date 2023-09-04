using BackendWebApi.Extensions;
using Microsoft.AspNetCore.Identity;

namespace BackendWebApi.Models
{
    public class User : IdentityUser<int>
    {
        public DateOnly DateOfBirth { get; set; }
        public string Nickname { get; set; }
        public DateTime Created { get; set; } = DateTime.UtcNow;
        public DateTime LastActive { get; set; } = DateTime.UtcNow;
        public string Gender { get; set; }
        public string About { get; set; }
        public string LookingFor { get; set; }
        public string Interests { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public List<Photo> Photos { get; set; } = new List<Photo>();
        public List<FavouriteUsers> AddedFavouriteUsers { get; set; }
        public List<FavouriteUsers> AddedFavouriteBy { get; set; }
        public List<Message> MessagesSent { get; set; }
        public List<Message> MessagesReceived { get; set; }
        public ICollection<UserRole> UserRoles { get; set; }

        // public int GetAge() 
        // {
        //     return DateOfBirth.CalculateAge();
        // }
    }
}