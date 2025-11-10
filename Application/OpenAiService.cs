using System.Text.Json;
using OpenAI.Chat;
using Resumai.Abstractions;


namespace Resumai.Services.Application
{
    public static class OpenAiService
    {
        public static async Task<object> Call(PromptBuilder p)
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
            return JsonSerializer.Deserialize<object>(json)!;
        }
    }
}