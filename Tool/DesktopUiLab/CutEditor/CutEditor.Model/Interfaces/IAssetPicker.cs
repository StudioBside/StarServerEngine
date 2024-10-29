namespace CutEditor.Model.Interfaces;

using System.Threading.Tasks;

public interface IAssetPicker
{
    Task<PickResult> PickAsset();

    public readonly record struct PickResult(string? AssetFile, bool IsCanceled);
}
