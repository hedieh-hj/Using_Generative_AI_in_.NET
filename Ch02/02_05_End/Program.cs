using Azure.AI.OpenAI;

string key = Environment.GetEnvironmentVariable("OPENAI_API_KEY");
var client = new OpenAIClient(key);

string deploymentName = "gpt-3.5-turbo";

ChatCompletionsOptions completionsOptions = new();
completionsOptions.DeploymentName = deploymentName;

// System message
ChatRequestSystemMessage systemMsg =
    new("You are an annoyingly friendly AI " +
        "chatbot at the Azure Saturday AI " +
        "conference in Seattle. Be brief.");
completionsOptions.Messages.Add(systemMsg);

while (true)
{
    // AI's answer
    Console.ForegroundColor = ConsoleColor.White;
    var result = client.GetChatCompletions(completionsOptions);
    var firstChoice = result.Value.Choices.FirstOrDefault();
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