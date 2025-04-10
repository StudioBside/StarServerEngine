namespace SlackAssist.Fremawork.Slack;

using SlackNet;

public sealed class DebugDumpLogger : ILogger
{
    public void Log(ILogEvent logEvent)
    {
        switch (logEvent.Category)
        {
            case LogCategory.Error:
                Cs.Logging.Log.Error(logEvent.FullMessage());
                break;
            default:
                Cs.Logging.Log.DebugBold(logEvent.FullMessage());
                break;
        }
    }
}