using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class TrapBridgeExtend : TrapBridge
{
	[Serializable]
	private struct LockedLeverGroup
	{
		public GameObject Lever;

		public Animator LockedLever;

		public TrackTriggerObjects UnlockDetector;
	}

	[Header("Extender")]
	[SerializeField]
	private Animator animator;

	[SerializeField]
	private PersistentBoolItem persistent;

	[SerializeField]
	private LockedLeverGroup[] lockedLevers;

	[Space]
	public UnityEvent OnExtend;

	public UnityEvent OnRetract;

	private bool activated;

	private static readonly int _closeAnim = Animator.StringToHash("Close");

	private static readonly int _openAnim = Animator.StringToHash("Open");

	private static readonly int _unlockAnim = Animator.StringToHash("Unlock");

	private void Awake()
	{
		SetLeversUnlocked(unlock: false);
		if ((bool)persistent)
		{
			persistent.OnGetSaveState += delegate(out bool value)
			{
				value = activated;
			};
			persistent.OnSetSaveState += delegate(bool value)
			{
				bool num = activated;
				activated = value;
				if (!num && activated)
				{
					SetLeversUnlocked(unlock: true);
				}
			};
		}
		onOpened.AddListener(UnlockLevers);
	}

	protected override IEnumerator DoOpenAnim()
	{
		OnExtend.Invoke();
		animator.Play(_openAnim);
		yield return null;
		yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
	}

	protected override IEnumerator DoCloseAnim()
	{
		OnRetract.Invoke();
		animator.Play(_closeAnim);
		yield return null;
		yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
	}

	private void UnlockLevers()
	{
		if (!activated)
		{
			activated = true;
			LockedLeverGroup[] array = lockedLevers;
			foreach (LockedLeverGroup lever in array)
			{
				StartCoroutine(UnlockLever(lever));
			}
		}
	}

	private void SetLeversUnlocked(bool unlock)
	{
		LockedLeverGroup[] array = lockedLevers;
		for (int i = 0; i < array.Length; i++)
		{
			LockedLeverGroup lockedLeverGroup = array[i];
			if ((bool)lockedLeverGroup.Lever)
			{
				lockedLeverGroup.Lever.SetActive(unlock);
			}
			if ((bool)lockedLeverGroup.LockedLever)
			{
				lockedLeverGroup.LockedLever.gameObject.SetActive(!unlock);
			}
		}
	}

	private IEnumerator UnlockLever(LockedLeverGroup lever)
	{
		if ((bool)lever.UnlockDetector)
		{
			while (!lever.UnlockDetector.IsInside)
			{
				yield return null;
			}
		}
		if ((bool)lever.LockedLever)
		{
			lever.LockedLever.Play(_unlockAnim);
			yield return null;
			yield return new WaitForSeconds(lever.LockedLever.GetCurrentAnimatorStateInfo(0).length);
			lever.LockedLever.gameObject.SetActive(value: false);
		}
		if ((bool)lever.Lever)
		{
			lever.Lever.SetActive(value: true);
		}
	}
}
