using Luval.AuthMate.Infrastructure.Logging;
using Luval.GenAIBotMate.Infrastructure.Configuration;
using Luval.GenAIBotMate.Infrastructure.Data;
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

            //TODO: Add secrests for the OpenAI key, Azure Blob Storage connection string
            builder.Services.AddGenAIBotServicesDefault(
                config.GetValue<string>("OpenAIKey") ?? throw new ArgumentNullException("OpenAIKey not found in secrets.json"),
                config.GetValue<string>("AzureStorageConnString") ?? throw new ArgumentNullException("AzureStorageConnString not found in secrets.json")
            );

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
            var dbHelper = new GenAIBotContextHelper(context, new ColorConsoleLogger<GenAIBotContextHelper>());
            dbHelper.InitializeAsync()
                .GetAwaiter()
                .GetResult();

            app.Run();
        }
    }
}
