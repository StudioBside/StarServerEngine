namespace Cs.Gpt;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cs.Logging;
using OpenAI.ObjectModels;
using OpenAI.ObjectModels.RequestModels;
using OpenAI.ObjectModels.ResponseModels;

public sealed class GptTranslator(string apiKey) : GptClient(apiKey)
{
    private long apiCallCount;
    private string modelName = Models.Gpt_3_5_Turbo_0125;

    public enum TranslationMode
    {
        ToEnglish,
        ToChinese,
        ToJapanese,
    }

    public long ApiCallCount => this.apiCallCount;

    public static bool ValidateResult(string source, string translated, TranslationMode mode)
    {
        if (string.IsNullOrEmpty(translated))
        {
            return false;
        }

        if (source.Contains('\t') == false && translated.Contains('\t'))
        {
            Log.Warn($"번역 결과에 탭문자 존재. mode:{mode} translated:{translated}");
            return false;
        }

        if (source.All(e => e == '…') && translated.Any(e => e != '…'))
        {
            return false;
        }

        if (translated.Any(IsKoreanCompleted))
        {
            return false;
        }

        switch (mode)
        {
            case TranslationMode.ToEnglish:
                if (IsSymbolNumeric(source) && HasAlphabet(translated))
                {
                    return false;
                }

                break;

            case TranslationMode.ToChinese:
                if (IsAlphaNumeric(source) == false && // 원본이 영문이 아닌데
                    string.IsNullOrEmpty(translated) == false && // 번역문을 제대로 받아왔고
                    IsAlphaNumeric(translated)) // 번역문이 영문이라면
                {
                    return false;
                }

                if (translated.Contains("translate", StringComparison.CurrentCultureIgnoreCase) ||
                    translated.Contains("translation", StringComparison.CurrentCultureIgnoreCase) ||
                    translated.Contains("Sorry,", StringComparison.CurrentCultureIgnoreCase))
                {
                    return false;
                }

                if (source.Length < 5 &&
                    translated.Length > 10 &&
                    translated.StartsWith("抱歉，", StringComparison.CurrentCultureIgnoreCase))
                {
                    return false;
                }

                break;
        }

        return true;

        static bool IsKoreanCompleted(char code)
        {
            int bottom = 0xAC00;
            int top = 0xD7A3;
            return bottom <= code && code <= top; // 완성된 한글
        }
    }

    public Task<string> Translate(TranslationMode mode, string sourceText)
    {
        var request = CreateRequest(mode, sourceText, this.modelName);
        return this.GetResponse(request);
    }

    public async Task<SingleProcessResult> Translate(TranslationMode mode, string sourceText, string translatedOld)
    {
        // 예전 번역이 존재하는 경우
        if (string.IsNullOrEmpty(translatedOld) == false)
        {
            Log.Warn($"예전의 번역 결과가 유효하지 않습니다. translated:{translatedOld}");
        }

        string translatedNew;
        var translated = true;
        // 영문 번역일 때 원문이 이미 영문숫자라면 스킵한다.
        if (mode == TranslationMode.ToEnglish && IsAlphaNumeric(sourceText))
        {
            Log.Info($"mode:{mode} - 영문/숫자만 포함되어 번역을 생략하고 원문을 그대로 사용합니다.");
            translatedNew = sourceText;
        }
        else if (IsSymbolNumeric(sourceText))
        {
            Log.Info($"mode:{mode} - 특수문자/숫자만 포함되어 번역을 생략하고 원문을 그대로 사용합니다.");
            translatedNew = sourceText;
        }
        else
        {
            translatedNew = await this.Translate(mode, sourceText);
            ++this.apiCallCount;
        }

        if (ValidateResult(sourceText, translatedNew, mode) == false)
        {
            Log.Warn($"번역 결과가 유효하지 않아 재번역 합니다. mode:{mode} translated:{translatedNew}");
            translatedNew = await this.ReTranslate(mode, sourceText, translatedNew);
            ++this.apiCallCount;

            if (ValidateResult(sourceText, translatedNew, mode) == false)
            {
                Log.Warn($"2회차 번역 결과도 유효하지 않습니다. mode:{mode} translated:{translatedNew}");
                translatedNew = string.Empty;
                translated = false;
            }
        }

        return new SingleProcessResult(translated, translatedNew);
    }

