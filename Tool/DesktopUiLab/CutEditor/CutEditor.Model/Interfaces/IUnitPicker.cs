namespace CutEditor.Model.Interfaces;

using System.Threading.Tasks;
using Shared.Templet.TempletTypes;

public interface IUnitPicker
{
    Task<PickResult> PickUnit();

    public readonly record struct PickResult(Unit? Unit, bool IsCanceled);
}
