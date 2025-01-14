namespace CutEditor.ViewModel.L10n.Strategies;

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Cs.Logging;
using CutEditor.Model.ExcelFormats;
using CutEditor.Model.L10n;
using CutEditor.ViewModel.Detail;
using Du.Core.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using static CutEditor.Model.Enums;
using static StringStorage.Enums;

internal sealed class CutsceneShortenStrategy(VmL10n viewModel) : L10nStrategyBase(L10nSourceType.CutsceneShorten)
{
    private readonly ObservableCollection<L10nMappingShorten> mappings = new();
    private readonly Dictionary<string, List<L10nMappingShorten>> mappingsByFile = new(); // 저장할 때 처리가 용이하도록 파일단위 데이터 유지.

    public override IReadOnlyList<IL10nMapping> Mappings => this.mappings;

    public override bool LoadOriginData(string name)
    {
        int normalCut = 0;
        int branchCut = 0;
        this.mappings.Clear();
        foreach (var (fileName, cuts) in ShortenCuts.LoadAll())
        {
            var perFileMappings = new List<L10nMappingShorten>();
            foreach (var cut in cuts)
            {
                if (cut.Choices.Count == 0)
                {
                    var mapping = new L10nMappingShorten(fileName, cut);
                    this.mappings.Add(mapping);
                    ++normalCut;

                    perFileMappings.Add(mapping);
                }
                else
                {
                    foreach (var choice in cut.Choices)
                    {
                        var mapping = new L10nMappingShorten(fileName, cut, choice);
                        this.mappings.Add(mapping);
                        ++branchCut;

                        perFileMappings.Add(mapping);
                    }
                }
            }

            this.mappingsByFile.Add(fileName, perFileMappings);
        }

        viewModel.WriteLog($"컷신 이름:{name} 전체 데이터 {this.mappings.Count}개. 기본형 {normalCut}개, 선택지 {branchCut}개.");
        return true;
    }

    public override bool ImportFile(string fileFullPath, ISet<string> importedHeaders)
    {
        foreach (var mapping in this.mappings)
        {
            mapping.SetImported(null);
        }

        var reader = viewModel.Services.GetRequiredService<IExcelFileReader>();
        var importedCuts = new List<ShortenCutOutputExcelFormat>();
        if (reader.Read(fileFullPath, importedHeaders, importedCuts) == false)
        {
            Log.Error($"엑셀 파일 읽기에 실패했습니다. fileName:{fileFullPath}");
            return false;
        }

        var dicMappings = this.mappings.ToDictionary(e => e.FileAndUid);
        this.ClearStatistics();
        // -------------------- mapping data --------------------
        foreach (var imported in importedCuts)
        {
            if (dicMappings.TryGetValue(imported.FileAndUid, out var mapping) == false)
            {
                viewModel.WriteLog($"컷을 찾을 수 없습니다. uid:{imported.Uid}");
                this.IncreaseStatistics(L10nMappingState.MissingOrigin);
                continue;
            }

            mapping.SetImported(imported);
            this.IncreaseStatistics(mapping.MappingState);

            if (mapping.MappingState == L10nMappingState.TextChanged)
            {
                var sb = new StringBuilder();
                sb.AppendLine($"[Uid:{mapping.UidStr}] 한글 텍스트가 일치하지 않습니다.");
                sb.AppendLine($"  원본: {mapping.SourceData.Korean}");
                sb.Append($"  번역본: {imported.Korean}");
                viewModel.WriteLog(sb.ToString());
            }
        }

        return true;
    }

    public override bool SaveToFile(string name, L10nType l10nType)
    {
        var updatedFiles = new List<string>();
        foreach (var (fileName, mappings) in this.mappingsByFile)
        {
            int changedCount = 0;
            foreach (var mapping in mappings)
            {
                if (mapping.ApplyData(l10nType))
                {
                    ++changedCount;
                }
            }

            if (changedCount == 0)
            {
                continue;
            }

            Log.Debug($"단축 컷신 번역 적용. 파일명:{fileName} 업데이트 데이터 개수:{changedCount}");
            if (CutFileIo.SaveCutData(fileName, mappings.Select(e => e.Cut), isShorten: true) == false)
            {
                Log.Error($"{fileName} 파일 저장 실패.");
                continue;
            }

            updatedFiles.Add(fileName);
        }

        viewModel.WriteLog($"번역 적용 완료. 대상 언어:{l10nType} 변경된 파일 {updatedFiles.Count}개.");
        foreach (var file in updatedFiles)
        {
            viewModel.WriteLog($"  - {file}");
        }

        return true;
    }
}
