namespace Du.Core.Interfaces;

using System.Collections.Generic;

public interface IExcelFileReader
{
    bool Read<T>(string filePath, IList<T> collection) where T : new();
    bool Read<T>(string filePath, ISet<string> headers, IList<T> collection) where T : new();
}
