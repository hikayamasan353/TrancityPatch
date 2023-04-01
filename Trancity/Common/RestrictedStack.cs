using System;
using System.Collections;
using System.Collections.Generic;

namespace Common
{
	public class RestrictedStack<T> : IEnumerable, ICollection
	{
		private readonly LinkedList<T> _underhoodItemList;

		private int _capacity;

		public int Capacity => _capacity;

		public int Count => _underhoodItemList.Count;

		public object SyncRoot
		{
			get
			{
				throw new NotSupportedException();
			}
		}

		public bool IsSynchronized
		{
			get
			{
				throw new NotSupportedException();
			}
		}

		public RestrictedStack(int capacity)
		{
			if (capacity < 1)
			{
				throw new ArgumentOutOfRangeException("capacity", "Capacity must be >= 0");
			}
			_underhoodItemList = new LinkedList<T>();
			_capacity = capacity;
		}

		public virtual void Push(T value)
		{
			for (int num = Count; num >= Capacity; num--)
			{
				_underhoodItemList.RemoveLast();
			}
			_underhoodItemList.AddFirst(value);
		}

		public virtual T Pop()
		{
			if (Count == 0)
			{
				throw new InvalidOperationException("Can not pop from empty stack");
			}
			T value = _underhoodItemList.First.Value;
			_underhoodItemList.RemoveFirst();
			return value;
		}

		public virtual T Peek()
		{
			if (Count == 0)
			{
				throw new InvalidOperationException("Can not peek from empty stack");
			}
			return _underhoodItemList.First.Value;
		}

		public void Clear()
		{
			_underhoodItemList.Clear();
		}

		public void CopyTo(Array array, int index)
		{
			T[] array2 = array as T[];
			_underhoodItemList.CopyTo(array2, index);
		}

		public IEnumerator GetEnumerator()
		{
			return _underhoodItemList.GetEnumerator();
		}
	}
}
