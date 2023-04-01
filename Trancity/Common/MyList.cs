using System;
using System.Collections;
using System.Collections.Generic;

namespace Common
{
	public class MyList : ICollection, IEnumerable
	{
		public delegate void Event();

		public object[] array;

		private MyList[] type_lists;

		private List<Type> types;

		public int Count => array.Length;

		public bool IsSynchronized => array.IsSynchronized;

		public object this[int index]
		{
			get
			{
				if (index >= 0 && index < array.Length)
				{
					return array[index];
				}
				return null;
			}
			set
			{
				if (index >= 0 && index < array.Length)
				{
					array[index] = value;
				}
			}
		}

		public object SyncRoot => array.SyncRoot;

		public event Event changed;

		public MyList(params Type[] object_types)
		{
			array = new object[0];
			types = new List<Type>();
			type_lists = new MyList[0];
			if (object_types.Length != 0)
			{
				types.AddRange(object_types);
				type_lists = new MyList[types.Count];
				for (int i = 0; i < type_lists.Length; i++)
				{
					type_lists[i] = new MyList();
				}
			}
		}

		public MyList(ICollection c, params Type[] object_types)
		{
			array = new object[0];
			types = new List<Type>();
			type_lists = new MyList[0];
			ArrayList arrayList = new ArrayList(c);
			array = arrayList.ToArray();
			if (object_types.Length == 0)
			{
				return;
			}
			types.AddRange(object_types);
			type_lists = new MyList[types.Count];
			for (int i = 0; i < type_lists.Length; i++)
			{
				type_lists[i] = new MyList();
			}
			foreach (object item in arrayList)
			{
				int num = types.IndexOf(item.GetType());
				if (num >= 0)
				{
					type_lists[num].Add(item);
				}
			}
		}

		public virtual void Add(object value)
		{
			ArrayList arrayList = new ArrayList(array);
			arrayList.Add(value);
			array = arrayList.ToArray();
			int num = types.IndexOf(value.GetType());
			if (num >= 0)
			{
				type_lists[num].Add(value);
			}
			if (this.changed != null)
			{
				this.changed();
			}
		}

		public virtual void AddRange(ICollection c)
		{
			ArrayList arrayList = new ArrayList(array);
			arrayList.AddRange(c);
			array = arrayList.ToArray();
			if (types.Count > 0)
			{
				foreach (object item in c)
				{
					int num = types.IndexOf(item.GetType());
					if (num >= 0)
					{
						type_lists[num].Add(item);
					}
				}
			}
			if (this.changed != null)
			{
				this.changed();
			}
		}

		public void Clear()
		{
			this.array = new object[0];
			MyList[] array = type_lists;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].Clear();
			}
			if (this.changed != null)
			{
				this.changed();
			}
		}

		public MyList Clone()
		{
			return new MyList(array, types.ToArray());
		}

		public bool Contains(object item)
		{
			return new ArrayList(array).Contains(item);
		}

		public void CopyTo(Array array, int index)
		{
			this.array.CopyTo(array, index);
		}

		public T[] Get_array<T>()
		{
			int num = types.IndexOf(typeof(T));
			if (num >= 0)
			{
				return (T[])type_lists[num].ToArray(typeof(T));
			}
			return new T[0];
		}

		public IEnumerator GetEnumerator()
		{
			return array.GetEnumerator();
		}

		public int IndexOf(object item)
		{
			return new ArrayList(array).IndexOf(item);
		}

		public void Insert(int index, object value)
		{
			ArrayList arrayList = new ArrayList(array);
			arrayList.Insert(index, value);
			array = arrayList.ToArray();
			int num = types.IndexOf(value.GetType());
			if (num >= 0)
			{
				type_lists[num].Insert(index, value);
			}
			if (this.changed != null)
			{
				this.changed();
			}
		}

		public static implicit operator MyList(object[] list)
		{
			return new MyList(list);
		}

		public static implicit operator object[](MyList list)
		{
			return list.array;
		}

		public void Remove(object value)
		{
			ArrayList arrayList = new ArrayList(array);
			arrayList.Remove(value);
			array = arrayList.ToArray();
			int num = types.IndexOf(value.GetType());
			if (num >= 0)
			{
				type_lists[num].Remove(value);
			}
			if (this.changed != null)
			{
				this.changed();
			}
		}

		public void RemoveAt(int index)
		{
			object obj = array[index];
			int num = types.IndexOf(obj.GetType());
			if (num >= 0)
			{
                int i_current=type_lists[num].IndexOf(obj);



				type_lists[num].RemoveAt(i_current);
			}
			ArrayList arrayList = new ArrayList(array);
			arrayList.RemoveAt(index);
			array = arrayList.ToArray();
			if (this.changed != null)
			{
				this.changed();
			}
		}

		public object[] ToArray()
		{
			return new ArrayList(array).ToArray();
		}

		public Array ToArray(Type type)
		{
			return new ArrayList(array).ToArray(type);
		}
	}
}
