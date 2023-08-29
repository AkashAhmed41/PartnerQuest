namespace BackendWebApi.Helpers;

public class FavouritesPaginationParams : PaginationParams
{
    public int UserId { get; set; }
    public string Predicate { get; set; }
}