using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using UnityEngine;

[Serializable]
[JsonObject(MemberSerialization.OptIn)]
public abstract class SerializableNamedList<TData, TContainer> : ISerializationCallbackReceiver where TContainer : SerializableNamedData<TData>, new()
{
	[SerializeField]
	[JsonProperty]
	private List<TContainer> savedData = new List<TContainer>();

	[NonSerialized]
	protected Dictionary<string, TData> RuntimeData = new Dictionary<string, TData>();

	[OnDeserialized]
	private void OnDeserialized(StreamingContext context)
	{
		OnAfterDeserialize();
	}

	public void OnAfterDeserialize()
	{
		RuntimeData = (from named in savedData
			group named by named.Name).ToDictionary((IGrouping<string, TContainer> group) => group.Key, (IGrouping<string, TContainer> group) => group.FirstOrDefault().Data);
		if (RuntimeData.Count < savedData.Count && !RuntimeData.ContainsKey(string.Empty))
		{
			RuntimeData[string.Empty] = default(TData);
		}
	}

	[OnSerializing]
	private void OnSerializing(StreamingContext context)
	{
		OnBeforeSerialize();
	}

	public void OnBeforeSerialize()
	{
		savedData = RuntimeData.Select((KeyValuePair<string, TData> kvp) => new TContainer
		{
			Name = kvp.Key,
			Data = kvp.Value
		}).ToList();
	}

	public void SetData(string itemName, TData data)
	{
		if (data is ISerializableNamedDataRedundancy { IsDataRedundant: not false })
		{
			if (RuntimeData.ContainsKey(itemName))
			{
				RuntimeData.Remove(itemName);
			}
		}
		else
		{
			RuntimeData[itemName] = data;
		}
	}

	public TData GetData(string itemName)
	{
		return RuntimeData.GetValueOrDefault(itemName);
	}

	public List<string> GetValidNames(Func<TData, bool> predicate = null)
	{
		if (predicate == null)
		{
			return RuntimeData.Keys.ToList();
		}
		return RuntimeData.Keys.Where((string name) => predicate(GetData(name))).ToList();
	}

	public List<TData> GetValidDatas(Func<TData, bool> predicate = null)
	{
		if (predicate == null)
		{
			return RuntimeData.Values.ToList();
		}
		return RuntimeData.Values.Where(predicate).ToList();
	}

	public bool IsAnyMatching(Func<TData, bool> predicate)
	{
		if (predicate == null)
		{
			return false;
		}
		foreach (TData value in RuntimeData.Values)
		{
			if (predicate(value))
			{
				return true;
			}
		}
		return false;
	}

	public IEnumerable<KeyValuePair<string, TData>> Enumerate()
	{
		foreach (KeyValuePair<string, TData> runtimeDatum in RuntimeData)
		{
			yield return runtimeDatum;
		}
	}
}
