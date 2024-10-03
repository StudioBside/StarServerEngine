namespace CutEditor.ViewModel.Detail;

internal sealed class TempUidGenerator
{
    private long seed = 0;

    public long Generate() => ++this.seed;
}
