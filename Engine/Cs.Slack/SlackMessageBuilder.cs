﻿namespace Cs.Slack;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using Cs.Core.Util;
using Cs.HttpClient;
using Cs.Logging;
using Cs.Slack.Abstracts;
using Cs.Slack.Elements;
using Cs.Slack.Responses;

internal sealed class SlackMessageBuilder : IDisposable
{
    private static readonly RestApiClient ApiClient = new RestApiClient("https://slack.com/api/");

    private readonly SlackEndpoint[] endpoints;
    private bool disposed;

    public SlackMessageBuilder(SlackEndpoint[] endpoints)
    {
        this.endpoints = endpoints;
    }

    public SlackMessageBuilder(SlackEndpoint endpoint)
    {
        this.endpoints = [endpoint];
    }

    public string UserName { get; set; } = string.Empty;
    public string IconEmoji { get; set; } = ":desktop_computer:";
    public string Text { get; set; } = string.Empty;
    public SnippetData? Snippet { get; set; }
    public List<Attachment> Attachments { get; } = new();
    public List<IBlock> Blocks { get; } = new();
    private bool HasAnyEndpont => this.endpoints != null && this.endpoints.Length > 0;

    public void CancelSendMessage()
    {
        this.disposed = true;
    }

    public void Dispose()
    {
        if (this.disposed)
        {
            return;
        }

        this.disposed = true;

        if (this.HasAnyEndpont == false)
        {
            return;
        }

        foreach (var endpoint in this.endpoints)
        {
            if (this.Snippet != null)
            {
                this.SendSnippet(endpoint);
                continue;
            }

            this.SendMessage(endpoint);
        }
    }

    private void SendSnippet(SlackEndpoint endpoint)
    {
        var contents = Encoding.UTF8.GetBytes(this.Snippet!.Content);

        var parameters = new Dictionary<string, string>
        {
            { "token", endpoint.Token },
            { "filename", this.Snippet.Title },
            { "length", $"{contents.Length}" },
        };

        var getUrlResponse = ApiClient.PostAsync("files.getUploadURLExternal", new FormUrlEncodedContent(parameters)).Result;
        if (getUrlResponse.IsSuccessStatusCode == false)
        {
            Log.Error($"[SlackMessageBuilder] files.getUploadURLExternal. response:{getUrlResponse}");
        }

        var uploadUrlResult = getUrlResponse.Content.ReadFromJsonAsync<GetUploadUrlResponse>().Result;
        if (uploadUrlResult is null || uploadUrlResult.Ok == false)
        {
            Log.Error($"[SlackMessageBuilder] files.getUploadURLExternal fail. error:{uploadUrlResult?.Error}");
            return;
        }

        var uploadClient = new RestApiClient(uploadUrlResult.UploadUrl);
        var uploadResponse = uploadClient.PostAsync(string.Empty, new ByteArrayContent(contents)).Result;
        if (uploadResponse.IsSuccessStatusCode == false)
        {
            Log.Error($"[SlackMessageBuilder] file upload fail. response:{uploadResponse}");
        }

        parameters = new Dictionary<string, string>
        {
            { "token", endpoint.Token },
            { "files", $"[{{\"id\":\"{uploadUrlResult.FileId}\"}}]" },
            { "channel_id", endpoint.Channel },
        };

        var completeResponse = ApiClient.PostAsync("files.completeUploadExternal", new FormUrlEncodedContent(parameters)).Result;
        if (completeResponse.IsSuccessStatusCode == false)
        {
            Log.Error($"[SlackMessageBuilder] files.completeUploadExternal fail. response:{completeResponse}");
        }

        Log.Info($"sending slack snippet. response code:{completeResponse.StatusCode} resonPhrase:{completeResponse.ReasonPhrase}");
    }

    private void SendMessage(SlackEndpoint endpoint)
    {
        var parameters = new Dictionary<string, string>
        {
            { "token", endpoint.Token },
            { "channel", endpoint.Channel },
            { "username", this.UserName },
            { "icon_emoji", this.IconEmoji },
        };

        if (string.IsNullOrEmpty(this.Text) == false)
        {
            parameters.Add("text", this.Text);
        }

        if (this.Blocks.Any())
        {
            string blocks = JsonUtil.ToLowerKeyJson(this.Blocks);
            parameters.Add("blocks", blocks);
        }

        if (this.Attachments.Any())
        {
            string attachments = JsonUtil.ToLowerKeyJson(this.Attachments);
            parameters.Add("attachments", attachments);
        }

        var response = ApiClient.PostAsync("chat.postMessage", new FormUrlEncodedContent(parameters)).Result;
        if (response.IsSuccessStatusCode == false)
        {
            Log.Error($"[SlackMessageBuilder] chat.postMessage fail. response:{response.ToString()}");
        }

        // florist. 메시지 전송 실패할 때 아래 주석 풀고 오류 내용 확인. 일반적인 경우는 task wait으로 block발생하여 사용 않음.
        ////var jObject = response.GetContent().Result;
        ////if ((string)jObject["ok"] != "true")
        ////{
        ////    Log.Error($"slack post error:{jObject}");
        ////}

        Log.Info($"sending slack attacment. response code:{response.StatusCode} resonPhrase:{response.ReasonPhrase}");
    }
}