namespace Cs.Gpt
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using OpenAI.ObjectModels.RequestModels;

    public sealed class GptTranslator : GptClient
    {
        public GptTranslator(string apiKey) : base(apiKey)
        {
        }

        public enum TranslateMode
        {
            ToEnglish,
            ToChinese,
            ToJapanese,
        }

        public Task<string> Translate(TranslateMode mode, string input)
        {
            var request = this.CreateRequest(mode, input);
            return this.GetResponse(request);
        }

        public Task<string> ReTranslate(TranslateMode mode, string input, string firstResponse)
        {
            var languageName = ToLanguageName(mode);
          
            var request = this.CreateRequest(mode, input);
            request.Messages.Add(ChatMessage.FromAssistant(firstResponse));
            request.Messages.Add(ChatMessage.FromSystem($"You must translate it into {languageName}, not into English."));
            request.Messages.Add(ChatMessage.FromUser(input));
            return this.GetResponse(request);
        }

        //// -----------------------------------------------------------------------------------------------

        private static string ToLanguageName(TranslateMode mode)
        {
            return mode switch
            {
                TranslateMode.ToEnglish => "English",
                TranslateMode.ToChinese => "Chinese",
                TranslateMode.ToJapanese => "Japanese",
                _ => throw new NotImplementedException(),
            };
        }

        private ChatCompletionCreateRequest CreateRequest(TranslateMode mode, string input)
        {
            var languageName = ToLanguageName(mode);
            string roleContent;
          
            // 원문에 컬러키가 있다면 누락하지 말 것을 명시한다.
            if (input.Contains("<color=#"))
            {
                roleContent = $"You are a {languageName} translator who translate user input. Just tell the translation for user input, no further explanation. If there is a color tag in the format '<color=#ffffff>' in the original text, do not omit it and keep it as is as much as possible.";
            }
            else
            {
                roleContent = $"You are a {languageName} translator who translate user input. Just tell the translation for user input, no further explanation.";
            }

            // 영문 번역 이외의 경우, 영어로 번역하지 말 것을 강조한다. 
            if (mode != TranslateMode.ToEnglish)
            {
                roleContent += $" You must translate it into {languageName}, not into English.";
            }

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
