using System.Collections.Generic;

namespace InControl
{
	internal class ThreadSafeQueue<T>
	{
		private object sync;

		private Queue<T> data;

		public ThreadSafeQueue()
		{
			sync = new object();
			data = new Queue<T>();
		}

		public ThreadSafeQueue(int capacity)
		{
			sync = new object();
			data = new Queue<T>(capacity);
		}

		public void Enqueue(T item)
		{
			lock (sync)
			{
				data.Enqueue(item);
			}
		}

		public bool Dequeue(out T item)
		{
			lock (sync)
			{
				if (data.Count > 0)
				{
					item = data.Dequeue();
					return true;
				}
			}
			item = default(T);
			return false;
		}

		public T Dequeue()
		{
			lock (sync)
			{
				if (data.Count > 0)
				{
					return data.Dequeue();
				}
			}
			return default(T);
		}

		public int Dequeue(ref IList<T> list)
		{
			lock (sync)
			{
				int count = data.Count;
				for (int i = 0; i < count; i++)
				{
					list.Add(data.Dequeue());
				}
				return count;
			}
		}
	}
}
