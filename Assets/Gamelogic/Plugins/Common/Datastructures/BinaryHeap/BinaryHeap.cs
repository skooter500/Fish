using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

namespace Gamelogic.Internal.BinaryHeap
{
	/// <summary>
	/// A binary heap, useful for sorting data and priority queues.
	/// </summary>
	/// <typeparam name="T">Type of item in the heap</typeparam>
	public class BinaryHeap<T> : ICollection<T>
	{
		// Constants
		private const int DefaultSize = 4;
		// Fields
		private T[] data = new T[DefaultSize];
		private int capacity = DefaultSize;
		private bool sorted;

		private readonly Func<T, T, int> compare; 

		// Properties
		/// <summary>
		/// Gets the number of values in the heap. 
		/// </summary>
		public int Count { get; private set; }

		/// <summary>
		/// Gets or sets the capacity of the heap.
		/// </summary>
		public int Capacity
		{
			get { return capacity; }
			set
			{
				int previousCapacity = capacity;
				capacity = Math.Max(value, Count);
				if (capacity != previousCapacity)
				{
					var temp = new T[capacity];
					Array.Copy(data, temp, Count);
					data = temp;
				}
			}
		}
		// Methods
		/// <summary>
		/// Creates a new binary heap.
		/// </summary>
		public BinaryHeap(Func<T, T, int> compare)
		{
			Count = 0;
			this.compare = compare;
		}

		private BinaryHeap(T[] data, int count)
		{
			Capacity = count;
			Count = count;
			Array.Copy(data, this.data, count);
		}
		/// <summary>
		/// Gets the first value in the heap without removing it.
		/// </summary>
		/// <returns>The lowest value of type TValue.</returns>
		public T Peek()
		{
			return data[0];
		}

		/// <summary>
		/// Removes all items from the heap.
		/// </summary>
		public void Clear()
		{
			Count = 0;
			data = new T[capacity];
		}
		/// <summary>
		/// Adds a key and value to the heap.
		/// </summary>
		/// <param name="item">The item to add to the heap.</param>
		public void Add(T item)
		{
			if (Count == capacity)
			{
				Capacity *= 2;
			}
			data[Count] = item;
			UpHeap();
			Count++;
		}

		/// <summary>
		/// Removes and returns the first item in the heap.
		/// </summary>
		/// <returns>The next value in the heap.</returns>
		public T Remove()
		{
			if (Count == 0)
			{
				throw new InvalidOperationException("Cannot remove item, heap is empty.");
			}
			T v = data[0];
			Count--;
			data[0] = data[Count];
			data[Count] = default(T); //Clears the Last Node
			DownHeap();
			return v;
		}
		private void UpHeap()
		//helper function that performs up-heap bubbling
		{
			sorted = false;
			int p = Count;
			T item = data[p];
			int par = Parent(p);
			while (par > -1 && compare(item,data[par]) < 0)
			{
				data[p] = data[par]; //Swap nodes
				p = par;
				par = Parent(p);
			}
			data[p] = item;
		}
		private void DownHeap()
		//helper function that performs down-heap bubbling
		{
			sorted = false;
			int n;
			int p = 0;
			T item = data[p];
			while (true)
			{
				int ch1 = Child1(p);
				if (ch1 >= Count) break;
				int ch2 = Child2(p);
				if (ch2 >= Count)
				{
					n = ch1;
				}
				else
				{
					n = compare(data[ch1], data[ch2]) < 0 ? ch1 : ch2;
				}
				if (compare(item, data[n]) > 0)
				{
					data[p] = data[n]; //Swap nodes
					p = n;
				}
				else
				{
					break;
				}
			}
			data[p] = item;
		}
		private void EnsureSort()
		{
			if (sorted) return;
			Array.Sort(data, 0, Count);
			sorted = true;
		}
		private static int Parent(int index)
		//helper function that calculates the parent of a node
		{
			return (index - 1) >> 1;
		}
		private static int Child1(int index)
		//helper function that calculates the first child of a node
		{
			return (index << 1) + 1;
		}
		private static int Child2(int index)
		//helper function that calculates the second child of a node
		{
			return (index << 1) + 2;
		}

		/// <summary>
		/// Creates a new instance of an identical binary heap.
		/// </summary>
		/// <returns>A BinaryHeap.</returns>
		public BinaryHeap<T> Copy()
		{
			return new BinaryHeap<T>(data, Count);
		}

		/// <summary>
		/// Gets an enumerator for the binary heap.
		/// </summary>
		/// <returns>An IEnumerator of type T.</returns>
		public IEnumerator<T> GetEnumerator()
		{
			EnsureSort();
			for (int i = 0; i < Count; i++)
			{
				yield return data[i];
			}
		}
		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		/// <summary>
		/// Checks to see if the binary heap contains the specified item.
		/// </summary>
		/// <param name="item">The item to search the binary heap for.</param>
		/// <returns>A boolean, true if binary heap contains item.</returns>
		public bool Contains(T item)
		{
			//EnsureSort();
			//return Array.BinarySearch(data, 0, Count, item) >= 0;
			return data.Contains(item);
		}
		/// <summary>
		/// Copies the binary heap to an array at the specified index.
		/// </summary>
		/// <param name="array">One dimensional array that is the destination of the copied elements.</param>
		/// <param name="arrayIndex">The zero-based index at which copying begins.</param>
		public void CopyTo(T[] array, int arrayIndex)
		{
			EnsureSort();
			Array.Copy(data, array, Count);
		}
		/// <summary>
		/// Gets whether or not the binary heap is readonly.
		/// </summary>
		public bool IsReadOnly
		{
			get { return false; }
		}
		/// <summary>
		/// Removes an item from the binary heap. This utilizes the type T's Comparer and will not remove duplicates.
		/// </summary>
		/// <param name="item">The item to be removed.</param>
		/// <returns>Boolean true if the item was removed.</returns>
		public bool Remove(T item)
		{
			EnsureSort();
			int i = Array.BinarySearch(data, 0, Count, item);
			if (i < 0) return false;
			Array.Copy(data, i + 1, data, i, Count - i);
			data[Count] = default(T);
			Count--;
			return true;
		}

	}
}