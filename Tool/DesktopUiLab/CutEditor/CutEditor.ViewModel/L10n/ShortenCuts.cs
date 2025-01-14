namespace CutEditor.ViewModel.L10n;

using System.Collections.Generic;
using CutEditor.Model;
using CutEditor.ViewModel.Detail;

public sealed record ShortenCuts(string FileName, IReadOnlyList<Cut> Cuts)
{
    public static IEnumerable<ShortenCuts> LoadAll()
    {
        foreach (var cutscene in CutSceneContainer.Instance.CutScenes)
        {
            var fileName = CutFileIo.GetTextFileName(cutscene.FileName, isShorten: true);
            if (File.Exists(fileName) == false)
            {
                continue;
            }

            var cuts = CutFileIo.LoadCutData(cutscene.FileName, isShorten: true);
            var uidGenerator = new CutUidGenerator(cuts);
            foreach (var cut in cuts.Where(e => e.Choices.Any()))
            {
                var choiceUidGenerator = new ChoiceUidGenerator(cut.Uid, cut.Choices);
            }

            yield return new ShortenCuts(cutscene.FileName, cuts);
        }
    }
}