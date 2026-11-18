using System.Text;
using Azure.AI.OpenAI;

string? key = Environment.GetEnvironmentVariable("OPENAI_API_KEY");

if (string.IsNullOrEmpty(key))
{
    Console.WriteLine("Please set the OPENAI_API_KEY environment variable.");
    return;
}

var client = new OpenAIClient(key);

string deploymentName = "gpt-4";

ChatCompletionsOptions completionsOptions = new();
completionsOptions.DeploymentName = deploymentName;
completionsOptions.MaxTokens = 5;

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
        var streamingResult = client.GetChatCompletionsStreamingAsync(completionsOptions);
        StringBuilder answerBuilder = new();
        CompletionsFinishReason? lastReason = null;

        await foreach (var update in streamingResult.Result.EnumerateValues())
        {
            Console.Write(update.ContentUpdate);
            answerBuilder.Append(update.ContentUpdate);
            lastReason = update.FinishReason;
        }

        if (lastReason != CompletionsFinishReason.Stopped)
        {
            Console.WriteLine($"\nChat completion didn't finish normally." +
                              $" Reason: {lastReason}");

            break;
        }

        var answer = answerBuilder.ToString();

        completionsOptions.Messages.Add(
            new ChatRequestAssistantMessage(answer));

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