using System.Text;
using Standard.AI.OpenAI.Clients.OpenAIs;
using Standard.AI.OpenAI.Models.Configurations;
using Standard.AI.OpenAI.Models.Services.Foundations.ChatCompletions;

namespace AI_XML_Doc.Helpers
{
    public class OaiHelper
    {
        private readonly OpenAIClient _openAIClient;

        /// <summary>
        /// Initializes a new instance of the OaiHelper class with the specified API key.
        /// </summary>
        /// <param name="apiKey">The API key to use for authentication.</param>
        /// <returns>A new instance of the OaiHelper class.</returns>
        /// <exception cref="ArgumentNullException">Thrown when apiKey is null or empty.</exception>
        public OaiHelper(string apiKey)
        {
            var openAIConfigurations = new OpenAIConfigurations()
            {
                ApiKey = apiKey
            };

            _openAIClient = new OpenAIClient(openAIConfigurations);
        }

        /// <summary>
        /// Generates an XML documentation comment for a given function in the specified language, or English by default. The comment includes a brief description of what the function does, its parameters, and its return value. The comment also provides guidance on including necessary tags such as <param>, <exceptions>, and <returns> to provide additional information about each part of the function.
        /// </summary>
        /// <param name="function">The function for which to generate the XML documentation comment.</param>
        /// <param name="language">The language in which to generate the XML documentation comment. If null, English will be used by default.</param>
        /// <returns>The generated XML documentation comment as a string.</returns>
        public async ValueTask<string> GenerateXmlDocComment(string function, string? language)
        {
            var sb = new StringBuilder();
            sb.AppendLine("Assume you are building a large project in C#.");
            sb.AppendLine("You have been asked to provide XML documentation comments for each function to help other developers understand how to use them.");
            sb.AppendLine($"Given the function that I will further provide you, write the XML documentation comment for it IN {(language ?? "english").ToUpper()}, including a brief description of what the function does, its parameters, and its return value.");
            sb.AppendLine("Make sure to include any necessary tags, such as <param> and <returns> to provide additional information about each part of the function.");
            sb.AppendLine("You do not need to add a <returns> tag if the function returns void.");
            sb.AppendLine("If the function interacts with other types, methods, or properties, use the <see /> tag to reference them, helping developers understand the relationship between them.");
            sb.AppendLine("Include a <remarks> tag only if there are any additional details or considerations that are not obvious to the developers reading the comment or the code.");
            sb.AppendLine("IMPORTANT: Please return ONLY THE XML documentation comment and NOTHING MORE.");
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
