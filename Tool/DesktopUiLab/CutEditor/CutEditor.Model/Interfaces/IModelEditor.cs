namespace CutEditor.Model.Interfaces;

using System.Threading.Tasks;

public interface IModelEditor<T> where T : class
{
    Task<PickResult> EditAsync(T? current);

    public readonly record struct PickResult(T? Data, bool IsCanceled);
}
