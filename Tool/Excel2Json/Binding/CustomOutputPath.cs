namespace Excel2Json.Binding;

using Cs.Logging;

internal sealed record CustomOutputPath
{
    public string ServerTextOutput { get; set; } = string.Empty;
    public string ServerBinOutput { get; set; } = string.Empty;
    public string ClientTextOutput { get; set; } = string.Empty;
    public string ClientBinOutput { get; set; } = string.Empty;

    public bool Validate()
    {
        bool pathValidation =
            ValidatePath(this.ServerTextOutput, nameof(this.ServerTextOutput)) &&
            ValidatePath(this.ServerBinOutput, nameof(this.ServerBinOutput)) &&
            ValidatePath(this.ClientTextOutput, nameof(this.ClientTextOutput)) &&
            ValidatePath(this.ClientBinOutput, nameof(this.ClientBinOutput));

        return pathValidation;

        static bool ValidatePath(string relPath, string pathName)
        {
            if (string.IsNullOrEmpty(relPath))
            {
                return true;
            }

            if (Directory.Exists(relPath) == false)
            {
                Log.Error($"설정 파일의 경로가 올바르지 않습니다. {pathName}:{relPath}");
                return false;
            }

            Log.Debug($"{pathName}:{relPath} ... OK");
            return true;
        }
    }

    public IEnumerable<string> GetTextOutputs()
    {
        if (string.IsNullOrEmpty(this.ServerTextOutput) == false)
        {
            yield return this.ServerTextOutput;
        }

        if (string.IsNullOrEmpty(this.ClientTextOutput) == false)
        {
            yield return this.ClientTextOutput;
        }
    }

    public IEnumerable<string> GetBinOutputs()
    {
        if (string.IsNullOrEmpty(this.ClientTextOutput) == false)
        {
            yield return this.ClientTextOutput;
        }

        if (string.IsNullOrEmpty(this.ClientBinOutput) == false)
        {
            yield return this.ClientBinOutput;
        }
    }
}
