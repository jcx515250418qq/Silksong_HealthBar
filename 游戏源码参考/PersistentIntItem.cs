using System;
using UnityEngine;

public class PersistentIntItem : PersistentItem<int>
{
	[Serializable]
	private class PersistentIntData : PersistentItemData<int>
	{
	}

	[SerializeField]
	private PersistentIntData itemData;

	protected override int DefaultValue => -1;

	protected override PersistentItemData<int> SerializedItemData => itemData;

	protected override PlayMakerFSM LookForMyFSM()
	{
		PlayMakerFSM[] components = GetComponents<PlayMakerFSM>();
		if (components == null)
		{
			Debug.LogErrorFormat("Persistent Int Item ({0}) does not have a PlayMakerFSM attached to read value from.", base.name);
			return null;
		}
		return FSMUtility.FindFSMWithPersistentInt(components);
	}

	protected override int GetValueFromFSM(PlayMakerFSM fromFsm)
	{
		return fromFsm.FsmVariables.FindFsmInt("Value").Value;
	}

	protected override void SetValueOnFSM(PlayMakerFSM toFsm, int value)
	{
		toFsm.FsmVariables.FindFsmInt("Value").Value = value;
	}

	protected override void SaveValue(PersistentItemData<int> newItemData)
	{
		SceneData.instance.PersistentInts.SetValue(newItemData);
	}

	protected override bool TryGetValue(ref PersistentItemData<int> newItemData)
	{
		if (SceneData.instance.PersistentInts.TryGetValue(newItemData.SceneName, newItemData.ID, out var value))
		{
			newItemData.Value = value.Value;
			return true;
		}
		return false;
	}
}
