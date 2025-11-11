using System.Runtime.CompilerServices;
using System.Text.Json;
using IsCool.Abstractions;
using IsCool.DB;
using IsCool.DTO;
using IsCool.Exceptions;
using IsCool.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
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
            var chat = _db.Chats.Include(x => x.PromptMessage).FirstOrDefault(c => c.Id == chatId && c.UserId == currentUser.Id)
                ?? throw new NotFoundException("Chat not found");

            if (!string.IsNullOrEmpty(chat.Name))
            {
                return chat.Name;
            }

            var json = JsonSerializer.Deserialize<IsCoolResponseDto>(chat.PromptMessage.First().Message);
            var content = json!.Summary;

            var chatName = await _openAiService.CallChat("Generate a concise and relevant name for a conversation about the following topic: " + content +
                "Make it short, no more than 5 words, and avoid special characters.");

            chat.SetName(chatName);
            await _db.SaveChangesAsync();

            return chatName;
        }

        public async Task<Dictionary<Guid, string?>> GetChatHistory(UserDTO user, int pageNumber, int pageSize)
        {
            if (pageNumber < 1 || pageSize < 1) throw new DomainException("Invalid request");

            return _db.Chats
                .Where(x => x.UserId == user.Id)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToDictionary(x => x.Id, x => x.Name);
        }

        public async Task<List<MessageDTO>> GetByChat(UserDTO user, Guid chatId, int pageNumber, int pageSize)
        {
            if (pageNumber < 1 || pageSize < 1) throw new DomainException("Invalid request");

            var firstQuery = _db.Chats
                .Include(x => x.PromptMessage)
                .FirstOrDefault(x => x.UserId == user.Id && x.Id == chatId)
                ?? throw new NotFoundException("Could not find chat");

            return firstQuery.PromptMessage
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList()
                .Select(Get)
                .ToList();
        }
        public MessageDTO Get(PromptMessage prompt)
        {
            var outMessage = new MessageDTO();
            if (prompt.WhoSent == WhoSentEnum.AI)
            {
                outMessage.Message = JsonSerializer.Deserialize<IsCoolResponseDto>(prompt.Message);
            }
            else if (prompt.WhoSent == WhoSentEnum.User)
            {
                outMessage.UserMessage = JsonDocument.Parse(prompt.Message).RootElement.GetProperty("userMessage").GetString();
            }
            outMessage.WhoSent = prompt.WhoSent;
            return outMessage;
        }
    }
}