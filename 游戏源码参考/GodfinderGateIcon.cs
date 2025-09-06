using System;
using UnityEngine;

public class GodfinderGateIcon : MonoBehaviour
{
	public enum IconType
	{
		Bound = 0,
		Unbound = 1,
		Complete = 2,
		CompleteRadiant = 3
	}

	[ArrayForEnum(typeof(IconType))]
	public GameObject[] icons;

	public string completionPD;

	[Tooltip("If assigned, icon will show unlocked when tier CAN be unlocked, rather than when the lock has been broken.")]
	public BossSequence unlockedSequence;

	public string requiredPDBool;

	private void Reset()
	{
		OnValidate();
	}

	private void OnValidate()
	{
		int num = Enum.GetNames(typeof(IconType)).Length;
		if (icons.Length != num)
		{
			GameObject[] array = icons;
			icons = new GameObject[num];
			for (int i = 0; i < array.Length; i++)
			{
				icons[i] = array[i];
			}
		}
	}

	private void SetIcon(IconType type)
	{
		for (int i = 0; i < icons.Length; i++)
		{
			if ((bool)icons[i])
			{
				icons[i].SetActive(i == (int)type);
			}
		}
	}

	public void Evaluate()
	{
		if (!string.IsNullOrEmpty(requiredPDBool) && !GameManager.instance.GetPlayerDataBool(requiredPDBool))
		{
			base.gameObject.SetActive(value: false);
			return;
		}
		BossSequenceDoor.Completion playerDataVariable = GameManager.instance.GetPlayerDataVariable<BossSequenceDoor.Completion>(completionPD);
		if (playerDataVariable.allBindings)
		{
			SetIcon(IconType.CompleteRadiant);
		}
		else if (playerDataVariable.completed)
		{
			SetIcon(IconType.Complete);
		}
		else if (playerDataVariable.unlocked || playerDataVariable.canUnlock || ((bool)unlockedSequence && unlockedSequence.IsUnlocked()))
		{
			SetIcon(IconType.Unbound);
		}
		else
		{
			SetIcon(IconType.Bound);
		}
		base.gameObject.SetActive(value: true);
	}
}
