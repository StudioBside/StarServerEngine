namespace CutEditor.Services;

using CutEditor.Model.Interfaces;
using Microsoft.Win32;

internal sealed class FilePicker : IFilePicker
{
    string? IFilePicker.PickFile(string initialDirectory, string filter)
    {
        var dialog = new OpenFileDialog
        {
            InitialDirectory = initialDirectory,
            Filter = filter,
        };

        if (dialog.ShowDialog() == false)
        {
            return null;
        }

        return dialog.FileName;
    }
}
