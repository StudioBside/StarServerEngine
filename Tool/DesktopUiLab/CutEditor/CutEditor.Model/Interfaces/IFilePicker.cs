namespace CutEditor.Model.Interfaces;

using System.Threading.Tasks;

public interface IFilePicker
{
    string? PickFile(string initialDirectory, string filter);
}
