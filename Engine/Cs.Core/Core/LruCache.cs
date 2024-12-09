namespace Cs.Core.Core;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

// 참고 : https://www.geeksforgeeks.org/lru-cache-implementation/
public sealed class LruCache<TKey, TElement> where TKey : notnull
{
    private readonly LinkedList<(TKey Key, TElement Value)> timeline = new LinkedList<(TKey, TElement)>();
    private readonly Dictionary<TKey, LinkedListNode<(TKey Key, TElement Value)>> index;
    private Action<TElement>? onElementRemoved;

    public LruCache(int maxCount)
    {
        this.index = new Dictionary<TKey, LinkedListNode<(TKey, TElement)>>(maxCount);
        this.MaxCount = maxCount;
    }

    public LruCache(int maxCount, IEqualityComparer<TKey> comparer)
    {
        this.index = new Dictionary<TKey, LinkedListNode<(TKey, TElement)>>(maxCount, comparer);
        this.MaxCount = maxCount;
    }

    public int Count => this.timeline.Count;
    public IEnumerable<TElement> Values => this.timeline.Select(e => e.Value);
    public int MaxCount { get; }

    public void SetElementRemovedCallback(Action<TElement> callback)
    {
        this.onElementRemoved = callback;
    }

    public bool TryGetValue(TKey key, out TElement result)
    {
        bool exist = this.index.TryGetValue(key, out var node);
        if (exist == false)
        {
            result = default!;
            return false;
        }

        if (node == null)
        {
            result = default!;
            return false;
        }

        result = node.Value.Value;
        this.timeline.Remove(node);
        this.timeline.AddFirst(node);
        return true;
    }

    public bool Insert(TKey key, [DisallowNull] TElement element)
    {
        bool insert = false;
        if (this.index.TryGetValue(key, out var node) == false) //// 기존에 값이 존재하지 않음.
        {
            while (this.timeline.Count >= this.MaxCount) //// delete least recently used element
            {
                node = this.timeline.Last!; // 캐시 크기가 넘치면, 기존의 node를 재활용한다.
                this.timeline.RemoveLast();
                this.index.Remove(node.Value.Key);

                this.onElementRemoved?.Invoke(node.Value.Value);
            }

            if (node != null)
            {
                node.Value = (key, element);
            }
            else
            {
                node = new LinkedListNode<(TKey, TElement)>((key, element));
            }

            this.index.Add(key, node);
            insert = true;
        }
        else //// 기존에 값이 존재했다면 timeline을 최신화 해준다.
        {
            this.timeline.Remove(node);
            // node.Value = (key, element); // insert는 기존 값을 변경하지 않는다. 아래 Upsert만 값을 바꾼다.
        }

        this.timeline.AddFirst(node);
        return insert;
    }

    public bool Upsert(TKey key, TElement element)
    {
        bool insert = false;
        if (this.index.TryGetValue(key, out var node) == false) //// 기존에 값이 존재하지 않음.
        {
            while (this.timeline.Count >= this.MaxCount) //// delete least recently used element
            {
                node = this.timeline.Last!; // 캐시 크기가 넘치면, 기존의 node를 재활용한다.
                this.timeline.RemoveLast();
                this.index.Remove(node.Value.Key);

                // 콜백 호출
                this.onElementRemoved?.Invoke(node.Value.Value);
            }

            insert = true;
        }
        else //// 기존에 값이 존재했다면 timeline을 최신화 해준다.
        {
            this.timeline.Remove(node);
            if (ReferenceEquals(node.Value.Value, element) == false)
            {
                // key가 같지만 다른 instance가 이미 등록되어 있던 경우. 이전 값 제거 콜백 호출.
                this.onElementRemoved?.Invoke(node.Value.Value);
            }
        }

        if (node != null)
        {
            node.Value = (key, element);
        }
        else
        {
            node = new LinkedListNode<(TKey, TElement)>((key, element));
        }

        this.timeline.AddFirst(node);
        this.index[key] = node;
        return insert;
    }

    public void Remove(TKey key)
    {
        if (this.index.TryGetValue(key, out var node) == false) //// 기존에 값이 존재하지 않음.
        {
            return;
        }

        this.timeline.Remove(node);
        this.index.Remove(key);

        // 콜백 호출
        this.onElementRemoved?.Invoke(node.Value.Value);
    }

    public void Clear()
    {
        foreach (var node in this.timeline)
        {
            // 콜백 호출
            this.onElementRemoved?.Invoke(node.Value);
        }

        this.timeline.Clear();
        this.index.Clear();
    }
}