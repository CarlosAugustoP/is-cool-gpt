using System.Text.Json;
using IsCool.Abstractions;
using IsCool.DTO;
using OpenAI.Chat;
using OpenAI.Responses;


namespace Resumai.Services.Application
{
    public static class OpenAiService
    {
        public static async Task<IsCoolResponseDto> CallChat(PromptBuilder p)
        {
            ChatClient client = new(model: "gpt-4o", apiKey: Environment.GetEnvironmentVariable("OPENAI_API_KEY"));
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

        public static async Task<string> CallChat(string input)
        {
            ChatClient client = new(model: "gpt-4o", apiKey: Environment.GetEnvironmentVariable("OPENAI_API_KEY"));
            var msgs = new List<ChatMessage>()
            {
                new UserChatMessage(input)
            };
            ChatCompletion response = await client.CompleteChatAsync(messages: msgs);
            return response.Content[0].Text;
        }
    }
}