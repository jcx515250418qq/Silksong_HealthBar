using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NamedScriptableObjectList<T> : NamedScriptableObjectListDummy, IList<T>, ICollection<T>, IEnumerable<T>, IEnumerable, IReadOnlyList<T>, IReadOnlyCollection<T> where T : ScriptableObject
{
	[SerializeField]
	[ContextMenuItem("Remove Duplicates", "RemoveDuplicates")]
	[ContextMenuItem("Remove Null Items", "RemoveNullItems")]
	private List<T> list = new List<T>();

	[NonSerialized]
	private Dictionary<string, T> dictionary;

	protected List<T> List => list;

	public int Count => list.Count;

	public bool IsReadOnly => ((ICollection<T>)list).IsReadOnly;

	public T this[int index]
	{
		get
		{
			return list[index];
		}
		set
		{
			list[index] = value;
			UpdateDictionary();
		}
	}

	private void OnEnable()
	{
		RemoveDuplicates();
		RemoveNullItems();
		UpdateDictionary();
	}

	private void UpdateDictionary()
	{
		if (list != null)
		{
			dictionary = (from obj in list
				where obj != null
				group obj by obj.name).ToDictionary((IGrouping<string, T> group) => group.Key, (IGrouping<string, T> group) => group.FirstOrDefault());
		}
	}

	public T GetByName(string itemName)
	{
		if (!string.IsNullOrEmpty(itemName) && dictionary != null && dictionary.ContainsKey(itemName))
		{
			return dictionary[itemName];
		}
		return null;
	}

	public void RemoveDuplicates()
	{
		InternalRemoveDuplicates();
		UpdateDictionary();
	}

	private void InternalRemoveDuplicates()
	{
		list = list.Distinct().ToList();
	}

	public void RemoveNullItems()
	{
		InternalRemoveNullItems();
		UpdateDictionary();
	}

	private void InternalRemoveNullItems()
	{
		list = list.Where((T item) => item != null).ToList();
	}

	public void Add(T item)
	{
		list.Add(item);
		UpdateDictionary();
	}

	public void Clear()
	{
		list.Clear();
		dictionary.Clear();
	}

	public bool Contains(T item)
	{
		return list.Contains(item);
	}

	public void CopyTo(T[] array, int arrayIndex)
	{
		list.CopyTo(array, arrayIndex);
	}

	public IEnumerator<T> GetEnumerator()
	{
		return list.GetEnumerator();
	}

	public bool Remove(T item)
	{
		bool result = list.Remove(item);
		UpdateDictionary();
		return result;
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return list.GetEnumerator();
	}

	public int IndexOf(T item)
	{
		return list.IndexOf(item);
	}

	public void Insert(int index, T item)
	{
		list.Insert(index, item);
		UpdateDictionary();
	}

	public void RemoveAt(int index)
	{
		list.RemoveAt(index);
		UpdateDictionary();
	}
}
