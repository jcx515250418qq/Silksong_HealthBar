using System;
using System.Collections;
using GlobalSettings;
using TeamCherry.SharedUtils;
using UnityEngine;

public class BellShrineTuningForkGroup : MonoBehaviour
{
	[Serializable]
	private class BellShrine
	{
		public Animator Icon;

		public BellShrineTuningFork TuningFork;
	}

	[SerializeField]
	[PlayerDataField(typeof(uint), true)]
	private string pdActivatedBitmask;

	[SerializeField]
	private string awardAchievementOnComplete;

	[SerializeField]
	private BellShrine[] bellShrines;

	[SerializeField]
	private Animator finalIcon;

	[SerializeField]
	private PlayMakerFSM eventFsm;

	[SerializeField]
	private float iconsBeginDelay;

	[SerializeField]
	private float iconsActivateDelay;

	[SerializeField]
	private float tuningForksBeginDelay;

	[SerializeField]
	private float tuningForksActivateDelay;

	[SerializeField]
	private MinMaxFloat tuningForksCompleteDelay;

	[SerializeField]
	private AudioEvent iconAppearSound;

	[SerializeField]
	private AudioEvent iconPreActivateSound;

	[SerializeField]
	private AudioEvent iconActivateSound;

	private static readonly int _appearInactiveId = Animator.StringToHash("Appear Inactive");

	private static readonly int _appearActiveId = Animator.StringToHash("Appear Active");

	private static readonly int _preActivateId = Animator.StringToHash("Pre Activate");

	private static readonly int _activateId = Animator.StringToHash("Activate");

	private void Start()
	{
		uint variable = PlayerData.instance.GetVariable<uint>(pdActivatedBitmask);
		if (variable == uint.MaxValue)
		{
			BellShrine[] array = bellShrines;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].TuningFork.SetInitialState(BellShrineTuningFork.States.AllComplete);
			}
		}
		else
		{
			for (int j = 0; j < bellShrines.Length; j++)
			{
				bellShrines[j].TuningFork.SetInitialState(variable.IsBitSet(j) ? BellShrineTuningFork.States.Activated : BellShrineTuningFork.States.Dormant);
			}
		}
		DoReset();
	}

	public void DoReset()
	{
		finalIcon.gameObject.SetActive(value: false);
	}

	public void DoActivation()
	{
		uint num = 0u;
		PlayerData instance = PlayerData.instance;
		uint num2 = instance.GetVariable<uint>(pdActivatedBitmask);
		int num3 = 0;
		for (int i = 0; i < bellShrines.Length; i++)
		{
			if (bellShrines[i].TuningFork.IsBellShrineCompleted)
			{
				num3++;
				if (!num2.IsBitSet(i))
				{
					num2 = num2.SetBitAtIndex(i);
					num = num.SetBitAtIndex(i);
				}
			}
		}
		if (num3 == bellShrines.Length)
		{
			num2 = uint.MaxValue;
			if (!string.IsNullOrWhiteSpace(awardAchievementOnComplete))
			{
				GameManager.instance.QueueAchievement(awardAchievementOnComplete);
			}
		}
		if (num != 0)
		{
			instance.SetVariable(pdActivatedBitmask, num2);
		}
		StartCoroutine(ActivationRoutine(num));
	}

	private IEnumerator ActivationRoutine(uint newMask)
	{
		PlayerData instance = PlayerData.instance;
		uint activatedMask = instance.GetVariable<uint>(pdActivatedBitmask);
		uint existingMask = activatedMask & ~newMask;
		yield return new WaitForSeconds(iconsBeginDelay);
		WaitForSeconds wait = new WaitForSeconds(iconsActivateDelay);
		for (int i = 0; i < bellShrines.Length; i++)
		{
			BellShrine bellShrine = bellShrines[i];
			bellShrine.Icon.gameObject.SetActive(value: true);
			if (existingMask.IsBitSet(i))
			{
				bellShrine.Icon.Play(_appearActiveId);
				iconActivateSound.SpawnAndPlayOneShot(Audio.Default2DAudioSourcePrefab, base.transform.position);
			}
			else
			{
				bellShrine.Icon.Play(_appearInactiveId);
				iconAppearSound.SpawnAndPlayOneShot(Audio.Default2DAudioSourcePrefab, base.transform.position);
			}
			if (existingMask.IsBitSet(i))
			{
				bellShrine.TuningFork.DoPulse();
			}
			yield return wait;
		}
		yield return new WaitForSeconds(tuningForksBeginDelay);
		wait = new WaitForSeconds(tuningForksActivateDelay);
		for (int i = 0; i < bellShrines.Length; i++)
		{
			if (newMask.IsBitSet(i))
			{
				BellShrine shrine = bellShrines[i];
				shrine.Icon.Play(_preActivateId);
				Vector3 pos = base.transform.position;
				iconPreActivateSound.SpawnAndPlayOneShot(Audio.Default2DAudioSourcePrefab, pos);
				yield return StartCoroutine(shrine.TuningFork.DoActivate());
				iconActivateSound.SpawnAndPlayOneShot(Audio.Default2DAudioSourcePrefab, pos);
				shrine.Icon.Play(_activateId);
				yield return null;
				yield return new WaitForSeconds(shrine.Icon.GetCurrentAnimatorStateInfo(0).length);
				yield return wait;
			}
		}
		bool isFinished = activatedMask == uint.MaxValue;
		if (isFinished)
		{
			finalIcon.gameObject.SetActive(value: true);
			finalIcon.Play(_activateId);
			yield return null;
			yield return new WaitForSeconds(finalIcon.GetCurrentAnimatorStateInfo(0).length);
		}
		eventFsm.SendEvent(isFinished ? "ACTIVATED ALL" : "ACTIVATION COMPLETE");
	}

	public void DoCompletion()
	{
		StartCoroutine(CompletionRoutine());
	}

	private IEnumerator CompletionRoutine()
	{
		float num = 0f;
		BellShrine[] array = bellShrines;
		foreach (BellShrine obj in array)
		{
			float randomValue = tuningForksCompleteDelay.GetRandomValue();
			obj.TuningFork.DoComplete(randomValue);
			if (randomValue > num)
			{
				num = randomValue;
			}
		}
		yield return new WaitForSeconds(num);
		eventFsm.SendEvent("COMPLETE ANIMS STARTED");
	}
}
