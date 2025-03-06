namespace CutEditor.Model.Interfaces;

using System.Threading.Tasks;
using Shared.Templet.TempletTypes;

public interface IUnitPopup
{
    Task Show(Unit unitTemplet);
}
