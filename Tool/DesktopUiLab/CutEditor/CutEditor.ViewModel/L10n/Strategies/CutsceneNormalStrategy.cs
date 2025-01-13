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

internal sealed class CutsceneNormalStrategy(VmL10n viewModel) : L10nStrategyBase(L10nSourceType.CutsceneNormal)
{
    private readonly ObservableCollection<L10nMappingNormal> mappings = new();

    public override IReadOnlyList<IL10nMapping> Mappings => this.mappings;

    public override bool LoadOriginData(string name)
    {
        var cutList = CutFileIo.LoadCutData(name, isShorten: false);
        if (cutList.Count == 0)
        {
            Log.Warn($"{name} 파일 로딩 실패.");
            return false;
        }

        var uidGenerator = new CutUidGenerator(cutList);
        foreach (var cut in cutList.Where(e => e.Choices.Any()))
        {
            var choiceUidGenerator = new ChoiceUidGenerator(cut.Uid, cut.Choices);
        }

        int normalCut = 0;
        int branchCut = 0;
        this.mappings.Clear();
        foreach (var cut in cutList)
        {
            L10nMappingNormal mapping;
            if (cut.Choices.Count == 0)
            {
                mapping = new L10nMappingNormal(cut);
                this.mappings.Add(mapping);
                ++normalCut;
            }
            else
            {
                foreach (var choice in cut.Choices)
                {
                    mapping = new L10nMappingNormal(cut, choice);
                    this.mappings.Add(mapping);
                    ++branchCut;
                }
            }
        }

        viewModel.WriteLog($"컷신 이름:{name} 전체 데이터 {cutList.Count}개. 기본형 {normalCut}개, 선택지 {branchCut}개.");
        return true;
    }

    public override bool ImportFile(string fileFullPath, ISet<string> importedHeaders)
    {
        foreach (var mapping in this.mappings)
        {
            mapping.SetImported(null);
        }

        var reader = viewModel.Services.GetRequiredService<IExcelFileReader>();
        var importedCuts = new List<CutOutputExcelFormat>();
        if (reader.Read(fileFullPath, importedHeaders, importedCuts) == false)
        {
            Log.Error($"엑셀 파일 읽기에 실패했습니다. fileName:{fileFullPath}");
            return false;
        }

        var dicMappings = this.mappings.ToDictionary(e => e.UidStr);
        this.ClearStatistics();
        // -------------------- mapping data --------------------
        foreach (var imported in importedCuts)
        {
            if (dicMappings.TryGetValue(imported.Uid, out var mapping) == false)
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
        return CutFileIo.SaveCutData(name, this.mappings.Select(e => e.Cut), isShorten: false);
    }
}
