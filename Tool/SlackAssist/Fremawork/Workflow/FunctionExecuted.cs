namespace SlackAssist.Fremawork.Workflow;

using System.Collections.Generic;
using SlackNet;
using SlackNet.Events;

// note: SlackTypeResolver에 등록하되려면 public이어야 합니다.
public sealed class FunctionExecuted : Event
{
    public string FunctionExecutionId { get; set; } = string.Empty;
    public string WorkflowExecuteId { get; set; } = string.Empty;
    public FunctionImplementation Function { get; set; } = new();

    public IDictionary<string, string> Inputs { get; set; } = new Dictionary<string, string>();

    public class FunctionImplementation
    {
        public string CallbackId { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public IList<WorkflowOutput> OutputParameters { get; set; } = [];
    }
}