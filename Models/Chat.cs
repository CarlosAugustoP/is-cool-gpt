namespace IsCool.Models
{
    public class Chat
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string? Name { get; private set; }
        public DateTime Timestamp { get; set; }
        public User User { get; set; } = null!;
        public List<PromptMessage> PromptMessage { get; set; } = [];

        public Chat() { }

        public Chat(Guid userId)
        {
            Id = Guid.NewGuid();
            UserId = userId;
            Timestamp = DateTime.UtcNow;
        }

        public void SetName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Chat name cannot be empty or whitespace.");
            }
            Name = name;
        }

    }
}