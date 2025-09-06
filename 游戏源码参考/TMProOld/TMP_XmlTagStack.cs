namespace TMProOld
{
	public struct TMP_XmlTagStack<T>
	{
		public T[] itemStack;

		public int index;

		public TMP_XmlTagStack(T[] tagStack)
		{
			itemStack = tagStack;
			index = 0;
		}

		public void Clear()
		{
			index = 0;
		}

		public void SetDefault(T item)
		{
			if (itemStack == null)
			{
				itemStack = new T[16];
			}
			if (itemStack != null && itemStack.Length != 0)
			{
				itemStack[0] = item;
				index = 1;
			}
		}

		public void Add(T item)
		{
			if (index < itemStack.Length)
			{
				itemStack[index] = item;
				index++;
			}
		}

		public T Remove()
		{
			index--;
			if (index <= 0)
			{
				index = 0;
				return itemStack[0];
			}
			return itemStack[index - 1];
		}

		public T CurrentItem()
		{
			if (index > 0)
			{
				return itemStack[index - 1];
			}
			return itemStack[0];
		}

		public T PreviousItem()
		{
			if (index > 1)
			{
				return itemStack[index - 2];
			}
			return itemStack[0];
		}
	}
}
