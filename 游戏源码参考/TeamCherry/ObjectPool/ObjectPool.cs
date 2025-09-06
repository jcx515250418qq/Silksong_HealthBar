using System;
using System.Collections.Generic;
using UnityEngine;

namespace TeamCherry.ObjectPool
{
	public class ObjectPool<T> : IDisposable, IPoolReleaser<T> where T : UnityEngine.Object, IPoolable<T>
	{
		public delegate T ObjectReturnDelegate();

		public delegate void ObjectPassDelegate(T element);

		protected ObjectReturnDelegate factory;

		protected ObjectPassDelegate onGet;

		protected ObjectPassDelegate onRelease;

		protected ObjectPassDelegate onDestroy;

		private int totalCount;

		private readonly Stack<T> inactiveStack;

		protected int TotalCount => totalCount;

		protected ObjectPool()
		{
			inactiveStack = new Stack<T>();
		}

		public ObjectPool(ObjectReturnDelegate createNewAction, ObjectPassDelegate onGet, ObjectPassDelegate onRelease, ObjectPassDelegate onDestroy, int startCapacity = 10)
			: this()
		{
			factory = createNewAction;
			this.onGet = onGet;
			this.onRelease = onRelease;
			this.onDestroy = onDestroy;
			InitialisePool(startCapacity);
		}

		protected void InitialisePool(int startCapacity)
		{
			for (int i = totalCount; i < startCapacity; i++)
			{
				T element = CreateNew();
				AddNew(element);
			}
		}

		protected void AddNew(T element)
		{
			totalCount++;
			Release(element);
		}

		protected T CreateNew()
		{
			return factory();
		}

		public T Get()
		{
			T val;
			if (inactiveStack.Count < 1)
			{
				val = CreateNew();
				totalCount++;
			}
			else
			{
				val = inactiveStack.Pop();
			}
			onGet?.Invoke(val);
			return val;
		}

		public void Release(T element)
		{
			onRelease?.Invoke(element);
			inactiveStack.Push(element);
		}

		public void Clear()
		{
			if (onDestroy != null)
			{
				foreach (T item in inactiveStack)
				{
					onDestroy(item);
				}
			}
			inactiveStack.Clear();
			totalCount = 0;
		}

		public void Dispose()
		{
			Clear();
		}
	}
}
