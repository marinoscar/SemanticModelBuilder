using Luval.SemanticModel.Entities;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.Text.Json;
using Luval.GenAIBotMate.Core.Services;
using Luval.GenAIBotMate.Infrastructure.Interfaces;

namespace Luval.SemanticModel
{
    public class DatabaseAgent : GenAIBotService
    {
        private Kernel _kernel = default!;
        private readonly IChatCompletionService _chatCompletionService;
        private readonly OpenAIPromptExecutionSettings _settings;
        private readonly ILogger _logger;
        private ChatHistory _chatHistory;
        private readonly Catalog _catalog;

        public DatabaseAgent(ILoggerFactory loggerFactory, IConfiguration configuration, IGenAIBotStorageService storageService, IMediaService mediaService)
        {
            _logger = loggerFactory.CreateLogger("App");
            var builder = Kernel.CreateBuilder().AddOpenAIChatCompletion("gpt-4o", configuration["OpenAIKey"] ?? throw new InvalidOperationException("OpenAIKey setting not provided in configuration"));
            _kernel = builder.Build();
            _chatCompletionService = _kernel.GetRequiredService<IChatCompletionService>();
            _settings = new OpenAIPromptExecutionSettings()
            {
                Temperature = 0
            };
            _catalog = GetCatalog();
            _chatHistory = new ChatHistory();

            _chatHistory.AddSystemMessage(GetSystemPrompt());
        }

        private static IChatCompletionService GetChatCompletions(IConfiguration configuration, ILoggerFactory loggerFactory)
        {
            var builder = Kernel.CreateBuilder().AddOpenAIChatCompletion("gpt-4o", configuration["OpenAIKey"] ?? throw new InvalidOperationException("OpenAIKey setting not provided in configuration"));
            builder.Plugins.AddFromObject(plugIn, "DatabasePlugIn");
            var kernel = builder.Build();
            _kernel = kernel;
            return kernel.GetRequiredService<IChatCompletionService>();
        }

        protected virtual string GetSystemPrompt()
        {
            var p = File.ReadAllText(".\\Prompts\\system-prompt-1.txt");
            var sp = p.Replace("<<yaml>>", _catalog.ToYAML());
            return sp;
        }

        protected virtual Catalog GetCatalog()
        {
            return JsonSerializer.Deserialize<Catalog>(File.ReadAllText("full-catalog.json")) ?? 
                throw new InvalidOperationException("Unable to extract catalog");
        }
    }

    public class DatabaseAgentConfiguration
    {
    }
}
