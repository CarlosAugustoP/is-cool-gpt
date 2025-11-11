namespace IsCool.Models
{
    public enum Language { EN, PTBR }
    public class User
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string Username { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;
        public string StudentOf { get; set; } = null!;
        public Language PreferredLanguage { get; set; } = Language.EN;

        public User() { }
        public User(string name, string username, string email, string passwordHash, string studentOf, Language preferredLanguage)
        {
            Id = Guid.NewGuid();
            Name = name;
            Username = username;
            Email = email;
            PasswordHash = passwordHash;
            StudentOf = studentOf;
            PreferredLanguage = preferredLanguage;
        }
    }
}