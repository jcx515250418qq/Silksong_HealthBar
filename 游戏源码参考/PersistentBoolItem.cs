using System;
using UnityEngine;

public class PersistentBoolItem : PersistentItem<bool>
{
	[Serializable]
	private class PersistentBoolData : PersistentItemData<bool>
	{
	}

	[SerializeField]
	private PersistentBoolData itemData;

	[Space]
	[SerializeField]
	private bool disableIfActivated;

	[SerializeField]
	private GameObject disablePrefabIfActivated;

	protected override bool DefaultValue => false;

	protected override PersistentItemData<bool> SerializedItemData => itemData;

	protected override void Awake()
	{
		if (itemData == null)
		{
			itemData = new PersistentBoolData();
		}
		base.Awake();
		if ((bool)disablePrefabIfActivated)
		{
			base.OnSetSaveState += delegate(bool value)
			{
				if (value)
				{
					disablePrefabIfActivated.SetActive(value: false);
				}
			};
		}
		if (!disableIfActivated)
		{
			return;
		}
		base.OnSetSaveState += delegate(bool value)
		{
			if (value)
			{
				base.gameObject.SetActive(value: false);
			}
		};
	}

	protected override PlayMakerFSM LookForMyFSM()
	{
		PlayMakerFSM[] components = GetComponents<PlayMakerFSM>();
		if (components == null)
		{
			return null;
		}
		return FSMUtility.FindFSMWithPersistentBool(components);
	}

	protected override bool GetValueFromFSM(PlayMakerFSM fromFsm)
	{
		return fromFsm.FsmVariables.FindFsmBool("Activated").Value;
	}

	protected override void SetValueOnFSM(PlayMakerFSM toFsm, bool value)
	{
		toFsm.FsmVariables.FindFsmBool("Activated").Value = value;
	}

	protected override void SaveValue(PersistentItemData<bool> newItemData)
	{
		SceneData.instance.PersistentBools.SetValue(newItemData);
	}

	protected override bool TryGetValue(ref PersistentItemData<bool> newItemData)
	{
		if (SceneData.instance.PersistentBools.TryGetValue(newItemData.SceneName, newItemData.ID, out var value))
		{
			newItemData.Value = value.Value;
			return true;
		}
		return false;
	}

	public new void SetValueOverride(bool value)
	{
		base.SetValueOverride(value);
	}
}