    public async Task<SingleProcessResult> RetranslateWithMultiInput(
        TranslationMode mode,
        string sourceKor,
        string sourceEng,
        string translatedOld)
    {
        var request = CreateRequest(mode, sourceKor, sourceEng, this.modelName);
        var translatedNew = await this.GetResponse(request);
        ++this.apiCallCount;
        var translated = true;

        if (ValidateResult(sourceKor, translatedNew, mode) == false)
        {
            Log.Warn($"3회차 번역 결과도 유효하지 않습니다. mode:{mode} translated:{translatedNew}");
            translatedNew = string.Empty;
            translated = false;
        }
        else
        {
            Log.Info($"3회차 번역 성공. mode:{mode} text:{translatedOld} -> {translatedNew}");
        }

        return new SingleProcessResult(translated, translatedNew);
    }

    public void ResetApiCallCount()
    {
        this.apiCallCount = 0;
    }

    //// -----------------------------------------------------------------------------------------------

    protected override void OnError(Error error)
    {
        if (error.Code == LimitExceedErrorCode && this.modelName == Models.Gpt_3_5_Turbo_0125)
        {
            var prevName = this.modelName;
            this.modelName = Models.Gpt_3_5_Turbo_1106;
            Log.Warn($"API 호출 제한을 초과해 ai model을 변경합니다. {prevName} -> {this.modelName}");

            // 기억했던 에러코드를 삭제하고 계속 처리를 진행하게 만든다.
            this.LastErrorCode = string.Empty;
        }
    }

    private static string ToLanguageName(TranslationMode mode)
    {
        return mode switch
        {
            TranslationMode.ToEnglish => "English",
            TranslationMode.ToChinese => "Chinese",
            TranslationMode.ToJapanese => "Japanese",
            _ => throw new NotImplementedException(),
        };
    }

    private static bool IsAlphaNumeric(string text)
    {
        return text.All(c => char.IsAscii(c) || char.IsWhiteSpace(c) || c == '…');
    }

    private static bool IsSymbolNumeric(string text)
    {
        return text.All(c => (uint)c <= '\x0040' || char.IsWhiteSpace(c) || c == '…');
    }

    private static bool HasAlphabet(string text)
    {
        return text.Any(c => char.IsLetter(c));
    }

    private static ChatCompletionCreateRequest CreateRequest(TranslationMode mode, string input, string modelName)
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
        if (mode != TranslationMode.ToEnglish)
        {
            roleContent += $" You must translate it into {languageName}, not into English.";
        }

        return new ChatCompletionCreateRequest
        {
            Model = modelName,
            Messages = new List<ChatMessage>
            {
                ChatMessage.FromSystem(roleContent),
                ChatMessage.FromUser(input),
            },
        };
    }

    private static ChatCompletionCreateRequest CreateRequest(TranslationMode mode, string sourceKor, string sourceEng, string modelName)
    {
        var languageName = ToLanguageName(mode);
        string roleContent;

        // 원문에 컬러키가 있다면 누락하지 말 것을 명시한다.
        if (sourceKor.Contains("<color=#"))
        {
            roleContent = $"You are a {languageName} translator who translate user input. Take two sentences with the same meaning in Korean and English and translate them into {languageName}, no further explanation. If there is a color tag in the format '<color=#ffffff>' in the original text, do not omit it and keep it as is as much as possible.";
        }
        else
        {
            roleContent = $"You are a {languageName} translator who translate user input. Take two sentences with the same meaning in Korean and English and translate them into {languageName}, no further explanation.";
        }

        var input = $"{sourceKor}\n{sourceEng}";

        // 영문 번역 이외의 경우, 영어로 번역하지 말 것을 강조한다. 
        if (mode != TranslationMode.ToEnglish)
        {
            roleContent += $" You must translate it into {languageName}, not into English.";
        }

        return new ChatCompletionCreateRequest
        {
            Model = modelName,
            Messages = new List<ChatMessage>
            {
                ChatMessage.FromSystem(roleContent),
                ChatMessage.FromUser(input),
            },
        };
    }

    private Task<string> ReTranslate(TranslationMode mode, string input, string firstResponse)
    {
        var languageName = ToLanguageName(mode);

        var request = CreateRequest(mode, input, this.modelName);
        request.Messages.Add(ChatMessage.FromAssistant(firstResponse));
        request.Messages.Add(ChatMessage.FromSystem($"You must translate it into {languageName}, not into English."));
        request.Messages.Add(ChatMessage.FromUser(input));
        return this.GetResponse(request);
    }

    public sealed record SingleProcessResult(bool Translated, string TranslatedText);
}
