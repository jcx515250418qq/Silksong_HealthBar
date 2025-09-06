using System.Collections;
using GlobalSettings;
using TeamCherry.NestedFadeGroup;
using UnityEngine;
using UnityEngine.Rendering;

public abstract class UIMsgPopupBase<TPopupDisplay, TPopupObject> : UIMsgPopupBaseBase where TPopupDisplay : IUIMsgPopupItem where TPopupObject : UIMsgPopupBase<TPopupDisplay, TPopupObject>
{
	[SerializeField]
	private SortingGroup sortingGroup;

	[SerializeField]
	private NestedFadeGroupBase fadeGroup;

	[SerializeField]
	private float fadeInTime = 0.3f;

	[SerializeField]
	private float waitTime = 4.25f;

	[SerializeField]
	private float fadeOutTime = 0.2f;

	[Space]
	[SerializeField]
	private GameObject replaceBurstEffect;

	private int initialSortingOrder;

	private bool canRemainAlive;

	private Coroutine displayRoutine;

	private static TPopupObject _lastActiveMsg;

	public TPopupDisplay DisplayingItem { get; protected set; }

	private void Awake()
	{
		if ((bool)sortingGroup)
		{
			initialSortingOrder = sortingGroup.sortingOrder;
		}
	}

	private void OnEnable()
	{
		canRemainAlive = true;
	}

	protected static TPopupObject SpawnInternal(TPopupObject prefab, TPopupDisplay item, TPopupObject replacing = null, bool forceReplacingEffect = false)
	{
		if ((bool)UIMsgPopupBaseBase.LastActiveMsgShared && UIMsgPopupBaseBase.LastActiveMsgShared.position.y > 8.5f)
		{
			return null;
		}
		TPopupObject val;
		if ((bool)_lastActiveMsg && !replacing)
		{
			Object representingObject = _lastActiveMsg.DisplayingItem.GetRepresentingObject();
			Object representingObject2 = item.GetRepresentingObject();
			val = ((!(representingObject2 != null) || !(representingObject2 == representingObject)) ? prefab.Spawn() : _lastActiveMsg);
		}
		else
		{
			val = prefab.Spawn();
		}
		if ((bool)val.replaceBurstEffect)
		{
			val.replaceBurstEffect.SetActive(value: false);
		}
		if (replacing != null)
		{
			if (replacing == _lastActiveMsg)
			{
				_lastActiveMsg = val;
			}
			if (replacing.transform == UIMsgPopupBaseBase.LastActiveMsgShared)
			{
				UIMsgPopupBaseBase.LastActiveMsgShared = val.transform;
			}
			val.transform.localPosition = replacing.transform.localPosition;
			replacing.End();
			val.DoReplacingEffects();
			if ((bool)val.sortingGroup)
			{
				val.sortingGroup.sortingOrder = replacing.sortingGroup.sortingOrder + 1;
			}
		}
		else
		{
			UIMsgPopupBaseBase.UpdatePosition(val.transform);
			_lastActiveMsg = val;
			if (forceReplacingEffect)
			{
				val.DoReplacingEffects();
			}
			if ((bool)val.sortingGroup)
			{
				val.sortingGroup.sortingOrder = val.initialSortingOrder;
			}
		}
		val.Display(item);
		return val;
	}

	public void DoReplacingEffects()
	{
		if ((bool)replaceBurstEffect)
		{
			replaceBurstEffect.SetActive(value: true);
		}
		UI.ItemQuestMaxPopupSound.SpawnAndPlayOneShot(Audio.DefaultUIAudioSourcePrefab, Vector3.zero);
	}

	protected void Display(TPopupDisplay item)
	{
		DisplayingItem = item;
		UpdateDisplay(item);
		if (displayRoutine != null)
		{
			StopCoroutine(displayRoutine);
			displayRoutine = StartCoroutine(DisplaySequence(fadeIn: false));
		}
		else
		{
			displayRoutine = StartCoroutine(DisplaySequence(fadeIn: true));
		}
	}

	public void End()
	{
		canRemainAlive = false;
	}

	private bool IsInterrupted()
	{
		return !canRemainAlive;
	}

	private IEnumerator DisplaySequence(bool fadeIn)
	{
		if ((bool)fadeGroup)
		{
			if (fadeIn)
			{
				fadeGroup.AlphaSelf = 0f;
				yield return new WaitForSecondsInterruptable(fadeGroup.FadeTo(1f, fadeInTime), IsInterrupted, isRealtime: true);
			}
			else
			{
				fadeGroup.FadeTo(1f, 0f);
			}
			fadeGroup.AlphaSelf = 1f;
		}
		yield return new WaitForSecondsInterruptable(waitTime, IsInterrupted, isRealtime: true);
		if ((bool)fadeGroup)
		{
			yield return new WaitForSecondsRealtime(fadeGroup.FadeTo(0f, fadeOutTime, null, isRealtime: true));
		}
		if (_lastActiveMsg == this)
		{
			_lastActiveMsg = null;
		}
		if (UIMsgPopupBaseBase.LastActiveMsgShared == base.transform)
		{
			UIMsgPopupBaseBase.LastActiveMsgShared = null;
			if (InteractManager.BlockingInteractable == null)
			{
				GameManager.instance.DoQueuedSaveGame();
			}
		}
		displayRoutine = null;
		base.gameObject.Recycle();
	}

	protected abstract void UpdateDisplay(TPopupDisplay item);
}
