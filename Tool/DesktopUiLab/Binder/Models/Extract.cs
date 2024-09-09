namespace Binder.Models;

using System.Collections.Generic;
using static Binder.Models.Enums;

public sealed class Extract
{
    private readonly List<Source> sources = new();
    private readonly List<Uniqueness> uniquenesses = new();
    private string outputFile = string.Empty;
    ////private string outputGroupBy = string.Empty;
    ////private string outputFilePrefix = string.Empty;
    ////private OutputDirection fileOutDirection = OutputDirection.All;
    ////private bool excludeToolOutput;
    ////// clientOutputType
    ////// bindRoot
    ////private CustomOutputPath? customOutputPath;
    ////private DuplicationCleaner? duplicationCleaner;

    public sealed class Source
    {
        private string excelFile = string.Empty;
        private string sheetName = string.Empty;
        private string beginCell = "A1";
    }

    public sealed class Uniqueness
    {
        private readonly string name = string.Empty;
        private readonly List<string> columnNames = new();
    }

    // 패처씬이 사용하는 스트링이 별도의 위치로 export한다.
    public sealed class CustomOutputPath
    {
        private string serverTextOutput = string.Empty;
        private string serverBinOutput = string.Empty;
        private string clientTextOutput = string.Empty;
        private string clientBinOutput = string.Empty;
    }

    public sealed class DuplicationCleaner
    {
        private readonly OutputDirection fileOutDirection = OutputDirection.Client;
        private readonly List<string> columnNames = new();
    }
}
