namespace CutEditor.ViewModel.Detail;

using CutEditor.Model;

internal sealed class CutUidGenerator
{
    private long uidSeed = 0;

    public CutUidGenerator(IEnumerable<Cut> cuts)
    {
        if (cuts.Any() == false)
        {
            return;
        }

        bool newFormat = cuts.First().Uid > 0;
        if (newFormat)
        {
            // 이미 로딩한 데이터에 uid가 있는 경우 (=신규 포맷) : seed를 조정.
            this.uidSeed = cuts.Max(x => x.Uid);
        }
        else
        {
            // 기존 포맷이어서 파일에 uid가 없는 경우 : 신규 발급
            foreach (var data in cuts)
            {
                data.ResetOldDataUid(this.Generate());
            }
        }
    }
     
    internal long Generate() => ++this.uidSeed;
}
