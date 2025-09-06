using System;
using System.Collections;
using UnityEngine;

[DefaultExecutionOrder(1)]
public class BossDoorTargetLock : MonoBehaviour
{
	[Serializable]
	public class BossDoorTarget
	{
		public BossSequenceDoor door;

		public GameObject indicator;

		public bool Evaluate()
		{
			if ((bool)door && (bool)indicator)
			{
				bool completed = door.CurrentCompletion.completed;
				indicator.SetActive(completed);
				return completed;
			}
			return false;
		}
	}

	public BossDoorTarget[] targets;

	public string playerData = "finalBossDoorUnlocked";

	public TriggerEnterEvent unlockTrigger;

	public string unlockAnimation = "Unlock";

	public string unlockedAnimation = "Unlocked";

	private Animator animator;

	private bool IsUnlocked
	{
		get
		{
			if (string.IsNullOrEmpty(playerData))
			{
				return false;
			}
			return GameManager.instance.GetPlayerDataBool(playerData);
		}
		set
		{
			if (!string.IsNullOrEmpty(playerData))
			{
				GameManager.instance.SetPlayerDataBool(playerData, value);
			}
			else
			{
				Debug.LogError("Can't save an empty PlayerData bool!", this);
			}
		}
	}

	private void Awake()
	{
		animator = GetComponent<Animator>();
	}

	private void Start()
	{
		bool flag = true;
		BossDoorTarget[] array = targets;
		for (int i = 0; i < array.Length; i++)
		{
			if (!array[i].Evaluate())
			{
				flag = false;
			}
		}
		if (IsUnlocked)
		{
			if ((bool)animator)
			{
				animator.Play(unlockedAnimation);
			}
		}
		else if (flag && (bool)unlockTrigger)
		{
			TriggerEnterEvent.CollisionEvent temp = null;
			temp = delegate
			{
				StartCoroutine(UnlockSequence());
				unlockTrigger.OnTriggerEntered -= temp;
			};
			unlockTrigger.OnTriggerEntered += temp;
		}
	}

	private IEnumerator UnlockSequence()
	{
		if ((bool)animator)
		{
			animator.Play(unlockAnimation);
			yield return null;
			yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
		}
		IsUnlocked = true;
	}

	private void StartRoarLock()
	{
		HeroController.instance.StartRoarLock();
	}

	private void StopRoarLock()
	{
		HeroController.instance.StopRoarLock();
	}
}
