namespace Cs.Gpt;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cs.Logging;
using OpenAI;
using OpenAI.Managers;
using OpenAI.ObjectModels.RequestModels;

public sealed class GptClient : IDisposable
{
    private readonly OpenAIService service;
    private readonly ServiceMode serviceMode;

    public GptClient(string apiKey, ServiceMode mode)
    {
        this.serviceMode = mode;
     
        var options = new OpenAiOptions { ApiKey = apiKey };
        this.service = new OpenAIService(options);
    }

    public enum ServiceMode
    {
        TranslateToEnglish,
        TranslateToChinese,
    }

    public void Dispose()
    {
        this.service.Dispose();
    }

    public async Task<string> GetResponse(string input)
    {
        var roleContent = this.serviceMode switch
        {
            ServiceMode.TranslateToEnglish => "You are a English translator who translate user input. Just tell the translation for user input, no further explanation.",
            ServiceMode.TranslateToChinese => "You are a Chinese translator who translate user input. Just tell the translation for user input, no further explanation.",
            _ => throw new NotImplementedException(),
        };

        var request = new ChatCompletionCreateRequest
        {
            Model = OpenAI.ObjectModels.Models.Gpt_3_5_Turbo_0125,
            Messages = new List<ChatMessage>
            {
                ChatMessage.FromSystem(roleContent),
                ChatMessage.FromUser(input),
            },
        };

        var result = await this.service.ChatCompletion.CreateCompletion(request);

        if (result.Error is not null)
        {
            var error = result.Error;
            Log.Error($"errorCode:{error.Code}");
            Log.Error($"errorType:{error.Type}");
            Log.Error($"errorMessage:{error.Message}");
            return $"Error: 오류가 발생했습니다";
        }

        return result.Choices[0].Message.Content ?? "(empty contents)";
    }
}
