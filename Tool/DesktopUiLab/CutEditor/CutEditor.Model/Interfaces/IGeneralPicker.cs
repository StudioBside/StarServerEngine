namespace CutEditor.Model.Interfaces;

using System.Threading.Tasks;
using Shared.Templet.Base;

public interface IGeneralPicker<T> where T : class
{
    Task<PickResult> Pick();
    void SetCurrentValues(T? currentValue)
    {
    }

    public readonly record struct PickResult(T? Data, bool IsCanceled);
}
