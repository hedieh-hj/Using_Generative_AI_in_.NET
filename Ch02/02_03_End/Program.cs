using Azure.AI.OpenAI;

string key = Environment.GetEnvironmentVariable("OPENAI_API_KEY");
var client = new OpenAIClient(key);

string deploymentName = "gpt-3.5-turbo";