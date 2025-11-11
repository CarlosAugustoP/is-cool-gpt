using IsCool.Models;

namespace IsCool.DTO
{
    public class MessageDTO
    {
        public WhoSentEnum WhoSent { get; set; }
        public IsCoolResponseDto? Message { get; set; }
        public string? UserMessage { get; set; }
    }
}