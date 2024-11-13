namespace Du.Core.Interfaces;

using System.Collections.Generic;

public interface IExcelFileWriter
{
    bool Write<T>(string filePath, IEnumerable<T> collection);
}
