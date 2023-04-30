using System.Text;
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

        public async ValueTask<string> GenerateXmlDocComment(string function, string? language)
        {
            var sb = new StringBuilder();
            sb.AppendLine("Assume you are building a large project in C#.");
            sb.AppendLine("You have been asked to provide XML documentation comments for each function to help other developers understand how to use them.");
            sb.AppendLine($"Given the function that I will further provide you, write the XML documentation comment for it IN {(language ?? "english").ToUpper()}, including a brief description of what the function does, its parameters, and its return value.");
            sb.AppendLine("Make sure to include any necessary tags, such as <param>, <exceptions> and <returns>, to provide additional information about each part of the function.");
            sb.AppendLine("IMPORTANT: Please provide ONLY THE XML documentation comment.");

            sb.AppendLine("Function:");
            sb.AppendLine("````");
            sb.AppendLine(function);
            sb.AppendLine("````");

            var chatCompletion = new ChatCompletion
            {
                Request = new ChatCompletionRequest
                {
                    Model = "gpt-3.5-turbo",
                    MaxTokens = 2048,
                    Temperature = 0.2D,
                    Messages = new ChatCompletionMessage[]
                    {
                        new ChatCompletionMessage
                        {
                            Content = sb.ToString(),
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
