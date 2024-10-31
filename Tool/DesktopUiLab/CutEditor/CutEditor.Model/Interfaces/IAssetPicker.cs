namespace CutEditor.Model.Interfaces;

using System.Threading.Tasks;

public interface IAssetPicker
{
    Task<PickResult> PickAsset(string? defaultValue);

    Task<PickResult> PickAsset() => this.PickAsset(defaultValue: null);

    public readonly record struct PickResult(string? AssetFile, bool IsCanceled);
}
