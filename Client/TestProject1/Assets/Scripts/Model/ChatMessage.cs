using MessagePack;

[MessagePackObject]
public class ChatMessage
{
    [Key(0)]
    public string Username { get; set; }

    [Key(1)]
    public string Message { get; set; }

    [Key(2)]
    public long Timestamp { get; set; }
}
