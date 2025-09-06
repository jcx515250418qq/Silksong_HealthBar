using System;
using System.Collections.Generic;

namespace TMProOld
{
	public class FastAction
	{
		private LinkedList<Action> delegates = new LinkedList<Action>();

		private Dictionary<Action, LinkedListNode<Action>> lookup = new Dictionary<Action, LinkedListNode<Action>>();

		public void Add(Action rhs)
		{
			if (!lookup.ContainsKey(rhs))
			{
				lookup[rhs] = delegates.AddLast(rhs);
			}
		}

		public void Remove(Action rhs)
		{
			if (lookup.TryGetValue(rhs, out var value))
			{
				lookup.Remove(rhs);
				delegates.Remove(value);
			}
		}

		public void Call()
		{
			for (LinkedListNode<Action> linkedListNode = delegates.First; linkedListNode != null; linkedListNode = linkedListNode.Next)
			{
				linkedListNode.Value();
			}
		}
	}
	public class FastAction<A>
	{
		private LinkedList<Action<A>> delegates = new LinkedList<Action<A>>();

		private Dictionary<Action<A>, LinkedListNode<Action<A>>> lookup = new Dictionary<Action<A>, LinkedListNode<Action<A>>>();

		public void Add(Action<A> rhs)
		{
			if (!lookup.ContainsKey(rhs))
			{
				lookup[rhs] = delegates.AddLast(rhs);
			}
		}

		public void Remove(Action<A> rhs)
		{
			if (lookup.TryGetValue(rhs, out var value))
			{
				lookup.Remove(rhs);
				delegates.Remove(value);
			}
		}

		public void Call(A a)
		{
			for (LinkedListNode<Action<A>> linkedListNode = delegates.First; linkedListNode != null; linkedListNode = linkedListNode.Next)
			{
				linkedListNode.Value(a);
			}
		}
	}
	public class FastAction<A, B>
	{
		private LinkedList<Action<A, B>> delegates = new LinkedList<Action<A, B>>();

		private Dictionary<Action<A, B>, LinkedListNode<Action<A, B>>> lookup = new Dictionary<Action<A, B>, LinkedListNode<Action<A, B>>>();

		public void Add(Action<A, B> rhs)
		{
			if (!lookup.ContainsKey(rhs))
			{
				lookup[rhs] = delegates.AddLast(rhs);
			}
		}

		public void Remove(Action<A, B> rhs)
		{
			if (lookup.TryGetValue(rhs, out var value))
			{
				lookup.Remove(rhs);
				delegates.Remove(value);
			}
		}

		public void Call(A a, B b)
		{
			for (LinkedListNode<Action<A, B>> linkedListNode = delegates.First; linkedListNode != null; linkedListNode = linkedListNode.Next)
			{
				linkedListNode.Value(a, b);
			}
		}
	}
	public class FastAction<A, B, C>
	{
		private LinkedList<Action<A, B, C>> delegates = new LinkedList<Action<A, B, C>>();

		private Dictionary<Action<A, B, C>, LinkedListNode<Action<A, B, C>>> lookup = new Dictionary<Action<A, B, C>, LinkedListNode<Action<A, B, C>>>();

		public void Add(Action<A, B, C> rhs)
		{
			if (!lookup.ContainsKey(rhs))
			{
				lookup[rhs] = delegates.AddLast(rhs);
			}
		}

		public void Remove(Action<A, B, C> rhs)
		{
			if (lookup.TryGetValue(rhs, out var value))
			{
				lookup.Remove(rhs);
				delegates.Remove(value);
			}
		}

		public void Call(A a, B b, C c)
		{
			for (LinkedListNode<Action<A, B, C>> linkedListNode = delegates.First; linkedListNode != null; linkedListNode = linkedListNode.Next)
			{
				linkedListNode.Value(a, b, c);
			}
		}
	}
}
