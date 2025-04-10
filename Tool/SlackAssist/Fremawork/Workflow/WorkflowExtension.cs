namespace SlackAssist.Fremawork.Workflow;

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SlackNet;

public static class WorkflowExtension
{
    public static Task WorkflowSuccess(this ISlackApiClient self, string executionId, Dictionary<string, string> outputs)
    {
        return self.Post(
            "functions.completeSuccess",
            new Dictionary<string, object>
            {
                { "function_execution_id", executionId },
                { "outputs", outputs },
            },
            CancellationToken.None);
    }

    public static Task WorkflowError(this ISlackApiClient self, string executionId, string errorMessage)
    {
        return self.Post(
            "functions.completeError",
            new Dictionary<string, object>
            {
                { "error", errorMessage },
                { "function_execution_id", executionId },
            },
            CancellationToken.None);
    }
}