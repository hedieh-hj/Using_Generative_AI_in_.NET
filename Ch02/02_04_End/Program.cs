using Azure.AI.OpenAI;

string key = Environment.GetEnvironmentVariable("OPENAI_API_KEY");
var client = new OpenAIClient(key);

string deploymentName = "gpt-3.5-turbo";

ChatCompletionsOptions completionsOptions = new();
completionsOptions.DeploymentName = deploymentName;

ChatRequestSystemMessage systemMsg =
    new("You are an annoyingly friendly AI " +
        "chatbot at the Azure Saturday AI " +
        "conference in Seattle. Be brief.");
completionsOptions.Messages.Add(systemMsg);

var result = client.GetChatCompletions(completionsOptions);

var firstChoice = result.Value.Choices.FirstOrDefault();

Console.WriteLine(firstChoice.Message.Content);
