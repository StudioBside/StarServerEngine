namespace Shared.Templet.Base;

using System;
using System.Collections.Frozen;
using System.Diagnostics.CodeAnalysis;
using Cs.Core;
using Cs.Core.Util;
using Cs.Logging;

public sealed class TempletContainer<T> where T : ITemplet
{
    private static readonly TempletContainer<T> Instance = Singleton<TempletContainer<T>>.Instance;

    private FrozenDictionary<int, T> data = FrozenDictionary<int, T>.Empty;
    private FrozenDictionary<string, T> strData = FrozenDictionary<string, T>.Empty;

    public static IEnumerable<T> Values => Instance.data.Values;

    public bool TryGet(int key, [MaybeNullWhen(false)] out T value) => Instance.data.TryGetValue(key, out value);
    public bool TryGet(string key, [MaybeNullWhen(false)] out T value) => Instance.strData.TryGetValue(key, out value);

    public T? Find(int key) => Instance.data.TryGetValue(key, out var value) ? value : default;
    public T? Find(string key) => Instance.strData.TryGetValue(key, out var value) ? value : default;

    //// --------------------------------------------------------------------

    internal static void Join()
    {
        foreach (var data in Values)
        {
            data.Join();
        }
    }

    internal static void Validate()
    {
        foreach (var data in Values)
        {
            data.Validate();
        }
    }

    internal static void SetData(IReadOnlyDictionary<int, T> data)
    {
        SetData(data, strKeySelector: null);
    }

    internal static void SetData(IReadOnlyDictionary<int, T> data, Func<T, string>? strKeySelector)
    {
        Instance.data = data.ToFrozenDictionary();
        if (strKeySelector is not null)
        {
            try
            {
                // 여기서 ToFrozenDictionary()를 호출하면, 중복 값이 존재하는 경우 시퀸스의 마지막 데이터로 덮어씌워집니다.
                var strData = data.Values.ToDictionary(strKeySelector);
                if (strData == null)
                {
                    ErrorContainer.Add($"[{typeof(T).Name}] Table contains string Dictionary null.");
                    return;
                }

                Instance.strData = strData.ToFrozenDictionary();
            }
            catch (Exception ex)
            {
                var duplicatedKeys = string.Join(",", data.Values.GetDuplicatedKeys(strKeySelector));
                ErrorContainer.Add($"[{typeof(T).Name}] Table contains duplicate string key. duplicatedKeys{duplicatedKeys} exception:{ex.Message}");
            }
        }
    }

    internal static void MergeContainer<TTarget>() where TTarget : class, T
    {
        var target = TempletContainer<TTarget>.Values;

        try
        {
            var merged = Enumerable.Union(Values, target).ToDictionary(e => e.Key);
            if (merged == null)
            {
                ErrorContainer.Add($"[{typeof(T).Name}] Merged contains Dictionary null.");
                return;
            }

            Instance.data = merged.ToFrozenDictionary();
        }
        catch (Exception ex)
        {
            ErrorContainer.Add($"[{typeof(T).Name}] Merge contains fail. exception:{ex.Message}");
        }
    }
}
