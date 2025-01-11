namespace CutEditor.Services;

using CutEditor.Model.Interfaces;
using Microsoft.Win32;

/// <summary>
/// 파일 선택 dialog를 띄우는 서비스입니다.
/// </summary>
/// <example>
/// var picker = this.services.GetRequiredService&lt;IFilePicker>();
/// var filePath = picker.PickFile(Environment.CurrentDirectory, "엑셀 파일 (*.xlsx)|*.xlsx");
/// if (filePath is null)
/// {
///     return;
/// }.
/// </example>
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
