using System;

namespace InControl.Internal
{
	public class RingBuffer<T>
	{
		private readonly int size;

		private readonly T[] data;

		private int head;

		private int tail;

		private readonly object sync;

		public RingBuffer(int size)
		{
			if (size <= 0)
			{
				throw new ArgumentException("RingBuffer size must be 1 or greater.");
			}
			this.size = size + 1;
			data = new T[this.size];
			head = 0;
			tail = 0;
			sync = new object();
		}

		public void Enqueue(T value)
		{
			lock (sync)
			{
				if (size > 1)
				{
					head = (head + 1) % size;
					if (head == tail)
					{
						tail = (tail + 1) % size;
					}
				}
				data[head] = value;
			}
		}

		public T Dequeue()
		{
			lock (sync)
			{
				if (size > 1 && tail != head)
				{
					tail = (tail + 1) % size;
				}
				return data[tail];
			}
		}
	}
}
