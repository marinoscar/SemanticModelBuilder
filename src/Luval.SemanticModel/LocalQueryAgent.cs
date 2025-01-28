using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Luval.SemanticModel.Entities;

namespace Luval.SemanticModel
{
    public class LocalQueryAgent
    {
        private readonly Kernel _kernel;
        private readonly IChatCompletionService _chatCompletionService;
        private readonly OpenAIPromptExecutionSettings _settings;
        private readonly ILogger _logger;
        private ChatHistory _chatHistory;
        private readonly Catalog _catalog;


        public LocalQueryAgent(ILogger logger, Catalog catalog)
        {
            _logger = logger;
            var builder = Kernel.CreateBuilder().AddOpenAIChatCompletion("gpt-4o", GetKey());
            _kernel = builder.Build();
            _chatCompletionService = _kernel.GetRequiredService<IChatCompletionService>();
            _settings = new OpenAIPromptExecutionSettings()
            {
                Temperature = 0
            };
            _catalog = catalog;
            _chatHistory = new ChatHistory();

            _chatHistory.AddSystemMessage(GetSystemPrompt());

        }

        protected virtual string GetKey()
        {
            return Environment.GetEnvironmentVariable("OpenAIKey", EnvironmentVariableTarget.User) ?? "";
        }

        protected virtual string GetSystemPrompt()
        {
            var p = File.ReadAllText(".\\Prompts\\system-prompt-1.txt");
            var sp = p.Replace("<<yaml>>", _catalog.ToYAML());
            return sp;
        }

        public string AddUserMessage(string text)
        {
            _chatHistory.AddUserMessage(text);
            var content = _chatCompletionService.GetChatMessageContentAsync(_chatHistory, _settings, _kernel).GetAwaiter().GetResult();
            return GetResponse(content);
        }

        public string GetResponse(ChatMessageContent content)
        {
            var sb = new StringBuilder();
            foreach (var item in content.Items)
            {
                sb.Append(item.ToString());
            }
            return sb.ToString();
        }


    }
}
