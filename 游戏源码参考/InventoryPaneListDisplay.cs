using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryPaneListDisplay : MonoBehaviour
{
	[SerializeField]
	private InventoryPaneListItem template;

	[SerializeField]
	private float itemSpacing = 2f;

	[SerializeField]
	private float arrowOffset = 1f;

	[SerializeField]
	private Transform leftArrow;

	[SerializeField]
	private Transform rightArrow;

	[Space]
	[SerializeField]
	private Transform leftArrowChild;

	[SerializeField]
	private Transform rightArrowChild;

	[SerializeField]
	private AnimationCurve arrowMoveXCurve;

	[SerializeField]
	private float arrowMoveXDuration = 0.2f;

	private List<InventoryPaneListItem> items;

	private Coroutine arrowAnimationRoutine;

	private Action onArrowAnimationEnd;

	public void PreInstantiate(int panesCount)
	{
		InstantiateNeededItems(panesCount);
	}

	public void UpdateDisplay(int currentPaneIndex, List<InventoryPane> panes, int cycleDirection)
	{
		int count = panes.Count;
		if (count <= 1)
		{
			base.gameObject.SetActive(value: false);
			return;
		}
		base.gameObject.SetActive(value: true);
		if (cycleDirection > 0)
		{
			DoAnimationRoutine(DoArrowAnimation(1, rightArrowChild), ref arrowAnimationRoutine, ref onArrowAnimationEnd);
		}
		else if (cycleDirection < 0)
		{
			DoAnimationRoutine(DoArrowAnimation(-1, leftArrowChild), ref arrowAnimationRoutine, ref onArrowAnimationEnd);
		}
		InstantiateNeededItems(count);
		for (int i = 0; i < items.Count; i++)
		{
			if (i < count)
			{
				items[i].gameObject.SetActive(value: true);
			}
			else
			{
				items[i].gameObject.SetActive(value: false);
			}
		}
		float num = itemSpacing * (float)(count - 1) / 2f;
		for (int j = 0; j < count; j++)
		{
			int num2 = j;
			bool isSelected = j == currentPaneIndex;
			InventoryPaneListItem inventoryPaneListItem = items[num2];
			inventoryPaneListItem.UpdateValues(panes[j], isSelected);
			inventoryPaneListItem.transform.SetLocalPositionX(Mathf.Lerp(0f - num, num, (float)num2 / (float)(count - 1)));
		}
		if ((bool)leftArrow)
		{
			leftArrow.transform.SetLocalPositionX(0f - num - arrowOffset);
		}
		if ((bool)rightArrow)
		{
			rightArrow.transform.SetLocalPositionX(num + arrowOffset);
		}
	}

	private IEnumerator DoArrowAnimation(int direction, Transform arrow)
	{
		if ((bool)arrow)
		{
			float startX = arrow.localPosition.x;
			onArrowAnimationEnd = delegate
			{
				arrow.SetLocalPositionX(startX);
			};
			for (float elapsed = 0f; elapsed < arrowMoveXDuration; elapsed += Time.unscaledDeltaTime)
			{
				arrow.SetLocalPositionX(startX + arrowMoveXCurve.Evaluate(elapsed / arrowMoveXDuration) * (float)direction);
				yield return null;
			}
			onArrowAnimationEnd();
			arrowAnimationRoutine = null;
		}
	}

	private void InstantiateNeededItems(int count)
	{
		if (items == null)
		{
			items = new List<InventoryPaneListItem>(count);
		}
		template.gameObject.SetActive(value: true);
		int num = count - items.Count;
		for (int i = 0; i < num; i++)
		{
			InventoryPaneListItem inventoryPaneListItem = UnityEngine.Object.Instantiate(template, template.transform.parent);
			items.Add(inventoryPaneListItem);
			inventoryPaneListItem.gameObject.name = $"{template.gameObject.name} ({items.Count})";
		}
		template.gameObject.SetActive(value: false);
	}

	private void DoAnimationRoutine(IEnumerator enumerator, ref Coroutine routine, ref Action endAction)
	{
		if (routine != null)
		{
			StopCoroutine(routine);
		}
		if (endAction != null)
		{
			endAction();
		}
		routine = StartCoroutine(enumerator);
	}
}
