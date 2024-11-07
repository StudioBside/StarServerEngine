namespace CutEditor.Model.Interfaces;

using System.Threading.Tasks;
using Shared.Templet.Base;

public interface ITempletPicker<T> where T : ITemplet
{
    Task<PickResult> Pick();

    public readonly record struct PickResult(T? Data, bool IsCanceled);
}
