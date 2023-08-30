namespace BackendWebApi.Dataflow;

public class MessageDataflow
{
    public int Id { get; set; }
    public int SenderId { get; set; }
    public string SenderUsername { get; set; }
    public string SenderProfilePhotoUrl { get; set; }
    public int RecipientId { get; set; }
    public string RecipientUsername { get; set; }
    public string RecipientProfilePhotoUrl { get; set; }
    public string MessageContent { get; set; }
    public DateTime? MessageRead { get; set; }
    public DateTime MessageSent { get; set; }
    
}