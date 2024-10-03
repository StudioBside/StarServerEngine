namespace CutEditor.Model.Interfaces;

using System.Threading.Tasks;

public interface IUnitPicker
{
    Task<Unit?> PickUnit();
}
