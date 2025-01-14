namespace CutEditor.Model.ExcelFormats;

public sealed class ShortenCutOutputExcelFormat : CutOutputExcelFormat
{
    public ShortenCutOutputExcelFormat(Cut cut, string fileName) : base(cut)
    {
        this.FileName = fileName;
    }

    public ShortenCutOutputExcelFormat(ChoiceOption choice, string fileName) : base(choice)
    {
        this.FileName = fileName;
    }

    public ShortenCutOutputExcelFormat()
    {
        // note: file에서 읽어들일 때 사용하기 때문에 빈 생성자를 만들어둡니다.
    }

    public string FileAndUid => $"{this.FileName} ({this.Uid})";
    public string FileName { get; set; } = string.Empty;
}
