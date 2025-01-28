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
using Luval.SemanticModel.PlugIns;

namespace Luval.SemanticModel
{
    public class DatabaseAgent : GenAIBotService
    {
        private static Kernel _kernel = default!;

        public DatabaseAgent(ILoggerFactory loggerFactory, IConfiguration configuration, IGenAIBotStorageService storageService, IMediaService mediaService)
            : base(GetChatCompletions(configuration,loggerFactory),
                    storageService, mediaService, loggerFactory.CreateLogger<GenAIBotService>())
        {
            SetKernel(_kernel);
        }

        private static IChatCompletionService GetChatCompletions(IConfiguration configuration, ILoggerFactory loggerFactory)
        {
            var builder = Kernel.CreateBuilder().AddOpenAIChatCompletion("gpt-4o", configuration["OpenAIKey"] ?? throw new InvalidOperationException("OpenAIKey setting not provided in configuration"));
            builder.Plugins.AddFromObject(new SqlServerPlugIn(configuration, loggerFactory.CreateLogger("App")), "DatabasePlugIn");
            var kernel = builder.Build();
            _kernel = kernel;
            return kernel.GetRequiredService<IChatCompletionService>();
        }

        public static string GetSystemPrompt()
        {
            var catalog = GetCatalog();
            var p = File.ReadAllText(".\\Prompts\\system-prompt-1.txt");
            var sp = p.Replace("<<yaml>>", catalog.ToYAML());
            return sp;
        }

        protected static Catalog GetCatalog()
        {
            return JsonSerializer.Deserialize<Catalog>(File.ReadAllText("full-catalog.json")) ?? 
                throw new InvalidOperationException("Unable to extract catalog");
        }
    }
}
