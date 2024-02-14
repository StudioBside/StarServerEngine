namespace Cs.Gpt
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using OpenAI.ObjectModels.RequestModels;

    public sealed class GptTranslator : GptClient
    {
        private readonly ServiceMode serviceMode;

        public GptTranslator(string apiKey, ServiceMode mode) : base(apiKey)
        {
            this.serviceMode = mode;
        }

        public enum ServiceMode
        {
            TranslateToEnglish,
            TranslateToChinese,
        }

        public Task<string> GetResponse(string input)
        {
            var request = this.CreateRequest(input);
            return this.GetResponse(request);
        }

        public Task<string> GetResponse(string input, string firstResponse)
        {
            var request = this.CreateRequest(input);
            request.Messages.Add(ChatMessage.FromAssistant(firstResponse));
            request.Messages.Add(ChatMessage.FromSystem("You must translate it into Chinese, not into English."));
            request.Messages.Add(ChatMessage.FromUser(input));
            return this.GetResponse(request);
        }

        //// -----------------------------------------------------------------------------------------------

        private ChatCompletionCreateRequest CreateRequest(string input)
        {
            string systemMessage;
          
            // 원문에 컬러키가 있다면 누락하지 말 것을 명시한다.
            if (input.Contains("<color=#"))
            {
                systemMessage = "You are a {0} translator who translate user input. Just tell the translation for user input, no further explanation. If there is a color tag in the format '<color=#ffffff>' in the original text, do not omit it and keep it as is as much as possible.";
            }
            else
            {
                systemMessage = "You are a {0} translator who translate user input. Just tell the translation for user input, no further explanation.";
            }

            // 중문 번역인 경우, 영어로 번역하지 말 것을 강조한다. 
            if (this.serviceMode == ServiceMode.TranslateToChinese)
            {
                systemMessage += " You must translate it into Chinese, not into English.";
            }

            var roleContent = this.serviceMode switch
            {
                ServiceMode.TranslateToEnglish => string.Format(systemMessage, "English"),
                ServiceMode.TranslateToChinese => string.Format(systemMessage, "Chinese"),
                _ => throw new NotImplementedException(),
            };

            return new ChatCompletionCreateRequest
            {
                Model = OpenAI.ObjectModels.Models.Gpt_3_5_Turbo_0125,
                Messages = new List<ChatMessage>
                {
                    ChatMessage.FromSystem(roleContent),
                    ChatMessage.FromUser(input),
                },
            };
        }
    }
}
