using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class PersistentActivator : MonoBehaviour
{
	[SerializeField]
	private PersistentBoolItem persistent;

	[Space]
	[SerializeField]
	private float unlockDelay;

	[SerializeField]
	private UnlockablePropBase[] unlockables;

	[Space]
	public UnityEvent OnActivate;

	public UnityEvent OnActivated;

	private bool isActivated;

	private void Awake()
	{
		persistent.OnGetSaveState += delegate(out bool value)
		{
			value = isActivated;
		};
		persistent.OnSetSaveState += delegate(bool value)
		{
			isActivated = value;
			if (isActivated)
			{
				Activated();
			}
		};
	}

	public void Activate()
	{
		if (!isActivated)
		{
			isActivated = true;
			OnActivate.Invoke();
			StartCoroutine(UnlockDelayed());
		}
	}

	private void Activated()
	{
		OnActivated.Invoke();
		UnlockablePropBase[] array = unlockables;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Opened();
		}
	}

	private IEnumerator UnlockDelayed()
	{
		yield return new WaitForSeconds(unlockDelay);
		UnlockablePropBase[] array = unlockables;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Open();
		}
	}
}
