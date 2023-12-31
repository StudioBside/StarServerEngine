namespace Excel2Json.ToStringTable.Model;

using System.Text.RegularExpressions;

// 스트링 데이터 하나에 해당하는 타입.
// value가 같은 데이터는 하나로 묶어서 출력한다.
internal sealed class SingleStringData
{
    private readonly HashSet<string> keys = new();

    public SingleStringData(string value)
    {
        this.StringValue = $"{Regex.Replace(value, Regex.Escape("\""), "\\\"")}";
    }

    public string StringValue { get; }
    public IEnumerable<string> Keys => this.keys;
    public int KeyCount => this.keys.Count;
    public bool IsSingleKey => this.keys.Count == 1;
    public string FirstKey => this.keys.First();

    public void AddKey(string key)
    {
        this.keys.Add(key);
    }
}
