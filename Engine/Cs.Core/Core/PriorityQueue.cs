/*
 * This code implements priority queue which uses min-heap as underlying storage
 * 
 * Copyright (C) 2010 Alexey Kurakin
 * www.avk.name
 * alexey[ at ]kurakin.me
 */

namespace Cs.Core.Core;

using System;
using System.Collections;
using System.Collections.Generic;

public sealed class PriorityQueue<TValue> : ICollection<TValue>
    where TValue : notnull
{
    private List<TValue> baseHeap;
    private IComparer<TValue> comparer;

    #region Constructors

    public PriorityQueue()
        : this(Comparer<TValue>.Default)
    {
    }

    public PriorityQueue(int capacity)
        : this(capacity, Comparer<TValue>.Default)
    {
    }

    public PriorityQueue(int capacity, IComparer<TValue> comparer)
    {
        this.baseHeap = new List<TValue>(capacity);
        this.comparer = comparer ?? throw new ArgumentNullException();
    }

    public PriorityQueue(IComparer<TValue> comparer)
    {
        this.baseHeap = new List<TValue>();
        this.comparer = comparer ?? throw new ArgumentNullException();
    }

    public PriorityQueue(IEnumerable<TValue> data)
        : this(data, Comparer<TValue>.Default)
    {
    }

    public PriorityQueue(IEnumerable<TValue> data, IComparer<TValue> comparer)
    {
        if (data == null || comparer == null)
        {
            throw new ArgumentNullException();
        }

        this.comparer = comparer;
        this.baseHeap = new List<TValue>(data);
        // heapify data
        for (int pos = (this.baseHeap.Count / 2) - 1; pos >= 0; pos--)
        {
            this.HeapifyFromBeginningToEnd(pos);
        }
    }

    #endregion

    public bool IsEmpty => this.baseHeap.Count == 0;
    public int Count => this.baseHeap.Count;
    public bool IsReadOnly => false;

    #region Merging

    public static PriorityQueue<TValue> MergeQueues(PriorityQueue<TValue> pq1, PriorityQueue<TValue> pq2)
    {
        if (pq1 == null || pq2 == null)
        {
            throw new ArgumentNullException();
        }

        if (pq1.comparer != pq2.comparer)
        {
            throw new InvalidOperationException("Priority queues to be merged must have equal comparers");
        }

        return MergeQueues(pq1, pq2, pq1.comparer);
    }

    public static PriorityQueue<TValue> MergeQueues(PriorityQueue<TValue> pq1, PriorityQueue<TValue> pq2, IComparer<TValue> comparer)
    {
        if (pq1 == null || pq2 == null || comparer == null)
        {
            throw new ArgumentNullException();
        }

        // merge data
        var result = new PriorityQueue<TValue>(pq1.Count + pq2.Count, pq1.comparer);
        result.baseHeap.AddRange(pq1.baseHeap);
        result.baseHeap.AddRange(pq2.baseHeap);
        // heapify data
        for (int pos = (result.baseHeap.Count / 2) - 1; pos >= 0; pos--)
        {
            result.HeapifyFromBeginningToEnd(pos);
        }

        return result;
    }

    #endregion

    #region Priority queue operations

    public void Enqueue(TValue value)
    {
        this.Insert(value);
    }

    public TValue Dequeue()
    {
        if (!this.IsEmpty)
        {
            TValue result = this.baseHeap[0];
            this.DeleteRoot();
            return result;
        }
        else
        {
            throw new InvalidOperationException("Priority queue is empty");
        }
    }

    public TValue Peek()
    {
        if (!this.IsEmpty)
        {
            return this.baseHeap[0];
        }
        else
        {
            throw new InvalidOperationException("Priority queue is empty");
        }
    }

    public bool ValueImproved(TValue value)
    {
        // O(n + logn)
        // A-star 알고리즘 한정 동작. 인자로 넘어온 element의 priority가 높아졌다. 
        // 높아진 것으로 한정할 수 있기 때문에, heap의 앞쪽으로만 재평가해도 무방하다.
        int pos = -1;
        for (int i = 0; i < this.baseHeap.Count; ++i)
        {
            if (this.baseHeap[i].Equals(value))
            {
                pos = i;
                break;
            }
        }

        if (pos < 0)
        { // Error!
            return false;
        }

        this.HeapifyFromEndToBeginning(pos);
        return true;
    }

    public bool ValueDecreased(TValue value)
    {
        // O(n + logn)
        // priority가 낮아진 것으로 확정할 수 있는 경우, heap의 뒷쪽으로만 재평가한다.
        int pos = -1;
        for (int i = 0; i < this.baseHeap.Count; ++i)
        {
            if (this.baseHeap[i].Equals(value))
            {
                pos = i;
                break;
            }
        }

        if (pos < 0)
        { // Error!
            return false;
        }

        this.HeapifyFromBeginningToEnd(pos);
        return true;
    }

    #endregion

    #region ICollection<TValue> implementation

    public void Add(TValue item)
    {
        this.Enqueue(item);
    }

    public void Clear()
    {
        this.baseHeap.Clear();
    }

    public bool Contains(TValue item)
    {
        return this.baseHeap.Contains(item);
    }

    public void CopyTo(TValue[] array, int arrayIndex)
    {
        this.baseHeap.CopyTo(array, arrayIndex);
    }

    public bool Remove(TValue item)
    {
        // find element in the collection and remove it
        int elementIdx = this.baseHeap.IndexOf(item);
        if (elementIdx < 0)
        {
            return false;
        }

        // remove element
        this.baseHeap[elementIdx] = this.baseHeap[this.baseHeap.Count - 1];
        this.baseHeap.RemoveAt(this.baseHeap.Count - 1);

        // heapify
        int newPos = this.HeapifyFromEndToBeginning(elementIdx);
        if (newPos == elementIdx)
        {
            this.HeapifyFromBeginningToEnd(elementIdx);
        }

        return true;
    }

    public IEnumerator<TValue> GetEnumerator()
    {
        return this.baseHeap.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return this.GetEnumerator();
    }

    #endregion

    #region Heap operations

    private void ExchangeElements(int pos1, int pos2)
    {
        TValue val = this.baseHeap[pos1];
        this.baseHeap[pos1] = this.baseHeap[pos2];
        this.baseHeap[pos2] = val;
    }

    private void Insert(TValue value)
    {
        this.baseHeap.Add(value);

        // heap[i] have children heap[2*i + 1] and heap[2*i + 2] and parent heap[(i-1)/ 2];

        // heapify after insert, from end to beginning
        this.HeapifyFromEndToBeginning(this.baseHeap.Count - 1);
    }

    private int HeapifyFromEndToBeginning(int pos)
    {
        if (pos >= this.baseHeap.Count)
        {
            return -1;
        }

        while (pos > 0)
        {
            int parentPos = (pos - 1) / 2;
            if (this.comparer.Compare(this.baseHeap[parentPos], this.baseHeap[pos]) > 0)
            {
                this.ExchangeElements(parentPos, pos);
                pos = parentPos;
            }
            else
            {
                break;
            }
        }

        return pos;
    }

    private void DeleteRoot()
    {
        if (this.baseHeap.Count <= 1)
        {
            this.baseHeap.Clear();
            return;
        }

        this.baseHeap[0] = this.baseHeap[this.baseHeap.Count - 1];
        this.baseHeap.RemoveAt(this.baseHeap.Count - 1);

        // heapify
        this.HeapifyFromBeginningToEnd(0);
    }

    private void HeapifyFromBeginningToEnd(int pos)
    {
        if (pos >= this.baseHeap.Count)
        {
            return;
        }

        // heap[i] have children heap[2*i + 1] and heap[2*i + 2] and parent heap[(i-1)/ 2];
        while (true)
        {
            // on each iteration exchange element with its smallest child
            int smallest = pos;
            int left = (2 * pos) + 1;
            int right = (2 * pos) + 2;
            if (left < this.baseHeap.Count && this.comparer.Compare(this.baseHeap[smallest], this.baseHeap[left]) > 0)
            {
                smallest = left;
            }

            if (right < this.baseHeap.Count && this.comparer.Compare(this.baseHeap[smallest], this.baseHeap[right]) > 0)
            {
                smallest = right;
            }

            if (smallest != pos)
            {
                this.ExchangeElements(smallest, pos);
                pos = smallest;
            }
            else
            {
                break;
            }
        }
    }

    #endregion
}
