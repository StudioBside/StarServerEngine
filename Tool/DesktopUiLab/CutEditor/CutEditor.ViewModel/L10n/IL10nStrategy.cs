namespace CutEditor.ViewModel.L10n;

using CutEditor.Model.L10n;

internal interface IL10nStrategy
{
    L10nSourceType SourceType { get; }
    IEnumerable<IL10nMapping> Mappings { get; }
    int SourceCount { get; }
    IReadOnlyList<int> Statistics { get; }

    bool LoadOriginData(string name, VmL10n viewModel);
    bool ImportFile(string fileFullPath, VmL10n viewModel, ISet<string> importedHeaders);
    bool SaveToFile(string name);
}
