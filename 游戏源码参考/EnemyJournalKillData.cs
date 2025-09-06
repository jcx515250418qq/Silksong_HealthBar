using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using UnityEngine;

[Serializable]
[JsonObject(MemberSerialization.OptIn)]
public class EnemyJournalKillData : ISerializationCallbackReceiver
{
	[Serializable]
	public struct KillData
	{
		public int Kills;

		public bool HasBeenSeen;
	}

	[Serializable]
	public struct NamedKillData
	{
		public string Name;

		public KillData Record;
	}

	[SerializeField]
	[JsonProperty]
	private List<NamedKillData> list;

	[NonSerialized]
	private Dictionary<string, KillData> dictionary;

	public Dictionary<string, KillData> Dictionary => dictionary;

	public EnemyJournalKillData()
	{
		list = new List<NamedKillData>();
		dictionary = new Dictionary<string, KillData>();
	}

	public EnemyJournalKillData(List<NamedKillData> startingList)
	{
		list = startingList;
		OnAfterDeserialize();
	}

	[OnDeserialized]
	private void OnDeserialized(StreamingContext context)
	{
		OnAfterDeserialize();
	}

	public void OnAfterDeserialize()
	{
		dictionary = (from namedRecord in list
			group namedRecord by namedRecord.Name).ToDictionary((IGrouping<string, NamedKillData> group) => group.Key, (IGrouping<string, NamedKillData> group) => group.FirstOrDefault().Record);
	}

	[OnSerializing]
	private void OnSerializing(StreamingContext context)
	{
		OnBeforeSerialize();
	}

	public void OnBeforeSerialize()
	{
		list = dictionary.Select(delegate(KeyValuePair<string, KillData> pair)
		{
			NamedKillData result = default(NamedKillData);
			result.Name = pair.Key;
			result.Record = pair.Value;
			return result;
		}).ToList();
	}

	public void RecordKillData(string journalRecordName, KillData killData)
	{
		dictionary[journalRecordName] = killData;
	}

	public KillData GetKillData(string journalRecordName)
	{
		if (!dictionary.TryGetValue(journalRecordName, out var value))
		{
			return default(KillData);
		}
		return value;
	}
}
