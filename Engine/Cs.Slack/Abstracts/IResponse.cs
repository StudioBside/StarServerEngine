namespace Cs.Slack.Abstracts
{
    public interface IResponse
    {
        public bool Ok { get; }
        public string Error { get; }
    }
}