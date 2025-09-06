using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using UnityEngine;

[Serializable]
[JsonObject(MemberSerialization.Fields)]
public class SceneData
{
	public enum PersistentMutatorTypes
	{
		None = 0,
		Mimic = 1
	}

	[Serializable]
	public class SerializableItemData<T>
	{
		public string SceneName;

		public string ID;

		public T Value;

		public PersistentMutatorTypes Mutator;
	}

	[Serializable]
	[JsonObject(MemberSerialization.Fields)]
	public class PersistentItemDataCollection<TValue, TContainer> : ISerializationCallbackReceiver where TContainer : SerializableItemData<TValue>, new()
	{
		[SerializeField]
		private List<TContainer> serializedList = new List<TContainer>();

		[NonSerialized]
		private Dictionary<string, Dictionary<string, PersistentItemData<TValue>>> scenes = new Dictionary<string, Dictionary<string, PersistentItemData<TValue>>>();

		[OnDeserialized]
		private void OnDeserialized(StreamingContext context)
		{
			OnAfterDeserialize();
		}

		public void OnAfterDeserialize()
		{
			if (scenes == null)
			{
				scenes = new Dictionary<string, Dictionary<string, PersistentItemData<TValue>>>();
			}
			else
			{
				Dictionary<string, Dictionary<string, PersistentItemData<TValue>>> dictionary = new Dictionary<string, Dictionary<string, PersistentItemData<TValue>>>();
				foreach (KeyValuePair<string, Dictionary<string, PersistentItemData<TValue>>> scene in scenes)
				{
					foreach (KeyValuePair<string, PersistentItemData<TValue>> item in scene.Value)
					{
						if (item.Value.IsSemiPersistent)
						{
							if (!dictionary.ContainsKey(scene.Key))
							{
								dictionary[scene.Key] = new Dictionary<string, PersistentItemData<TValue>>();
							}
							dictionary[scene.Key][item.Key] = item.Value;
						}
					}
				}
				scenes = dictionary;
			}
			foreach (TContainer serialized in serializedList)
			{
				if (!scenes.ContainsKey(serialized.SceneName))
				{
					scenes[serialized.SceneName] = new Dictionary<string, PersistentItemData<TValue>>();
				}
				scenes[serialized.SceneName][serialized.ID] = new PersistentItemData<TValue>
				{
					SceneName = serialized.SceneName,
					ID = serialized.ID,
					Value = serialized.Value,
					Mutator = serialized.Mutator
				};
			}
		}

		[OnSerializing]
		private void OnSerializing(StreamingContext context)
		{
			OnBeforeSerialize();
		}

		public void OnBeforeSerialize()
		{
			serializedList = new List<TContainer>();
			foreach (KeyValuePair<string, Dictionary<string, PersistentItemData<TValue>>> scene in scenes)
			{
				foreach (KeyValuePair<string, PersistentItemData<TValue>> item in scene.Value)
				{
					if (!item.Value.IsSemiPersistent)
					{
						serializedList.Add(new TContainer
						{
							SceneName = item.Value.SceneName,
							ID = item.Value.ID,
							Value = item.Value.Value,
							Mutator = item.Value.Mutator
						});
					}
				}
			}
		}

		public void SetValue(PersistentItemData<TValue> itemData)
		{
			if (!scenes.ContainsKey(itemData.SceneName))
			{
				scenes[itemData.SceneName] = new Dictionary<string, PersistentItemData<TValue>>();
			}
			scenes[itemData.SceneName][itemData.ID] = itemData;
		}

		public bool TryGetValue(string sceneName, string id, out PersistentItemData<TValue> value)
		{
			if (scenes.ContainsKey(sceneName) && scenes[sceneName].ContainsKey(id))
			{
				value = scenes[sceneName][id];
				return true;
			}
			value = null;
			return false;
		}

		public TValue GetValueOrDefault(string sceneName, string id)
		{
			if (!TryGetValue(sceneName, id, out var value))
			{
				return default(TValue);
			}
			return value.Value;
		}

