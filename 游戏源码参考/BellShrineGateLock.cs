using System;
using System.Collections;
using TeamCherry.SharedUtils;
using UnityEngine;

public class BellShrineGateLock : MonoBehaviour
{
	[Serializable]
	private class BellLock
	{
		public GameObject Root;

		public JitterSelfForTime ActivateJitter;

		public JitterSelf LoopJitter;

		[PlayerDataField(typeof(bool), true)]
		public string PdBool;
	}

	[SerializeField]
	private BellLock[] bellLocks;

	[SerializeField]
	private float startDelay;

	[SerializeField]
	private float bellDelay;

	[SerializeField]
	private float endDelay;

	[SerializeField]
	private PlayMakerFSM eventFsm;

	private bool isPlaying;

	private Coroutine playRoutine;

	private void Start()
	{
		UpdateActivation();
	}

	private void UpdateActivation()
	{
		PlayerData instance = PlayerData.instance;
		BellLock[] array = bellLocks;
		foreach (BellLock bellLock in array)
		{
			bellLock.Root.SetActive(instance.GetVariable<bool>(bellLock.PdBool));
		}
	}

	public bool GetIsAllUnlocked()
	{
		PlayerData instance = PlayerData.instance;
		BellLock[] array = bellLocks;
		foreach (BellLock bellLock in array)
		{
			if (!instance.GetVariable<bool>(bellLock.PdBool))
			{
				return false;
			}
		}
		return true;
	}

	public void StartedPlaying()
	{
		if (playRoutine == null)
		{
			UpdateActivation();
			isPlaying = true;
			playRoutine = StartCoroutine(PlayRoutine());
		}
	}

	public void StoppedPlaying()
	{
		isPlaying = false;
	}

	private IEnumerator PlayRoutine()
	{
		yield return new WaitForSeconds(startDelay);
		if (!isPlaying)
		{
			playRoutine = null;
			yield break;
		}
		WaitForSeconds bellWait = new WaitForSeconds(bellDelay);
		PlayerData pd = PlayerData.instance;
		bool allDone = true;
		int playedCount = 0;
		for (int i = 0; i < bellLocks.Length; i++)
		{
			if (i > 0)
			{
				yield return bellWait;
			}
			if (!isPlaying)
			{
				break;
			}
			BellLock bellLock = bellLocks[i];
			if (!pd.GetVariable<bool>(bellLock.PdBool))
			{
				allDone = false;
				continue;
			}
			bellLock.ActivateJitter.StartTimedJitter();
			bellLock.LoopJitter.StartJitter();
			playedCount++;
		}
		if (playedCount >= bellLocks.Length)
		{
			EventRegister.SendEvent("NEEDOLIN LOCK");
		}
		if (!isPlaying || !allDone)
		{
			if (isPlaying)
			{
				yield return bellWait;
			}
			playRoutine = null;
			EndAll();
		}
		else
		{
			eventFsm.SendEvent("GATE LOCKS PRE COMPLETE");
			yield return new WaitForSeconds(endDelay);
			EndAll();
			eventFsm.SendEvent("GATE LOCKS COMPLETE");
		}
	}

	private void EndAll()
	{
		BellLock[] array = bellLocks;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].LoopJitter.StopJitterWithDecay();
		}
	}
}
