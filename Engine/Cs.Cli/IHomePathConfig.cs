namespace Cs.Cli;

public enum HomePathConfigType
{
    Global,
    Local,
    All,
}

public interface IHomePathConfig
{
    // immutable
    IEnumerable<string> GetList(HomePathConfigType type);
    string? GetValue(HomePathConfigType type, string key);
    string GetFilePath(HomePathConfigType type);

    // mutable
    void SetValue(HomePathConfigType type, string key, string value);
    void UnsetValue(HomePathConfigType type, string key);
    void RemoveSection(HomePathConfigType type, string section);
}
