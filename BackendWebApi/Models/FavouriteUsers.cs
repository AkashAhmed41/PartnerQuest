namespace BackendWebApi.Models;

public class FavouriteUsers
{
    public User SourceUser { get; set; }
    public int SourceUserId { get; set; }
    public User FavouriteUser { get; set; }
    public int FavouriteUserId { get; set; }
}