namespace CutEditor.Services;

using System.Threading.Tasks;
using CutEditor.Model;
using CutEditor.Model.Interfaces;

internal sealed class UnitPicker(UnitContainer container) : IUnitPicker
{
    public async Task<Unit?> PickUnit()
    {
        await Task.Delay(1000);
        return container.Units.First();
    }
}
