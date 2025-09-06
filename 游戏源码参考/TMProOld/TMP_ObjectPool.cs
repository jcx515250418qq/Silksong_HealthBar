using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace TMProOld
{
	internal class TMP_ObjectPool<T> where T : new()
	{
		private readonly Stack<T> m_Stack = new Stack<T>();

		private readonly UnityAction<T> m_ActionOnGet;

		private readonly UnityAction<T> m_ActionOnRelease;

		public int countAll { get; private set; }

		public int countActive => countAll - countInactive;

		public int countInactive => m_Stack.Count;

		public TMP_ObjectPool(UnityAction<T> actionOnGet, UnityAction<T> actionOnRelease)
		{
			m_ActionOnGet = actionOnGet;
			m_ActionOnRelease = actionOnRelease;
		}

		public T Get()
		{
			T val;
			if (m_Stack.Count == 0)
			{
				val = new T();
				countAll++;
			}
			else
			{
				val = m_Stack.Pop();
			}
			if (m_ActionOnGet != null)
			{
				m_ActionOnGet(val);
			}
			return val;
		}

		public void Release(T element)
		{
			if (m_Stack.Count > 0 && (object)m_Stack.Peek() == (object)element)
			{
				Debug.LogError("Internal error. Trying to destroy object that is already released to pool.");
			}
			if (m_ActionOnRelease != null)
			{
				m_ActionOnRelease(element);
			}
			m_Stack.Push(element);
		}
	}
}
