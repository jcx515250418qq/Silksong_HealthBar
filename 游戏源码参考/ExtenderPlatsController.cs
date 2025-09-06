using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class ExtenderPlatsController : UnlockablePropBase
{
	[Serializable]
	public class UnityBoolEvent : UnityEvent<bool>
	{
	}

	[SerializeField]
	private PersistentBoolItem persistent;

	[SerializeField]
	private ExtenderPlatLink[] links;

	[SerializeField]
	private float linkUnfoldStartDelay;

	[SerializeField]
	private float linkUnfoldDelay;

	[SerializeField]
	private float linkUnfoldDuration;

	[SerializeField]
	private float platUnfoldStartDelay;

	[SerializeField]
	private float platUnfoldDelay;

	[SerializeField]
	private float platUnfoldDuration;

	[SerializeField]
	private AudioSource audioStart;

	[Space]
	public UnityBoolEvent OnSetActive;

	[Space]
	[SerializeField]
	private UnlockablePropBase[] passUnlock;

	private bool isUnfolded;

	private void Awake()
	{
		if (!persistent)
		{
			return;
		}
		persistent.OnGetSaveState += delegate(out bool value)
		{
			value = isUnfolded;
		};
		persistent.OnSetSaveState += delegate(bool value)
		{
			isUnfolded = value;
			if (isUnfolded)
			{
				SetUnfoldedInstant();
			}
		};
	}

	private void Start()
	{
		if (!isUnfolded)
		{
			ResetChain();
		}
	}

	[ContextMenu("Reset")]
	private void ResetChain()
	{
		ExtenderPlatLink[] array = links;
		foreach (ExtenderPlatLink obj in array)
		{
			obj.UpdateLinkRotation(0f);
			obj.UpdatePlatRotation(0f);
			obj.SetActive(value: false, isInstant: true);
		}
		isUnfolded = false;
		OnSetActive.Invoke(arg0: false);
	}

	[ContextMenu("Reset", true)]
	[ContextMenu("Unfold", true)]
	private bool CanUnfold()
	{
		return Application.isPlaying;
	}

	[ContextMenu("Unfold")]
	public void Unfold()
	{
		if (base.isActiveAndEnabled && !isUnfolded)
		{
			isUnfolded = true;
			OnSetActive.Invoke(arg0: true);
			StartCoroutine(UnfoldChain());
		}
	}

	public void SetUnfoldedInstant()
	{
		if (isUnfolded)
		{
			return;
		}
		isUnfolded = true;
		OnSetActive.Invoke(arg0: true);
		ExtenderPlatLink[] array = links;
		foreach (ExtenderPlatLink obj in array)
		{
			obj.UpdateLinkRotation(1f);
			obj.UpdatePlatRotation(1f);
			obj.SetActive(value: true, isInstant: true);
		}
		UnlockablePropBase[] array2 = passUnlock;
		foreach (UnlockablePropBase unlockablePropBase in array2)
		{
			if ((bool)unlockablePropBase)
			{
				unlockablePropBase.Opened();
			}
		}
	}

	private IEnumerator UnfoldChain()
	{
		yield return new WaitForSeconds(linkUnfoldStartDelay);
		WaitForSeconds linkUnfoldWait = new WaitForSeconds(linkUnfoldDelay);
		int lastLink = links.Length - 1;
		for (int i = 0; i < links.Length; i++)
		{
			ExtenderPlatLink extenderPlatLink = links[i];
			if (extenderPlatLink.gameObject.activeSelf)
			{
				extenderPlatLink.LinkRotationStarted();
				StartCoroutine(LinkTimer(linkUnfoldDuration, extenderPlatLink.UpdateLinkRotation, null));
				if (i != lastLink)
				{
					yield return linkUnfoldWait;
				}
			}
		}
		yield return new WaitForSeconds(platUnfoldStartDelay);
		WaitForSeconds platUnfoldWait = new WaitForSeconds(platUnfoldDelay);
		for (int i = 0; i < links.Length; i++)
		{
			ExtenderPlatLink link = links[i];
			if (link.IsPlatformActive)
			{
				link.PlatRotationStarted();
				StartCoroutine(LinkTimer(platUnfoldDuration, link.UpdatePlatRotation, delegate
				{
					link.SetActive(value: true, isInstant: false);
				}));
			}
			if (i != lastLink)
			{
				yield return platUnfoldWait;
			}
		}
		UnlockablePropBase[] array = passUnlock;
		foreach (UnlockablePropBase unlockablePropBase in array)
		{
			if ((bool)unlockablePropBase)
			{
				unlockablePropBase.Open();
			}
		}
	}

	private IEnumerator LinkTimer(float duration, Action<float> handler, Action onEnd)
	{
		for (float elapsed = 0f; elapsed < duration; elapsed += Time.deltaTime)
		{
			float obj = elapsed / duration;
			handler(obj);
			yield return null;
		}
		handler(1f);
		onEnd?.Invoke();
	}

	public override void Open()
	{
		audioStart.Play();
		Unfold();
	}

	public override void Opened()
	{
		SetUnfoldedInstant();
	}
}
