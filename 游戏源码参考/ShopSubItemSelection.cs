using System.Collections;
using GlobalSettings;
using TeamCherry.NestedFadeGroup;
using UnityEngine;

public class ShopSubItemSelection : MonoBehaviour
{
	[SerializeField]
	private Transform itemsParent;

	[SerializeField]
	private PlayMakerFSM itemListFsm;

	[SerializeField]
	private NestedFadeGroupBase fadeParent;

	[SerializeField]
	private Transform selectionCursor;

	[SerializeField]
	private Animator selectorAnimator;

	[SerializeField]
	private AudioEvent moveSelectionAudio;

	[SerializeField]
	private AudioEvent confirmAudio;

	[SerializeField]
	private AudioEvent cancelAudio;

	private ShopItemStats item;

	private int currentIndex;

	private float endTimer;

	private Vector2 cursorTargetPos;

	private Coroutine cursorMoveRoutine;

	private InputHandler ih;

	private Platform platform;

	private static readonly int _appearAnimId = Animator.StringToHash("Appear");

	private static readonly int _disappearAnimId = Animator.StringToHash("Disappear");

	private void OnEnable()
	{
		ih = ManagerSingleton<InputHandler>.Instance;
		platform = Platform.Current;
		item = null;
		endTimer = 0f;
	}

	private void Update()
	{
		if (endTimer > 0f)
		{
			endTimer -= Time.deltaTime;
			if (endTimer <= 0f)
			{
				base.gameObject.SetActive(value: false);
				EventRegister.SendEvent(EventRegisterEvents.ResetShopWindow);
			}
		}
		else
		{
			if (!item)
			{
				return;
			}
			switch (platform.GetMenuAction(ih.inputActions))
			{
			case Platform.MenuActions.Submit:
				selectorAnimator.Play(_disappearAnimId);
				itemListFsm.FsmVariables.FindFsmInt("Current Item Sub").Value = currentIndex;
				itemListFsm.SendEvent("TO CONFIRM");
				confirmAudio.SpawnAndPlayOneShot(Audio.DefaultUIAudioSourcePrefab, cursorTargetPos);
				return;
			case Platform.MenuActions.Cancel:
				selectorAnimator.Play(_disappearAnimId);
				endTimer = 0.1f;
				fadeParent.FadeToZero(endTimer);
				cancelAudio.SpawnAndPlayOneShot(Audio.DefaultUIAudioSourcePrefab, cursorTargetPos);
				return;
			}
			int num;
			if (ih.inputActions.Left.WasPressed || ih.inputActions.PaneLeft.WasPressed)
			{
				num = -1;
			}
			else
			{
				if (!ih.inputActions.Right.WasPressed && !ih.inputActions.PaneRight.WasPressed)
				{
					return;
				}
				num = 1;
			}
			int subItemsCount = item.Item.SubItemsCount;
			currentIndex += num;
			if (currentIndex >= subItemsCount)
			{
				currentIndex = 0;
			}
			else if (currentIndex < 0)
			{
				currentIndex = subItemsCount - 1;
			}
			SetSelected(currentIndex, isInstant: false);
		}
	}

	public void SetItem(GameObject itemObj, int initialSelection)
	{
		item = itemObj.GetComponent<ShopItemStats>();
		currentIndex = initialSelection;
		SetSelected(currentIndex, isInstant: true);
		selectorAnimator.Play(_appearAnimId);
	}

	private void SetSelected(int index, bool isInstant)
	{
		if (cursorMoveRoutine != null)
		{
			StopCoroutine(cursorMoveRoutine);
			cursorMoveRoutine = null;
			selectionCursor.SetPosition2D(cursorTargetPos);
		}
		int num = 0;
		ShopSubItemStats shopSubItemStats = null;
		foreach (Transform item in itemsParent)
		{
			if (!item.gameObject.activeSelf)
			{
				continue;
			}
			ShopSubItemStats component = item.GetComponent<ShopSubItemStats>();
			if ((bool)component)
			{
				if (num == index)
				{
					shopSubItemStats = component;
					break;
				}
				num++;
			}
		}
		if ((bool)shopSubItemStats)
		{
			cursorTargetPos = shopSubItemStats.transform.position;
			if (isInstant)
			{
				selectionCursor.SetPosition2D(cursorTargetPos);
			}
			else
			{
				cursorMoveRoutine = StartCoroutine(MoveCursorTo(cursorTargetPos));
			}
		}
	}

	private IEnumerator MoveCursorTo(Vector2 targetPos)
	{
		moveSelectionAudio.SpawnAndPlayOneShot(Audio.DefaultUIAudioSourcePrefab, targetPos);
		selectorAnimator.Play(_disappearAnimId);
		yield return null;
		yield return new WaitForSeconds(selectorAnimator.GetCurrentAnimatorStateInfo(0).length);
		selectionCursor.SetPosition2D(targetPos);
		selectorAnimator.Play(_appearAnimId);
	}
}
