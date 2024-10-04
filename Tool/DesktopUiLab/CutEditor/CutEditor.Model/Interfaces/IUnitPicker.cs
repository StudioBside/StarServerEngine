namespace CutEditor.Model.Interfaces;

using System.Threading.Tasks;
using Shared.Templet.TempletTypes;

public interface IUnitPicker
{
    Task<Unit?> PickUnit();
}
