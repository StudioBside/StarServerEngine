namespace Cs.Slack.Exceptions
{
    using System.Collections.Generic;
    using Cs.Exception;
    using Cs.Logging;

    public sealed class ExceptionSlackSender : ISlackSender
    {
        private readonly SlackEndpoint[] slackEndponts;
       
        public ExceptionSlackSender(SlackEndpoint[] slackEndponts)
        {
            List<SlackEndpoint> validEndpoints = new();
            foreach (var endpoint in slackEndponts)
            {
                if (SlackGlobalSetting.TryGetChannelId(endpoint.Channel, out var channelId) == false)
                {
                    Log.Error($"크래시 리포트 할 슬랙 체널의 아이디를 찾지 못했습니다 channel:{endpoint.Channel}");
                    continue;
                }

                validEndpoints.Add(new SlackEndpoint(endpoint.Token, channelId));
            }

            this.slackEndponts = validEndpoints.ToArray();
        }

        public void SendSnippet(string title, string text)
        {
            using var builder = new SlackMessageBuilder(this.slackEndponts);
            builder.PresetSnippet(title, text);
        }

        public void SendMessage(string userName, string authorName, string title, string text)
        {
            using var builder = new SlackMessageBuilder(this.slackEndponts);
            builder.PresetAttachment(userName, authorName, title, text);
        }
    }
}
