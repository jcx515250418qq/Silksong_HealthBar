using System.Collections.Generic;
using HutongGames.PlayMaker;
using TeamCherry.SharedUtils;
using UnityEngine;

public class PlayerDataBoolFsmGroup : MonoBehaviour
{
	[SerializeField]
	private GameObject parent;

	[SerializeField]
	[PlayerDataField(typeof(bool), true)]
	private string pdBool;

	[Space]
	[SerializeField]
	private GameObject[] activateObjects;

	[Space]
	[SerializeField]
	private string sendEventOnUnlock;

	private bool wasUnlocked;

	private readonly List<FsmBool> activatedBools = new List<FsmBool>();

	private void Start()
	{
		if (!parent)
		{
			base.enabled = false;
			return;
		}
		if (!string.IsNullOrEmpty(pdBool) && PlayerData.instance.GetVariable<bool>(pdBool))
		{
			if ((bool)parent)
			{
				parent.SetActive(value: false);
			}
			Unlock();
			return;
		}
		activateObjects.SetAllActive(value: false);
		PlayMakerFSM[] componentsInChildren = parent.GetComponentsInChildren<PlayMakerFSM>(includeInactive: true);
		foreach (PlayMakerFSM obj in componentsInChildren)
		{
			FsmGameObject fsmGameObject = obj.FsmVariables.FindFsmGameObject("Notify Group");
			if (fsmGameObject != null)
			{
				fsmGameObject.Value = base.gameObject;
			}
			FsmBool fsmBool = obj.FsmVariables.FindFsmBool("Activated");
			if (fsmBool != null)
			{
				activatedBools.Add(fsmBool);
			}
		}
	}

	public void UpdateCrustUnlock()
	{
		if (string.IsNullOrEmpty(pdBool))
		{
			return;
		}
		foreach (FsmBool activatedBool in activatedBools)
		{
			if (!activatedBool.Value)
			{
				return;
			}
		}
		PlayerData.instance.SetVariable(pdBool, value: true);
		Unlock();
		if (!string.IsNullOrEmpty(sendEventOnUnlock))
		{
			GameObject[] array = activateObjects;
			for (int i = 0; i < array.Length; i++)
			{
				FSMUtility.SendEventToGameObject(array[i], sendEventOnUnlock);
			}
		}
	}

	private void Unlock()
	{
		if (!wasUnlocked)
		{
			wasUnlocked = true;
			activateObjects.SetAllActive(value: true);
		}
	}
}
