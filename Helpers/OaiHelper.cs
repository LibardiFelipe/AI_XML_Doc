using Standard.AI.OpenAI.Clients.OpenAIs;
using Standard.AI.OpenAI.Models.Configurations;
using Standard.AI.OpenAI.Models.Services.Foundations.ChatCompletions;

namespace AI_XML_Doc.Helpers
{
    public class OaiHelper
    {
        private readonly OpenAIClient _openAIClient;

        public OaiHelper(string apiKey)
        {
            var openAIConfigurations = new OpenAIConfigurations()
            {
                ApiKey = apiKey
            };

            _openAIClient = new OpenAIClient(openAIConfigurations);
        }

        public async ValueTask<string> GenerateXmlDocComment(string function, string xmlLanguage)
        {
            var prompt = $"Me retorne SOMENTE O XML Documentation Comments EM {xmlLanguage.ToUpper()} para a seguinte função em C#:\n{function}";
            var chatCompletion = new ChatCompletion
            {
                Request = new ChatCompletionRequest
                {
                    Model = "gpt-3.5-turbo",
                    MaxTokens = 1024,
                    Temperature = 0.4D,
                    Messages = new ChatCompletionMessage[]
                    {
                        new ChatCompletionMessage
                        {
                            Content = prompt,
                            Role = "user",
                        }
                    },
                }
            };

            var resultChatCompletion =
                await _openAIClient.ChatCompletions.SendChatCompletionAsync(
                    chatCompletion);

            return resultChatCompletion.Response.Choices[0].Message.Content;
        }
    }
}
