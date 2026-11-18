using System.Text;
using Azure.AI.OpenAI;

Console.OutputEncoding = Encoding.UTF8;

string key = Environment.GetEnvironmentVariable("OPENAI_API_KEY");

if (string.IsNullOrEmpty(key))
{
    Console.WriteLine("Please set the OPENAI_API_KEY environment variable.");
    return;
}

var client = new OpenAIClient(key);
string deploymentName = "gpt-4o";

ChatCompletionsOptions completionOptions = new();
completionOptions.DeploymentName = deploymentName;
//completionOptions.MaxTokens = 5;

// System Message
ChatRequestSystemMessage msg =
    new("You are an annoyingly friendly AI chatbot at the Azure Saturday Conference in Seattle. Be brief.");

completionOptions.Messages.Add(msg);

try
{
    while (true)
    {
        // AI's answer
        var streamingResult = client.GetChatCompletionsStreamingAsync(completionOptions);
        StringBuilder answerBuilder = new();
        CompletionsFinishReason? lastReason = null;

        Console.ForegroundColor = ConsoleColor.White;

        await foreach (var update in streamingResult.Result.EnumerateValues())
        {
            Console.Write(update.ContentUpdate);
            answerBuilder.Append(update.ContentUpdate);
            lastReason = update.FinishReason;
        }

        Console.WriteLine();
        Console.WriteLine();

        if (lastReason != CompletionsFinishReason.Stopped)
        {
            Console.WriteLine($"Chat completion didn't finish normally. Reason: {lastReason}");
            break;
        }
        
        var answer = answerBuilder.ToString();
        completionOptions.Messages.Add(new ChatRequestAssistantMessage(answer));

        // User's input
        Console.ForegroundColor = ConsoleColor.Yellow;
        var userInput = Console.ReadLine();

        ChatRequestUserMessage userMessage = new(userInput);
        completionOptions.Messages.Add(userMessage);
    }
}

catch (Exception ex)
{
    Console.WriteLine($"An error occured: {ex.Message}");
}