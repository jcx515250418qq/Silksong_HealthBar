using System.Collections;
using TeamCherry.NestedFadeGroup;
using UnityEngine;

public class ScrollView : MonoBehaviour
{
	[Header("View")]
	[SerializeField]
	private Bounds viewBounds;

	[SerializeField]
	private Bounds contentBounds;

	[Space]
	[SerializeField]
	private bool useChildColliders;

	[SerializeField]
	private Vector2 maxMargins;

	[SerializeField]
	private Vector2 minMargins;

	[Header("Scroll")]
	[SerializeField]
	private float scrollTopWidth;

	[SerializeField]
	private float scrollBottomWidth;

	[Space]
	[SerializeField]
	private float scrollTime = 0.2f;

	[SerializeField]
	private InvAnimateUpAndDown upArrow;

	[SerializeField]
	private NestedFadeGroupBase topGradient;

	[SerializeField]
	private InvAnimateUpAndDown downArrow;

	[SerializeField]
	private NestedFadeGroupBase bottomGradient;

	private Vector2 lastPosition;

	private Coroutine scrollRoutine;

	private InventoryPaneInput paneInput;

	private bool wasOffTop;

	private bool wasOffBottom;

	private float ScrollTime
	{
		get
		{
			if (!paneInput)
			{
				return scrollTime;
			}
			return paneInput.ListScrollSpeed;
		}
	}

	public Bounds ViewBounds => viewBounds;

	private void OnDrawGizmosSelected()
	{
		Transform obj = base.transform;
		Transform parent = obj.parent;
		if ((bool)parent)
		{
			Vector3 position = parent.position;
			Vector3 center = position + viewBounds.center;
			Gizmos.color = Color.green;
			Gizmos.DrawWireCube(center, viewBounds.size);
			float y = position.y + viewBounds.center.y + scrollTopWidth;
			Gizmos.color = Color.yellow;
			Gizmos.DrawLine(new Vector3(center.x - 1f, y), new Vector3(center.x + 1f, y));
			float y2 = position.y + viewBounds.center.y - scrollBottomWidth;
			Gizmos.color = Color.yellow;
			Gizmos.DrawLine(new Vector3(center.x - 1f, y2), new Vector3(center.x + 1f, y2));
		}
		Gizmos.color = Color.cyan;
		Gizmos.DrawWireCube(obj.position + contentBounds.center, contentBounds.size);
	}

	private void OnValidate()
	{
		FullUpdate();
	}

	private void OnEnable()
	{
		FullUpdate();
	}

	private void Start()
	{
		UpdateArrows(isInstant: true);
		InventoryItemManager componentInParent = GetComponentInParent<InventoryItemManager>();
		if ((bool)componentInParent)
		{
			paneInput = componentInParent.GetComponent<InventoryPaneInput>();
		}
	}

	private void LateUpdate()
	{
		if (!((Vector2)base.transform.localPosition == lastPosition))
		{
			ClampPosition();
			if (!((Vector2)base.transform.localPosition == lastPosition))
			{
				UpdateArrows();
				lastPosition = base.transform.localPosition;
			}
		}
	}

	private void OnTransformChildrenChanged()
	{
		FullUpdate();
	}

	public void FullUpdate()
	{
		CalculateContentBounds();
		ClampPosition();
		UpdateArrows(isInstant: true);
	}

	private void CalculateContentBounds()
	{
		if (!useChildColliders)
		{
			return;
		}
		Collider2D[] componentsInChildren = GetComponentsInChildren<Collider2D>();
		if (componentsInChildren.Length == 0)
		{
			contentBounds.SetMinMax(Vector3.zero, Vector3.zero);
			return;
		}
		float num = float.MaxValue;
		float num2 = float.MinValue;
		float num3 = float.MaxValue;
		float num4 = float.MinValue;
		Collider2D[] array = componentsInChildren;
		for (int i = 0; i < array.Length; i++)
		{
			Bounds bounds = array[i].bounds;
			if (bounds.max.x > num2)
			{
				num2 = bounds.max.x;
			}
			if (bounds.min.x < num)
			{
				num = bounds.min.x;
			}
			if (bounds.max.y > num4)
			{
				num4 = bounds.max.y;
			}
			if (bounds.min.y < num3)
			{
				num3 = bounds.min.y;
			}
		}
		Vector2 vector = new Vector2(num, num3) - minMargins;
		Vector2 vector2 = new Vector2(num2, num4) + maxMargins;
		Vector2 vector3 = base.transform.position;
		vector -= vector3;
		vector2 -= vector3;
		contentBounds.SetMinMax(vector, vector2);
	}

