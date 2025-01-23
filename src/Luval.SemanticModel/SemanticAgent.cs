using Luval.SemanticModel.Entities;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Luval.SemanticModel
{
    public class SemanticAgent
    {

        private readonly Kernel _kernel;
        private readonly IChatCompletionService _chatCompletionService;
        private readonly OpenAIPromptExecutionSettings _settings;
        private readonly ILogger _logger;

        public SemanticAgent() : this(new ColorConsoleLogger())
        {
            
        }

        public SemanticAgent(ILogger logger)
        {
            _logger = logger;
            var builder = Kernel.CreateBuilder().AddOpenAIChatCompletion("gpt-4o", GetKey());
            _kernel = builder.Build();
            _chatCompletionService = _kernel.GetRequiredService<IChatCompletionService>();
            _settings = new OpenAIPromptExecutionSettings()
            {
                Temperature = 0
            };
        }

        public async Task UpdateSemanticModel(Catalog catalog)
        {
            _logger.LogInformation("Starting UpdateSemanticModel for catalog {CatalogName}", catalog.CatalogName);
            var totalCount = catalog.Tables.Count;
            foreach (var table in catalog.Tables)
            {
                var index = catalog.Tables.IndexOf(table);
                var working = true;
                var retries = 0;
                while (working)
                {
                    try
                    {
                        _logger.LogInformation("Updating semantic model for table {TableName} {index} of {totalCount}", table.QualifiedName, index + 1, totalCount);
                        await UpdateTableSemanticModel(catalog, table);
                        working = false;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Error updating table {TableName}, retrying... attempt {RetryCount}", table.QualifiedName, retries + 1);
                        Task.Delay(1000).Wait();
                        retries++;
                        if (retries > 3)
                        {
                            _logger.LogError(ex, "Error updating table {TableName} after {RetryCount} attempts", table.QualifiedName, retries);
                            working = false;
                        }
                    }
                }
            }
            File.WriteAllText("full-catalog.json", catalog.ToPrettyJson());
            _logger.LogInformation("Completed UpdateSemanticModel for catalog {CatalogName}", catalog.CatalogName);
        }

        private async Task UpdateTableSemanticModel(Catalog catalog, Table table)
        {
            _logger.LogInformation("Creating catalog for inspection for table {TableName}", table.QualifiedName);
            var newCatalog = CreateCatalogForInspection(catalog, table);
            var prompt = GetPrompt(newCatalog, table);
            _logger.LogInformation("Generated prompt for table {TableName}", table.QualifiedName);
            var completions = await _chatCompletionService.GetChatMessageContentsAsync(prompt, _settings, _kernel);
            var json = GetJsonResult(completions);
            _logger.LogInformation("Received JSON result for table {TableName}", table.QualifiedName);
            MergeTable(table, ParseJson(json));
            _logger.LogInformation("Merged AI results into table {TableName}", table.QualifiedName);
        }


        private Table ParseJson(string json)
        {
            var jsonTable = JsonSerializer.Deserialize<Table>(json);
            var obj = JsonObject.Parse(json);
            var table = new Table() {
                SemanticName = Convert.ToString(obj["Table"]["SemanticName"]),
                SemanticDescription = Convert.ToString(obj["Table"]["SemanticDescription"]),
                Columns = jsonTable.Columns
            };
            return table;
        }

        private void MergeTable(Table table, Table aiTable)
        {
            table.SemanticName = aiTable.SemanticName;
            table.SemanticDescription = aiTable.SemanticDescription;
            foreach (var c in table.Columns)
            {
                c.SemanticName = aiTable.Columns.FirstOrDefault(i => i.ColumnName == c.ColumnName)?.SemanticName;
                c.SemanticDescription = aiTable.Columns.FirstOrDefault(i => i.ColumnName == c.ColumnName)?.SemanticDescription;
                c.ColumnType = aiTable.Columns.FirstOrDefault(i => i.ColumnName == c.ColumnName)?.ColumnType;
                c.DefaultAggreation = aiTable.Columns.FirstOrDefault(i => i.ColumnName == c.ColumnName)?.DefaultAggreation;
            }
        }

        private Catalog CreateCatalogForInspection(Catalog fullCatalog, Table table)
        {
            var catalog = new Catalog()
            {
                CatalogName = fullCatalog.CatalogName,
                SqlEngine = fullCatalog.SqlEngine
            };
            catalog.Tables.Add(table);
            foreach (var r in table.References)
            {
                var childTable = fullCatalog.Tables.FirstOrDefault(i => i.QualifiedName == r.ChildTableQualifiedName);
                if(childTable != null) catalog.Tables.Add(childTable);
            }
            return catalog;
        }

        protected virtual string GetKey()
        {
            return Environment.GetEnvironmentVariable("OpenAIKey", EnvironmentVariableTarget.User) ?? "";
        }

        private string[] ExtractJsonBlocks(string input)
        {
            string pattern = @"```json\s*([\s\S]*?)\s*```";
            MatchCollection matches = Regex.Matches(input, pattern, RegexOptions.IgnoreCase);

            string[] results = new string[matches.Count];
            for (int i = 0; i < matches.Count; i++)
            {
                results[i] = matches[i].Groups[1].Value.Trim();
            }

            return results;
        }

        private string GetJsonResult(IEnumerable<ChatMessageContent> completions)
        {
            var content = new StringBuilder();
            foreach (var c in completions)
            {
                foreach (var i in c.Items)
                {
                    content.Append(i.ToString());
                }
            }
            var blocks = ExtractJsonBlocks(content.ToString());
            if(blocks == null || !blocks.Any()) return default!;
            return blocks[0];
        }

        private string GetPrompt(Catalog catalog, Table table)
        {
            var p = File.ReadAllText(".\\Prompts\\prompt-1.txt");
            return p.Replace("<<table>>", table.QualifiedName).Replace("<<json>>", catalog.ToJson());
        }
    }
}
