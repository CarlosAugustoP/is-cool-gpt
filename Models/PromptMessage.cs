namespace IsCool.Models
{
    public enum WhoSentEnum { User, AI }
    public class PromptMessage
    {
        public Guid Id { get; set; }
        public WhoSentEnum WhoSent { get; set; }
        //Will be JSONB in Postgres
        public string Message { get; set; } = null!;
        public DateTime Timestamp { get; set; }
        public Chat Chat { get; set; } = null!;
        public Guid ChatId { get; set; }

        public PromptMessage() { }
        public PromptMessage(WhoSentEnum whoSent, string message, Guid chatId)
        {
            Id = Guid.NewGuid();
            WhoSent = whoSent;
            Message = message;
            Timestamp = DateTime.UtcNow;
            ChatId = chatId;
        }

    }
}