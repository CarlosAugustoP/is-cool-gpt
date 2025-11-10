using OpenAI.Chat;

namespace Resumai.Abstractions
{
    public class PromptBuilder
    {
        public (ChatResponseFormat, string) Build()
        {
            return (ChatResponseFormat.CreateJsonObjectFormat(), string.Empty);
        }
    }
}