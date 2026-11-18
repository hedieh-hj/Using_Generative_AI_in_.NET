using Azure;
using Azure.AI.OpenAI;

/*string? key = Environment.GetEnvironmentVariable("OPENAI_API_KEY");

if (string.IsNullOrEmpty(key))
{
    Console.WriteLine("Please set the OPENAI_API_KEY environment variable.");
    return;
}

var client = new OpenAIClient(key);

string deploymentName = "gpt-3.5-turbo";*/

var endPoint = 
    Environment.GetEnvironmentVariable("AZURE_OPENAI_API_ENDPOINT");

var apiKey =
    Environment.GetEnvironmentVariable("AZURE_OPENAI_API_KEY");

var deploymentName = "gpt-35-turbo";

var credential = new AzureKeyCredential(apiKey);

var client = new OpenAIClient(new Uri(endPoint), credential);

ChatCompletionsOptions completionsOptions = new();
completionsOptions.DeploymentName = deploymentName;

// System message
ChatRequestSystemMessage systemMsg =
    new("You are an annoyingly friendly AI " +
        "chatbot at the Azure Saturday AI " +
        "conference in Seattle. Be brief.");
completionsOptions.Messages.Add(systemMsg);

try
{
    while (true)
    {
        // AI's answer
        Console.ForegroundColor = ConsoleColor.White;
        var result = client.GetChatCompletions(completionsOptions);
        var firstChoice = result?.Value?.Choices?.FirstOrDefault();

        if (firstChoice == null)
        {
            Console.WriteLine("No chat completion choices available.");
            break;
        }

        if (firstChoice.FinishReason != CompletionsFinishReason.Stopped)
        {
            Console.WriteLine($"Chat completion didn't finish normally." +
                              $"Reason: {firstChoice.FinishReason}");

            break;
        }

        var answer = firstChoice.Message.Content;

        completionsOptions.Messages.Add(
            new ChatRequestAssistantMessage(answer));

        Console.WriteLine(answer);
        Console.WriteLine();

        // User's Input
        Console.ForegroundColor = ConsoleColor.Yellow;
        var userInput = Console.ReadLine();
        ChatRequestUserMessage userMessage = new(userInput);
        completionsOptions.Messages.Add(userMessage);
    }
}
catch (Exception ex)
{
    Console.WriteLine($"An error has occured:\n{ex.Message}");
}