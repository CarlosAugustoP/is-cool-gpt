namespace IsCool.Models
{
    public class Chat
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public DateTime Timestamp { get; set; }
        public User User { get; set; } = null!;

        public Chat() { }

        public Chat(Guid userId, DateTime timestamp)
        {
            Id = Guid.NewGuid();
            UserId = userId;
            Timestamp = timestamp;
        }
        }
}