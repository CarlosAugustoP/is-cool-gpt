using System.Text;
using Newtonsoft.Json.Serialization;
using OpenAI.Chat;

namespace IsCool.Abstractions
{
    public class PromptBuilder
    {
        private string _userMsg = "";
        private Models.User? _user;
        public PromptBuilder WithUserMessage(string message)
        {
            _userMsg = message;
            return this;
        }

        public PromptBuilder WithUser(Models.User user)
        {
            _user = user;
            return this;
        }
        public (ChatResponseFormat, string) Build()
    {
          if (_user == null)
            {
                throw new InvalidOperationException("User information must be provided.");
            }
            var jsonSchema = BinaryData.FromString(@"
            {
              ""name"": ""IsCoolResponse"",
              ""description"": ""Estrutura da resposta do assistente IsCool AI."",
              ""type"": ""object"",
              ""properties"": {
                ""summary"": {
                  ""type"": ""string"",
                  ""description"": ""Resumo claro e conciso da resposta.""
                },
                ""details"": {
                  ""type"": ""string"",
                  ""description"": ""Explicação mais detalhada ou contextual.""
                }
              },
              ""required"": [""summary""]
            }");

            var format = ChatResponseFormat.CreateJsonSchemaFormat(
                jsonSchemaFormatName: "IsCoolResponse",
                jsonSchema: jsonSchema
            );

            const string baseMsg = 
                "You are IsCool AI, an AI assistant dedicated to helping students with their academic needs. " +
                "Provide clear, concise, and accurate information in a friendly manner.";

            var sb = new StringBuilder();
            sb.AppendLine(baseMsg);
            sb.AppendLine($"User Message: {_userMsg}");
            sb.AppendLine("Use the user's preferred language for the response:" + _user.PreferredLanguage.ToString());
            sb.AppendLine("Keep the response relevant to the user's field of study: " + _user.StudentOf);
            sb.AppendLine("Return the answer in: { \"summary\": \"...\", \"details\": \"...\" }");

            return (format, sb.ToString());
        }
    }
}
