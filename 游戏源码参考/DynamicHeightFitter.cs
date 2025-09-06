using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
[RequireComponent(typeof(RectTransform))]
public sealed class DynamicHeightFitter : MonoBehaviour, ILayoutElement, IInitialisable
{
	[SerializeField]
	private float spacing = 1f;

	[SerializeField]
	private RectTransform parentRect;

	[SerializeField]
	private float minElementHeight = 100f;

	[SerializeField]
	private float maxElementHeight = 300f;

	private RectTransform rectTransform;

	private float calculatedHeight;

	private bool hasAwaken;

	private bool hasStarted;

	public float minHeight => minElementHeight;

	public float preferredHeight => calculatedHeight;

	public float flexibleHeight => 0f;

	public float minWidth => -1f;

	public float preferredWidth => -1f;

	public float flexibleWidth => -1f;

	public int layoutPriority => 1;

	GameObject IInitialisable.gameObject => base.gameObject;

	public bool OnAwake()
	{
		if (hasAwaken)
		{
			return false;
		}
		hasAwaken = true;
		this.rectTransform = GetComponent<RectTransform>();
		if (parentRect == null && base.transform.parent is RectTransform rectTransform)
		{
			parentRect = rectTransform;
		}
		calculatedHeight = maxElementHeight;
		return true;
	}

	public bool OnStart()
	{
		OnAwake();
		if (hasStarted)
		{
			return false;
		}
		hasStarted = true;
		return true;
	}

	private void Awake()
	{
		OnAwake();
	}

	private void OnEnable()
	{
		UpdateHeight();
	}

	private void UpdateHeight()
	{
		OnAwake();
		if (parentRect == null)
		{
			Debug.LogWarning("Parent RectTransform not assigned.");
			return;
		}
		VerticalLayoutGroup component = parentRect.GetComponent<VerticalLayoutGroup>();
		float num = 0f;
		float num2 = 0f;
		if (component != null)
		{
			num = component.spacing;
			num2 = component.padding.top + component.padding.bottom;
		}
		float num3 = parentRect.rect.height - num2;
		float num4 = 0f;
		int num5 = 0;
		foreach (RectTransform item in parentRect)
		{
			if (!(item == rectTransform))
			{
				num5++;
				ILayoutElement component2 = item.GetComponent<ILayoutElement>();
				num4 = ((component2 == null) ? (num4 + item.rect.height) : (num4 + Mathf.Max(component2.minHeight, component2.preferredHeight)));
			}
		}
		num4 += num * (float)Mathf.Max(0, num5 - 1);
		float num6 = num3 - num4;
		if (num5 > 0)
		{
			num6 -= spacing;
		}
		calculatedHeight = Mathf.Clamp(num6, minElementHeight, maxElementHeight);
		rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, calculatedHeight);
	}

	public void CalculateLayoutInputVertical()
	{
		UpdateHeight();
	}

	public void CalculateLayoutInputHorizontal()
	{
	}

	private void OnValidate()
	{
		if (minElementHeight > maxElementHeight)
		{
			Debug.LogWarning("MinHeight cannot be greater than MaxHeight. Adjusting values.");
			minElementHeight = maxElementHeight;
		}
		if (!Application.isPlaying)
		{
			UpdateHeight();
		}
	}
}
