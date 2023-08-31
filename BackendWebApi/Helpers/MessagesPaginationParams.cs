namespace BackendWebApi.Helpers;

public class MessagesPaginationParams : PaginationParams
{
    public string Username { get; set; }
    public string Container { get; set; } = "Unread";
}