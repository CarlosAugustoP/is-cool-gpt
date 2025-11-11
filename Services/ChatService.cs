using System.Text.Json;
using IsCool.Abstractions;
using IsCool.DB;
using IsCool.DTO;
using IsCool.Exceptions;
using IsCool.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Newtonsoft.Json.Linq;
using Resumai.Services.Application;

namespace IsCool.Services
{
    public class ChatService
    {
        private readonly AppDbContext _db;
        private readonly OpenAiService _openAiService;
        public ChatService(AppDbContext db, OpenAiService openAiService)
        {
            _db = db;
            _openAiService = openAiService;
        }
        public Guid StartChat(UserDTO currentUser)
        {
            var chat = new Chat(currentUser.Id);
            _db.Chats.Add(chat);
            _db.SaveChanges();
            return chat.Id;
        }

        //TODO make response an actula type not object
        public async Task<IsCoolResponseDto> TalkToAi(string message, UserDTO currentUser, Guid chatId)
        {
            if (!_db.Chats.Any(chat => chat.Id == chatId && chat.UserId == currentUser.Id))
            {
                throw new NotFoundException("Chat not found");
            }

            var user = _db.Users.FirstOrDefault(u => u.Id == currentUser.Id)
                ?? throw new NotFoundException("User not found");

            var promptBuilder = new PromptBuilder()
                .WithUserMessage(message)
                .WithUser(user);

            var response = await _openAiService.CallChat(promptBuilder);

            var toPersist = new
            {
                userMessage = message
            };
            
            _db.PromptMessages.Add(new PromptMessage(WhoSentEnum.User, JsonSerializer.Serialize(toPersist), chatId));
            _db.PromptMessages.Add(new PromptMessage(WhoSentEnum.AI, JsonSerializer.Serialize(response), chatId));
            _db.SaveChanges();

            return response;
        }
        public async Task<string> GetChatName(Guid chatId, UserDTO currentUser)
        {
            var chat = _db.Chats.FirstOrDefault(c => c.Id == chatId && c.UserId == currentUser.Id)
                ?? throw new NotFoundException("Chat not found");

            if (!string.IsNullOrEmpty(chat.Name))
            {
                return chat.Name;
            }

            var chatName = await _openAiService.CallChat("Generate a concise and relevant name for a conversation about the following topic: " +
                "Make it short, no more than 5 words, and avoid special characters.");

            chat.SetName(chatName);
            await _db.SaveChangesAsync();

            return chatName;
        }
    }
}