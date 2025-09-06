using System.Collections;
using TeamCherry.NestedFadeGroup;
using UnityEngine;

public class InventoryCursor : MonoBehaviour
{
	public interface ICursorTarget
	{
		bool ShowCursor { get; }

		Color? CursorColor { get; }
	}

	[SerializeField]
	private Transform topLeft;

	[SerializeField]
	private Transform topRight;

	[SerializeField]
	private Transform bottomLeft;

	[SerializeField]
	private Transform bottomRight;

	[SerializeField]
	private Transform back;

	[SerializeField]
	private SpriteRenderer backGlow;

	private Color defaultGlowColor;

	[SerializeField]
	public float moveTime = 0.15f;

	[SerializeField]
	private NestedFadeGroupBase group;

	[Space]
	[SerializeField]
	private AudioSource audioSourcePrefab;

	[SerializeField]
	private AudioEvent changeSelectionSound;

	private Coroutine moveRoutine;

	private Transform currentTarget;

	private InventoryPaneInput paneInput;

	private bool skipNextLerp;

	private bool queueActivate;

	private bool isPosClamped;

	private Vector2 minPos;

	private Vector2 maxPos;

	private float MoveTime
	{
		get
		{
			if (!paneInput)
			{
				return moveTime;
			}
			return Mathf.Min(moveTime, paneInput.ListScrollSpeed);
		}
	}

	private void Awake()
	{
		if ((bool)backGlow)
		{
			defaultGlowColor = backGlow.color;
		}
		InventoryItemManager componentInParent = GetComponentInParent<InventoryItemManager>(includeInactive: true);
		if ((bool)componentInParent)
		{
			paneInput = componentInParent.GetComponent<InventoryPaneInput>();
		}
		if ((bool)paneInput)
		{
			paneInput.OnActivated += delegate
			{
				if ((bool)currentTarget)
				{
					skipNextLerp = false;
					queueActivate = true;
					SetTarget(currentTarget);
				}
			};
			paneInput.OnDeactivated += Deactivate;
		}
		InventoryPane componentInParent2 = GetComponentInParent<InventoryPane>(includeInactive: true);
		if (!componentInParent2)
		{
			return;
		}
		Deactivate();
		if ((bool)paneInput)
		{
			componentInParent2.OnPaneStart += Deactivate;
		}
		else
		{
			componentInParent2.OnPaneStart += delegate
			{
				queueActivate = true;
			};
		}
		componentInParent2.OnPaneEnd += Deactivate;
		queueActivate = true;
	}

	public void ActivateIfNotActive()
	{
		if (!base.gameObject.activeSelf)
		{
			Activate(setPreviousTarget: true);
		}
	}

	public void Activate()
	{
		Activate(setPreviousTarget: false);
	}

	public void Deactivate()
	{
		base.gameObject.SetActive(value: false);
	}

	public void Activate(bool setPreviousTarget)
	{
		ResetAppearance();
		skipNextLerp = true;
		if (setPreviousTarget)
		{
			SetTarget(currentTarget);
		}
	}

	private void ResetAppearance()
	{
		if ((bool)back)
		{
			back.SetLocalPosition2D(0f, 0f);
			back.localScale = Vector3.one;
			back.gameObject.SetActive(value: true);
		}
		if ((bool)bottomLeft)
		{
			bottomLeft.SetLocalPosition2D(-0.5f, -0.5f);
			bottomLeft.gameObject.SetActive(value: true);
		}
		if ((bool)bottomRight)
		{
			bottomRight.SetLocalPosition2D(0.5f, -0.5f);
			bottomRight.gameObject.SetActive(value: true);
		}
		if ((bool)topLeft)
		{
			topLeft.SetLocalPosition2D(-0.5f, 0.5f);
			topLeft.gameObject.SetActive(value: true);
		}
		if ((bool)topRight)
		{
			topRight.SetLocalPosition2D(0.5f, 0.5f);
			topRight.gameObject.SetActive(value: true);
		}
		if ((bool)backGlow)
		{
			backGlow.color = defaultGlowColor;
		}
	}

	private void LateUpdate()
	{
		if ((bool)currentTarget)
		{
			base.transform.SetPosition2D(GetClampedTargetPos(currentTarget.position));
		}
	}

	private Vector2 GetClampedTargetPos(Vector2 pos)
	{
		if (!isPosClamped)
		{
			return pos;
		}
		if (pos.x < minPos.x)
		{
			pos.x = minPos.x;
		}
		if (pos.y < minPos.y)
		{
			pos.y = minPos.y;
		}
		if (pos.x > maxPos.x)
		{
			pos.x = maxPos.x;
		}
		if (pos.y > maxPos.y)
		{
			pos.y = maxPos.y;
		}
		return pos;
	}

	public void SetClampedPos(Vector2 min, Vector2 max)
	{
		isPosClamped = true;
		minPos = min;
		maxPos = max;
	}

	public void ResetClampedPos()
	{
		isPosClamped = false;
	}

