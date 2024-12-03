namespace Shared.Templet.UnitScripts;

using System.Text;
using Cs.Core.Util;
using Newtonsoft.Json.Linq;

public sealed class UiState
{
    public UiState(JToken token)
    {
        this.Name = token.GetString("StateName");
    }

    public string Name { get; }
}
