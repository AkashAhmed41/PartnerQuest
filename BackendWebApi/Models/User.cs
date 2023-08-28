using BackendWebApi.Extensions;

namespace BackendWebApi.Models
{
    public class User
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
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

        // public int GetAge() 
        // {
        //     return DateOfBirth.CalculateAge();
        // }
    }
}