	public void SetTarget(Transform target)
	{
		Transform transform = currentTarget;
		currentTarget = target;
		if (!target || ((bool)paneInput && !paneInput.isActiveAndEnabled))
		{
			return;
		}
		if (currentTarget != transform)
		{
			changeSelectionSound.SpawnAndPlayOneShot(audioSourcePrefab, base.transform.position);
		}
		Vector2 glowScale = Vector2.one;
		if (skipNextLerp)
		{
			BoxCollider2D component = target.GetComponent<BoxCollider2D>();
			Vector2 pos = (component ? target.TransformPoint(component.offset) : currentTarget.position);
			base.transform.SetPosition2D(GetClampedTargetPos(pos));
			ICursorTarget component2 = target.GetComponent<ICursorTarget>();
			if (component2 != null && !component2.ShowCursor && (bool)group)
			{
				group.AlphaSelf = 0f;
			}
			base.gameObject.SetActive(value: true);
			if ((bool)backGlow)
			{
				Color color = defaultGlowColor;
				InventoryItemSelectable component3 = target.GetComponent<InventoryItemSelectable>();
				if ((bool)component3)
				{
					color = component3.CursorColor ?? color;
					glowScale = component3.CursorGlowScale;
				}
				backGlow.color = color;
			}
			skipNextLerp = false;
		}
		if (moveRoutine != null)
		{
			StopCoroutine(moveRoutine);
			moveRoutine = null;
		}
		if (base.gameObject.activeInHierarchy)
		{
			moveRoutine = StartCoroutine(MoveTo(target, glowScale, doAnim: true));
		}
		else
		{
			MoveTo(target, glowScale, doAnim: false);
			if (queueActivate)
			{
				Activate();
			}
		}
		queueActivate = false;
	}

	private IEnumerator MoveTo(Transform target, Vector2 glowScale, bool doAnim)
	{
		Vector2 topRightInitialPos = (topRight ? ((Vector2)topRight.position) : Vector2.zero);
		Vector2 topLeftInitialPos = (topLeft ? ((Vector2)topLeft.position) : Vector2.zero);
		Vector2 bottomRightInitialPos = (bottomRight ? ((Vector2)bottomRight.position) : Vector2.zero);
		Vector2 bottomLeftInitialPos = (bottomLeft ? ((Vector2)bottomLeft.position) : Vector2.zero);
		Vector2 backInitialPos = (back ? ((Vector2)back.position) : Vector2.zero);
		base.transform.SetPosition2D(GetClampedTargetPos(target.position));
		if ((bool)topRight)
		{
			topRight.SetPosition2D(topRightInitialPos);
			topRightInitialPos = topRight.localPosition;
		}
		if ((bool)topLeft)
		{
			topLeft.SetPosition2D(topLeftInitialPos);
			topLeftInitialPos = topLeft.localPosition;
		}
		if ((bool)bottomRight)
		{
			bottomRight.SetPosition2D(bottomRightInitialPos);
			bottomRightInitialPos = bottomRight.localPosition;
		}
		if ((bool)bottomLeft)
		{
			bottomLeft.SetPosition2D(bottomLeftInitialPos);
			bottomLeftInitialPos = bottomLeft.localPosition;
		}
		if ((bool)back)
		{
			back.SetPosition2D(backInitialPos);
			backInitialPos = back.localPosition;
		}
		Vector3 backInitialScale = (back ? back.localScale : Vector3.one);
		BoxCollider2D component = target.GetComponent<BoxCollider2D>();
		Vector2 boxOffset = (component ? component.offset : Vector2.zero);
		Vector2 boxScale;
		if ((bool)component)
		{
			boxScale = target.TransformVector(component.size);
			if ((bool)base.transform.parent)
			{
				boxScale = boxScale.MultiplyElements(Vector2.one.DivideElements((Vector2)base.transform.parent.lossyScale));
			}
		}
		else
		{
			boxScale = Vector2.one;
		}
		Vector2 cornerOffset = boxScale / 2f;
		Color startColor = (backGlow ? backGlow.color : Color.white);
		Color newColor = defaultGlowColor;
		bool flag = true;
		ICursorTarget component2 = target.GetComponent<ICursorTarget>();
		if (component2 != null)
		{
			newColor = component2.CursorColor ?? newColor;
			flag = component2.ShowCursor;
		}
		boxScale = boxScale.MultiplyElements(glowScale);
		float startGroupAlpha = (group ? group.AlphaSelf : 1f);
		float targetGroupAlpha = (flag ? 1 : 0);
		if (doAnim)
		{
			for (float elapsed = 0f; elapsed < MoveTime; elapsed += Time.unscaledDeltaTime)
			{
				float time2 = elapsed / MoveTime;
				SetPositions(time2);
				yield return null;
			}
		}
		SetPositions(1f);
		moveRoutine = null;
		void SetPositions(float time)
		{
			Vector2 vector = cornerOffset;
			if ((bool)topRight)
			{
				topRight.SetLocalPosition2D(Vector2.Lerp(topRightInitialPos, boxOffset + vector, time));
			}
			vector.x *= -1f;
			if ((bool)topLeft)
			{
				topLeft.SetLocalPosition2D(Vector2.Lerp(topLeftInitialPos, boxOffset + vector, time));
			}
			vector.y *= -1f;
			if ((bool)bottomLeft)
			{
				bottomLeft.SetLocalPosition2D(Vector2.Lerp(bottomLeftInitialPos, boxOffset + vector, time));
			}
			vector.x *= -1f;
			if ((bool)bottomRight)
			{
				bottomRight.SetLocalPosition2D(Vector2.Lerp(bottomRightInitialPos, boxOffset + vector, time));
			}
			if ((bool)back)
			{
				back.SetLocalPosition2D(Vector2.Lerp(backInitialPos, boxOffset, time));
				back.localScale = Vector3.Lerp(backInitialScale, boxScale, time);
			}
			if ((bool)backGlow)
			{
				backGlow.color = Color.Lerp(startColor, newColor, time);
			}
			if ((bool)group)
			{
				group.AlphaSelf = Mathf.Lerp(startGroupAlpha, targetGroupAlpha, time);
			}
		}
	}
}
