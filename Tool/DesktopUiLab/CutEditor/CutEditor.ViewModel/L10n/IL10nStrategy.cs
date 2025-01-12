namespace CutEditor.ViewModel.L10n;

using CutEditor.Model.L10n;

internal interface IL10nStrategy
{
    L10nSourceType SourceType { get; }
    IReadOnlyList<IL10nMapping> Mappings { get; }
    IReadOnlyList<int> Statistics { get; }

    bool LoadOriginData(string name);
    bool ImportFile(string fileFullPath, ISet<string> importedHeaders);
    bool SaveToFile(string name);
}
