namespace CutEditor.Model.Interfaces;

using System.Threading.Tasks;

public interface IEnumPicker<T> where T : Enum
{
    Task<PickResult> Pick(T defaultValue);

    public readonly record struct PickResult(T Data, bool IsCanceled);
}
