using System;
using System.Collections.Generic;

namespace Engine.AStar
{
    /// <summary>
    /// Min-heap priority queue implementation for compatibility with Mono/.NET Framework (Unity)
    /// Compatible alternative to .NET 6+ PriorityQueue
    /// </summary>
    public class PriorityQueue<TElement, TPriority> where TPriority : IComparable<TPriority>
    {
        private List<(TElement element, TPriority priority)> heap;

        public int Count => heap.Count;

        public PriorityQueue()
        {
            heap = new List<(TElement, TPriority)>();
        }

        public void Enqueue(TElement element, TPriority priority)
        {
            heap.Add((element, priority));
            HeapifyUp(heap.Count - 1);
        }

        public TElement Dequeue()
        {
            if (heap.Count == 0)
                throw new InvalidOperationException("Queue is empty");

            var result = heap[0].element;

            // Move last element to root
            heap[0] = heap[heap.Count - 1];
            heap.RemoveAt(heap.Count - 1);

            if (heap.Count > 0)
            {
                HeapifyDown(0);
            }

            return result;
        }

        public TElement Peek()
        {
            if (heap.Count == 0)
                throw new InvalidOperationException("Queue is empty");

            return heap[0].element;
        }

        private void HeapifyUp(int index)
        {
            while (index > 0)
            {
                int parentIndex = (index - 1) / 2;

                // Min-heap: parent should have lower priority
                if (heap[index].priority.CompareTo(heap[parentIndex].priority) >= 0)
                    break;

                // Swap with parent
                var temp = heap[index];
                heap[index] = heap[parentIndex];
                heap[parentIndex] = temp;

                index = parentIndex;
            }
        }

        private void HeapifyDown(int index)
        {
            while (true)
            {
                int smallest = index;
                int leftChild = 2 * index + 1;
                int rightChild = 2 * index + 2;

                // Find smallest among node and its children
                if (leftChild < heap.Count &&
                    heap[leftChild].priority.CompareTo(heap[smallest].priority) < 0)
                {
                    smallest = leftChild;
                }

                if (rightChild < heap.Count &&
                    heap[rightChild].priority.CompareTo(heap[smallest].priority) < 0)
                {
                    smallest = rightChild;
                }

                // If node is already smallest, we're done
                if (smallest == index)
                    break;

                // Swap with smallest child
                var temp = heap[index];
                heap[index] = heap[smallest];
                heap[smallest] = temp;

                index = smallest;
            }
        }

        public void Clear()
        {
            heap.Clear();
        }
    }
}