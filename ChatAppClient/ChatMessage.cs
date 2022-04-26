public class ChatMessage
{
    public Guid Id { get; set; }
    public String Message { get; set; } = null!;
    public DateTime SendDateTime { get; set; }
}