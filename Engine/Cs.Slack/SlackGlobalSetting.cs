namespace Cs.Slack
{
    using System;

    public static class SlackGlobalSetting
    {
        public static string UserName { get; set; }
        public static SlackEndpoint[] Endpoints { get; set; } = Array.Empty<SlackEndpoint>();
    }
}
