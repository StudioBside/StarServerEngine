namespace CutEditor.Model.Interfaces;

using System.Threading.Tasks;
using Shared.Templet.TempletTypes;

public interface IArcpointPicker
{
    Task<PickResult> Pick();

    public readonly record struct PickResult(LobbyItem? Data, bool IsCanceled);
}
