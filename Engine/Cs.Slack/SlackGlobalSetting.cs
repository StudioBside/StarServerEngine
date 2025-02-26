namespace Cs.Slack;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Cs.HttpClient;
using Cs.Logging;
using Cs.Slack.Responses;

public static class SlackGlobalSetting
{
    private static readonly Dictionary<string, string> ChannelNameToId = new();

    public static string UserName { get; set; } = string.Empty;
    public static SlackEndpoint[] Endpoints { get; set; } = Array.Empty<SlackEndpoint>();

    public static void BuildChannelNameToId()
    {
        if (Endpoints.Length <= 0)
        {
            return;
        }

        var result = GetChannelList(Endpoints[0].Token, excludeArchived: true, limit: 1000).Result;
        if (result == null || result.Ok == false)
        {
            Log.Error($"채널 목록을 가져오는데 실패했습니다.");
            return;
        }

        ChannelNameToId.Clear();
        foreach (var channel in result.Channels)
        {
            ChannelNameToId.Add(channel.Name, channel.Id);
        }
    }

    public static bool TryGetChannelId(string channelName, [MaybeNullWhen(false)] out string channelId)
    {
        return ChannelNameToId.TryGetValue(channelName, out channelId);
    }

    private static async Task<ConversationListResponse?> GetChannelList(string token, bool excludeArchived, int limit)
    {
        var apiClient = new RestApiClient("https://slack.com/api/");
        apiClient.SetBearerAutohrization(token);

        var request = new Dictionary<string, string>()
        {
            { "exclude_archived", $"{excludeArchived}" },
        };

        if (limit > 0)
        {
            if (limit > 1000)
            {
                limit = 1000;
            }

            request.Add("limit", $"{limit}");
        }

        var response = apiClient.PostAsync("conversations.list", new FormUrlEncodedContent(request)).Result;

        if (response is null ||
            response.IsSuccessStatusCode == false)
        {
            return null;
        }

        return await response.Content.ReadFromJsonAsync<ConversationListResponse>();
    }
}