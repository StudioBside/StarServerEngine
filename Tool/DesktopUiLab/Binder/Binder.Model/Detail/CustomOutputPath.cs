namespace Binder.Model.Detail;

using System.Text.Json;
using Cs.Core.Util;

public sealed class CustomOutputPath
{
    // 패처씬이 사용하는 스트링이 별도의 위치로 export한다.
    private string serverTextOutput = string.Empty;
    private string serverBinOutput = string.Empty;
    private string clientTextOutput = string.Empty;
    private string clientBinOutput = string.Empty;

    public CustomOutputPath(JsonElement element)
    {
        this.serverTextOutput = element.GetString("serverTextOutput", string.Empty);
        this.serverBinOutput = element.GetString("serverBinOutput", string.Empty);
        this.clientTextOutput = element.GetString("clientTextOutput", string.Empty);
        this.clientBinOutput = element.GetString("clientBinOutput", string.Empty);
    }

    public string ServerTextOutput => this.serverTextOutput;
    public string ServerBinOutput => this.serverBinOutput;
    public string ClientTextOutput => this.clientTextOutput;
    public string ClientBinOutput => this.clientBinOutput;
}
