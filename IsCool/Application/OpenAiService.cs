using System.Text.Json;
using IsCool.Abstractions;
using IsCool.DTO;
using Microsoft.Extensions.Configuration;
using OpenAI.Chat;
using OpenAI.Responses;


namespace Resumai.Services.Application
{
    public class OpenAiService
    {
        private readonly IConfiguration _configuration;
        public OpenAiService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task<IsCoolResponseDto> CallChat(PromptBuilder p)
        {
            var key = _configuration.GetValue<string>("OpenAiApiKey");
            ChatClient client = new(model: "gpt-4o", apiKey: key);
            var (options, prompt) = p.Build();
            var msgs = new List<ChatMessage>()
            {
                new UserChatMessage(prompt)
            };
            ChatCompletion response = await client.CompleteChatAsync(messages: msgs,
            options: new ChatCompletionOptions
            {
                ResponseFormat = options
            });
            var json = response.Content[0].Text;
            return JsonSerializer.Deserialize<IsCoolResponseDto>(json)!;
        }

        public async Task<string> CallChat(string input)
        {
            var key = _configuration.GetValue<string>("OpenAiApiKey");
            ChatClient client = new(model: "gpt-4o", apiKey: key);
            var msgs = new List<ChatMessage>()
            {
                new UserChatMessage(input)
            };
            ChatCompletion response = await client.CompleteChatAsync(messages: msgs);
            return response.Content[0].Text;
        }
    }
}