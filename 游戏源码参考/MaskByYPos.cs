using System;
using TeamCherry.NestedFadeGroup;
using UnityEngine;

public class MaskByYPos : MonoBehaviour
{
	[SerializeField]
	private SpriteRenderer spriteRenderer;

	[SerializeField]
	private NestedFadeGroupBase fadeGroup;

	[SerializeField]
	[HideInInspector]
	[Obsolete]
	private float yPos;

	[SerializeField]
	private bool maskIfAboveY;

	[SerializeField]
	[ModifiableProperty]
	[Conditional("maskIfAboveY", true, false, false)]
	private float aboveYPos;

	[SerializeField]
	private bool maskIfBelowY;

	[SerializeField]
	[ModifiableProperty]
	[Conditional("maskIfBelowY", true, false, false)]
	private float belowYPos;

	private bool wasActive;

	private bool hasSpriteRenderer;

	private bool hasFadeGroup;

	private void OnDrawGizmosSelected()
	{
		if (maskIfAboveY)
		{
			Vector3 position = base.transform.position;
			float? y = aboveYPos;
			Gizmos.DrawWireSphere(position.Where(null, y, null), 0.3f);
		}
		if (maskIfBelowY)
		{
			Vector3 position2 = base.transform.position;
			float? y = belowYPos;
			Gizmos.DrawWireSphere(position2.Where(null, y, null), 0.3f);
		}
	}

	private void OnValidate()
	{
		if (!Mathf.Approximately(yPos, 0f))
		{
			aboveYPos = yPos;
			belowYPos = yPos;
			yPos = 0f;
		}
	}

	private void Awake()
	{
		OnValidate();
	}

	private void Reset()
	{
		fadeGroup = GetComponent<NestedFadeGroupBase>();
		if (!fadeGroup)
		{
			spriteRenderer = GetComponent<SpriteRenderer>();
		}
	}

	private void OnEnable()
	{
		if (spriteRenderer == null && !fadeGroup)
		{
			spriteRenderer = GetComponent<SpriteRenderer>();
		}
		hasSpriteRenderer = spriteRenderer;
		hasFadeGroup = fadeGroup;
		DoCheck(force: true);
	}

	private void LateUpdate()
	{
		DoCheck(force: false);
	}

	private void DoCheck(bool force)
	{
		float y = base.transform.position.y;
		bool flag = (!maskIfAboveY || !(y > aboveYPos)) && ((!maskIfBelowY || !(y < belowYPos)) ? true : false);
		if (!flag)
		{
			if (wasActive || force)
			{
				if (hasSpriteRenderer)
				{
					spriteRenderer.enabled = false;
				}
				if (hasFadeGroup)
				{
					fadeGroup.AlphaSelf = 0f;
				}
			}
		}
		else if (!wasActive || force)
		{
			if (hasSpriteRenderer)
			{
				spriteRenderer.enabled = true;
			}
			if (hasFadeGroup)
			{
				fadeGroup.AlphaSelf = 1f;
			}
		}
		wasActive = flag;
	}
}
