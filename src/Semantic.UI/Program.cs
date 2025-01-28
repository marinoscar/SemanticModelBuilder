using Luval.AuthMate.Infrastructure.Logging;
using Luval.GenAIBotMate.Infrastructure.Configuration;
using Luval.GenAIBotMate.Infrastructure.Data;
using Luval.GenAIBotMate.Infrastructure.Interfaces;
using Luval.SemanticModel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.FluentUI.AspNetCore.Components;
using Semantic.UI.Components;

namespace Semantic.UI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var config = builder.Configuration;

            // Add services to the container.
            builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents();
            builder.Services.AddFluentUIComponents();

            builder.Services.AddLogging();

            //TODO: Add secrests for the OpenAI key, Azure Blob Storage connection string
            builder.Services.AddGenAIBotServicesDefault(
                config.GetValue<string>("OpenAIKey") ?? throw new ArgumentNullException("OpenAIKey not found in secrets.json"),
                config.GetValue<string>("AzureStorageConnString") ?? throw new ArgumentNullException("AzureStorageConnString not found in secrets.json")
            );

            builder.Services.AddScoped<DatabaseAgent>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseAntiforgery();

            app.MapStaticAssets();
            app.MapRazorComponents<App>()
                .AddInteractiveServerRenderMode();

            // Initialize the database
            var context = new SqliteChatDbContext();
            var dbHelper = new GenAIBotContextHelper(context, new Luval.AuthMate.Infrastructure.Logging.ColorConsoleLogger<GenAIBotContextHelper>());
            dbHelper.InitializeAsync()
                .GetAwaiter()
                .GetResult();

            // Check if the bot has a system prompt
            CheckBot(context);

            app.Run();
        }

        private static void CheckBot(IChatDbContext context)
        {
            var bot = context.GenAIBots.FirstOrDefault();
            if (bot == null) return;
            if (string.IsNullOrEmpty(bot.SystemPrompt))
            {
                bot.SystemPrompt = DatabaseAgent.GetSystemPrompt();
                bot.UtcUpdatedOn = DateTime.UtcNow;
                bot.UpdatedBy = "System";
                bot.Version++;
                context.SaveChanges();
            }
        }
    }
}