		public void ResetSemiPersistent()
		{
			Dictionary<string, Dictionary<string, PersistentItemData<TValue>>> dictionary = new Dictionary<string, Dictionary<string, PersistentItemData<TValue>>>();
			foreach (KeyValuePair<string, Dictionary<string, PersistentItemData<TValue>>> scene in scenes)
			{
				Dictionary<string, PersistentItemData<TValue>> dictionary2 = new Dictionary<string, PersistentItemData<TValue>>();
				foreach (KeyValuePair<string, PersistentItemData<TValue>> item in scene.Value)
				{
					if (!item.Value.IsSemiPersistent)
					{
						dictionary2.Add(item.Key, item.Value);
					}
				}
				dictionary.Add(scene.Key, dictionary2);
			}
			scenes = dictionary;
		}

		public void Mutate(Action<PersistentItemData<TValue>> mutateAction)
		{
			if (mutateAction == null)
			{
				return;
			}
			foreach (KeyValuePair<string, Dictionary<string, PersistentItemData<TValue>>> scene in scenes)
			{
				foreach (KeyValuePair<string, PersistentItemData<TValue>> item in scene.Value)
				{
					mutateAction(item.Value);
				}
			}
		}

		public bool Remove(string sceneName, string id)
		{
			if (!scenes.TryGetValue(sceneName, out var value))
			{
				return false;
			}
			return value.Remove(id);
		}
	}

	[Serializable]
	public class SerializableBoolData : SerializableItemData<bool>
	{
	}

	[Serializable]
	private class PersistentBoolCollection : PersistentItemDataCollection<bool, SerializableBoolData>
	{
	}

	[Serializable]
	public class SerializableIntData : SerializableItemData<int>
	{
	}

	[Serializable]
	private class PersistentIntCollection : PersistentItemDataCollection<int, SerializableIntData>
	{
	}

	[SerializeField]
	private PersistentBoolCollection persistentBools;

	[SerializeField]
	private PersistentIntCollection persistentInts;

	[SerializeField]
	private PersistentIntCollection geoRocks;

	private static SceneData _instance;

	public PersistentItemDataCollection<bool, SerializableBoolData> PersistentBools => persistentBools;

	public PersistentItemDataCollection<int, SerializableIntData> PersistentInts => persistentInts;

	public static SceneData instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = new SceneData();
			}
			return _instance;
		}
		set
		{
			_instance = value;
		}
	}

	public SceneData()
	{
		SetupNewSceneData();
	}

	public void Reset()
	{
		SetupNewSceneData();
	}

	public void SaveMyState(GeoRockData geoRockData)
	{
		geoRocks.SetValue(new PersistentItemData<int>
		{
			SceneName = geoRockData.sceneName,
			ID = geoRockData.id,
			Value = geoRockData.hitsLeft
		});
	}

	public GeoRockData FindMyState(GeoRockData grd)
	{
		if (geoRocks.TryGetValue(grd.sceneName, grd.id, out var value))
		{
			return new GeoRockData
			{
				sceneName = value.SceneName,
				id = value.ID,
				hitsLeft = value.Value
			};
		}
		return null;
	}

	public void ResetSemiPersistentItems()
	{
		persistentBools.ResetSemiPersistent();
		persistentInts.ResetSemiPersistent();
		geoRocks.ResetSemiPersistent();
	}

	private void SetupNewSceneData()
	{
		persistentBools = new PersistentBoolCollection();
		persistentInts = new PersistentIntCollection();
		geoRocks = new PersistentIntCollection();
	}

	public void MimicShuffle()
	{
		persistentInts.Mutate(delegate(PersistentItemData<int> data)
		{
			if (data.Mutator == PersistentMutatorTypes.Mimic && data.Value == 0 && UnityEngine.Random.Range(0, 50) == 0)
			{
				data.Value = -10;
			}
		});
	}
}
