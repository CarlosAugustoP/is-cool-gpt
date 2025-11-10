namespace IsCool.Models
{
    public class User
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string Username { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;
        public string StudentOf { get; set; } = null!;

        public User() { }
        public User(string name, string username, string email, string passwordHash, string studentOf)
        {
            Id = Guid.NewGuid();
            Name = name;
            Username = username;
            Email = email;
            PasswordHash = passwordHash;
        }
    }
}