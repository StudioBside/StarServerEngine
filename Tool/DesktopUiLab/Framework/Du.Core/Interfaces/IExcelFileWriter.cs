namespace Du.Core.Interfaces;

using System.Collections.Generic;

public interface IExcelFileWriter : IDisposable
{
    bool Write<T>(string filePath, IEnumerable<T> collection);

    bool CreateSheet<T>(string filePath);
    bool AppendToSheet<T>(IEnumerable<T> collection);
    bool CloseSheet<T>();
}