	private void ClampPosition()
	{
		Transform transform = base.transform;
		Transform parent = transform.parent;
		if ((bool)parent)
		{
			Bounds bounds = new Bounds(viewBounds.center + parent.position, viewBounds.size);
			Bounds bounds2 = new Bounds(contentBounds.center + transform.position, contentBounds.size);
			if (bounds2.size.y < bounds.size.y)
			{
				float num = bounds.size.y - bounds2.size.y;
				Vector2 vector = bounds2.min;
				vector.y -= num;
				bounds2.min = vector;
			}
			float b = bounds.max.y - bounds2.max.y;
			float b2 = bounds2.min.y - bounds.min.y;
			b = Mathf.Max(0f, b);
			b2 = Mathf.Max(0f, b2);
			float y = b2 - b;
			transform.position -= new Vector3(0f, y, 0f);
		}
	}

	public void ScrollTo(Vector2 localPosition, bool isInstant = false)
	{
		Transform transform = base.transform;
		Transform parent = transform.parent;
		if ((bool)parent)
		{
			Vector2 vector = transform.position;
			Vector2 vector2 = parent.position;
			Vector2 vector3 = vector2 + localPosition;
			Vector2 vector4 = vector2 + (Vector2)viewBounds.center;
			Vector2 vector5 = vector3 - vector4;
			Vector2 distance = vector2 - vector5 - vector;
			float num = ((distance.y > 0f) ? scrollBottomWidth : scrollTopWidth);
			float b = Mathf.Abs(distance.y) - num;
			distance.y = Mathf.Sign(distance.y) * Mathf.Max(0f, b);
			if (scrollRoutine != null)
			{
				StopCoroutine(scrollRoutine);
				scrollRoutine = null;
			}
			if (!isInstant && base.gameObject.activeInHierarchy)
			{
				scrollRoutine = StartCoroutine(ScrollDistance(distance, isInstant: false));
			}
			else
			{
				ScrollDistance(distance, isInstant: true).MoveNext();
			}
		}
	}

	private IEnumerator ScrollDistance(Vector2 distance, bool isInstant)
	{
		Vector3 startPosition = base.transform.localPosition;
		Vector3 targetPosition = startPosition;
		targetPosition.y += distance.y;
		WaitForEndOfFrame wait = new WaitForEndOfFrame();
		if (!isInstant)
		{
			for (float elapsed = 0f; elapsed < ScrollTime; elapsed += Time.unscaledDeltaTime)
			{
				UpdatePosition(elapsed / ScrollTime);
				yield return wait;
			}
		}
		UpdatePosition(1f);
		void UpdatePosition(float time)
		{
			base.transform.localPosition = Vector3.Lerp(startPosition, targetPosition, time);
			ClampPosition();
		}
	}

	private void UpdateArrows(bool isInstant = false)
	{
		Transform transform = base.transform;
		Bounds bounds = new Bounds(viewBounds.center + transform.parent.position, viewBounds.size);
		Bounds bounds2 = new Bounds(contentBounds.center + transform.position, contentBounds.size);
		bool flag = bounds2.max.y > bounds.max.y + 0.01f;
		bool flag2 = bounds2.min.y < bounds.min.y - 0.01f;
		UpdateArrow(upArrow, isInstant, flag);
		UpdateArrow(downArrow, isInstant, flag2);
		if ((bool)topGradient && (isInstant || flag != wasOffTop))
		{
			topGradient.FadeTo(flag ? 1 : 0, isInstant ? 0f : ScrollTime, null, isRealtime: true);
			wasOffTop = flag;
		}
		if ((bool)bottomGradient && (isInstant || flag2 != wasOffBottom))
		{
			bottomGradient.FadeTo(flag2 ? 1 : 0, isInstant ? 0f : ScrollTime, null, isRealtime: true);
			wasOffBottom = flag2;
		}
	}

	private void UpdateArrow(InvAnimateUpAndDown arrow, bool isInstant, bool offTop)
	{
		if (!arrow)
		{
			return;
		}
		if (offTop)
		{
			if (arrow.IsLastAnimatedDown)
			{
				if (isInstant)
				{
					arrow.Show();
				}
				else
				{
					arrow.AnimateUp();
				}
			}
		}
		else if (isInstant)
		{
			arrow.Hide();
		}
		else if (!arrow.IsLastAnimatedDown)
		{
			arrow.AnimateDown();
		}
	}
}
