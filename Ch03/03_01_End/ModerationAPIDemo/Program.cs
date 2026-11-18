using OpenAI;
using OpenAI.Chat;
using OpenAI.Moderations;

OpenAIClient client = new();

Message sysPrompt = new(Role.System, 
    "You are an AI Assistant at the Seattle AI Saturdays event.");

List<Message> history = new() {sysPrompt};

while (true)
{
    // User
    Console.ForegroundColor = ConsoleColor.Yellow;
    var userInput = Console.ReadLine();

    ModerationsRequest moderationsRequest = new(userInput);
    var moderationResult = (await client.ModerationsEndpoint.CreateModerationAsync(
        moderationsRequest)).Results[0];

    if (moderationResult.Flagged)
    {
        Console.WriteLine($"Your message was flagged. " +
                          $"Scores: {moderationResult.Scores}");

        var moderationMsg = new Message(Role.System,
            "The content moderation system has flagged the user's answer. " +
            "Here is the moderation system's output. React accordingly " +
            "to the user, explicitly mentioning that the user's specific " +
            "violation will not be tolerated. \n\n"
            + moderationResult.Scores);
        history.Add(moderationMsg);
    }
    else
    {
        history.Add(new Message(Role.User, userInput));
    }

    //Assistant
    var result = await client.ChatEndpoint.GetCompletionAsync(
        new ChatRequest(history, model: "gpt-4-turbo"));

    var msg = result.FirstChoice.Message;
    history.Add(msg);
    Console.ForegroundColor = ConsoleColor.White;
    Console.WriteLine(msg.Content);
}
