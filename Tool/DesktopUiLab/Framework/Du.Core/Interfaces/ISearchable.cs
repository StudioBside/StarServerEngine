namespace Du.Core.Interfaces;

public interface ISearchable
{
    bool IsTarget(string keyword);
}

public static class SearchableExt
{
    public static bool CheckKeyword(this ISearchable self, string keyword)
    {
        if (string.IsNullOrEmpty(keyword))
        {
            return true;
        }

        return self.IsTarget(keyword);
    }
}
