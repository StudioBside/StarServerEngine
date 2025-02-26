﻿namespace Cs.Gpt;

using System;
using System.Threading;
using System.Threading.Tasks;
using Betalgo.Ranul.OpenAI;
using Betalgo.Ranul.OpenAI.Managers;
using Betalgo.Ranul.OpenAI.ObjectModels.RequestModels;
using Betalgo.Ranul.OpenAI.ObjectModels.ResponseModels;
using Cs.Core.Util;
using Cs.Logging;

public abstract class GptClient : IDisposable
{
    public const string LimitExceedErrorCode = "rate_limit_exceeded";

    private const int RetryCount = 3;
#pragma warning disable CA2213 // 삭제 가능한 필드는 삭제해야 합니다. Dispose에서 호출해도 에러나서 무시 처리 했습니다.
    private readonly OpenAIService service;
#pragma warning restore CA2213 // 삭제 가능한 필드는 삭제해야 합니다.
    private readonly TimeSpan? delay; // rate limit 대응을 위한 딜레이
    private DateTime lastRequestTime;
    private long apiCallCount;
    private bool disposed;

    public GptClient(string apiKey, int apiRpd)
    {
        var options = new OpenAIOptions { ApiKey = apiKey };
        this.service = new OpenAIService(options);

        if (apiRpd > 0)
        {
            var daySeconds = 24 * 60 * 60;
            var rpdSeconds = daySeconds / apiRpd;
            this.delay = TimeSpan.FromSeconds(rpdSeconds);
            Log.Info($"apiRpd:{apiRpd} delay:{this.delay}");
        }
    }

    public long ApiCallCount => this.apiCallCount;
    public string LastErrorCode { get; protected set; } = string.Empty;
    public bool LimitExceed => this.LastErrorCode == LimitExceedErrorCode;

    public void Dispose()
    {
        // 이 코드를 변경하지 마세요. 'Dispose(bool disposing)' 메서드에 정리 코드를 입력합니다.
        this.Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    //// ----------------------------------------------------------------------------------------

    protected async Task<string> GetResponse(ChatCompletionCreateRequest request, CancellationToken cancelToken)
    {
        for (int i = 0; i < RetryCount; ++i)
        {
            ChatCompletionCreateResponse? result;
            try
            {
                if (this.delay.HasValue)
                {
                    var current = ServiceTime.Recent;
                    var elapsed = current - this.lastRequestTime;
                    if (elapsed < this.delay)
                    {
                        var wait = this.delay.Value - elapsed;
                        Log.Info($"current:{current} apiCallCount:{this.apiCallCount} wait:{wait}");
                        await Task.Delay(wait, cancelToken);

                        if (cancelToken.IsCancellationRequested)
                        {
                            return string.Empty;
                        }
                    }
                }

                result = await this.service.ChatCompletion.CreateCompletion(request, cancellationToken: cancelToken);
                ++this.apiCallCount;
                this.lastRequestTime = ServiceTime.Recent;
            }
            catch (Exception e)
            {
                if (i == RetryCount - 1)
                {
                    Log.Error($"retry:{i} #error:{e.Message}");
                    return string.Empty;
                }

                var waitingSeconds = (int)Math.Pow(2, i + 1); // 2, 4, 8
                
                Log.Error($"재시도 인덱스:{i} 예외:{e.Message}");
                Log.Error($"재시도 대기 시간:{waitingSeconds}초");

                await Task.Delay(TimeSpan.FromSeconds(waitingSeconds), cancelToken);
                continue;
            }

            if (result.Error is not null)
            {
                var error = result.Error;
                this.LastErrorCode = error.Code ?? string.Empty;
                Log.Error($"receive error response. code:{error.Code} type:{error.Type} message:{error.Message}");
                this.OnError(error);
                continue;
            }

            return result.Choices[0].Message.Content ?? string.Empty;
        }

        return string.Empty;
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!this.disposed)
        {
            if (disposing)
            {
                // TODO: 관리형 상태(관리형 개체)를 삭제합니다.
                this.service.Dispose();
            }

            // TODO: 비관리형 리소스(비관리형 개체)를 해제하고 종료자를 재정의합니다.
            // TODO: 큰 필드를 null로 설정합니다.
            this.disposed = true;
        }
    }

    protected abstract void OnError(Error error);
}
