using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public sealed class VerticalScrollRectController : MonoBehaviour
{
	[SerializeField]
	private ScrollRect scrollRect;

	[SerializeField]
	private List<ScrollIndicator> topIndicators = new List<ScrollIndicator>();

	[SerializeField]
	private List<ScrollIndicator> bottomIndicators = new List<ScrollIndicator>();

	[Space]
	[SerializeField]
	private Vector2 size;

	[SerializeField]
	private Vector2 extent;

	[SerializeField]
	private Vector2 contentSize;

	[SerializeField]
	private Vector2 scrollMin = Vector2.zero;

	[SerializeField]
	private Vector2 scrollMax;

	[Space]
	[SerializeField]
	private Vector2 scroll;

	[SerializeField]
	private float scrollAmount = 260f;

	[SerializeField]
	private bool centreFocus;

	private RectTransform contentRT;

	private RectTransformDimensionChangedEvent contentChangedEvent;

	private bool topShown;

	private bool bottomShown;

	private bool dirty = true;

	public void ScrollUp()
	{
		DoScroll(new Vector2(0f, 0f - scrollAmount));
	}

	public void ScrollDown()
	{
		DoScroll(new Vector2(0f, scrollAmount));
	}

	private void Awake()
	{
		if ((bool)scrollRect)
		{
			scrollRect.onValueChanged.AddListener(OnScrollValueChanged);
			contentRT = scrollRect.content;
			if ((bool)contentRT)
			{
				contentChangedEvent = contentRT.gameObject.AddComponentIfNotPresent<RectTransformDimensionChangedEvent>();
				contentChangedEvent.DimensionsChanged += OnDimensionsChanged;
			}
		}
	}

	private void OnValidate()
	{
		if (!Application.isPlaying)
		{
			UpdateScrollValues();
		}
	}

	private void Update()
	{
		UpdateIfDirty();
	}

	private void OnDimensionsChanged()
	{
		dirty = true;
	}

	private void OnScrollValueChanged(Vector2 newScroll)
	{
		UpdateIfDirty();
		newScroll = contentRT.localPosition;
		newScroll = ClampScroll(newScroll);
		scroll = newScroll.ClampVector2(scrollMin, scrollMax);
		contentRT.localPosition = scroll;
	}

	private void UpdateIfDirty()
	{
		if (dirty)
		{
			dirty = false;
			UpdateScrollValues();
		}
	}

	private void UpdateScrollValues()
	{
		RectTransform rectTransform = base.transform as RectTransform;
		if ((bool)rectTransform)
		{
			Rect rect = rectTransform.rect;
			size = new Vector2(rect.width, rect.height);
			extent = size * 0.5f;
		}
		if ((bool)scrollRect)
		{
			contentRT = scrollRect.content;
		}
		if ((bool)contentRT)
		{
			Rect rect2 = contentRT.rect;
			contentSize = new Vector2(rect2.width, rect2.height);
			scroll = contentRT.localPosition;
		}
		scrollMax = contentSize - size;
		if (scrollMax.x < 0f)
		{
			scrollMax.x = 0f;
		}
		if (scrollMax.y < 0f)
		{
			scrollMax.y = 0f;
		}
	}

	private Vector2 ClampScroll(Vector2 value)
	{
		if (value.y <= scrollMin.y + 1f)
		{
			value.y = scrollMin.y;
			ToggleTopIndicators(show: false);
		}
		else if (contentSize.y > size.y && value.y > 0f)
		{
			ToggleTopIndicators(show: true);
		}
		if (value.y >= scrollMax.y - 1f)
		{
			value.y = scrollMax.y;
			ToggleBottomIndicators(show: false);
		}
		else if (contentSize.y > value.y && value.y < contentSize.y - extent.y)
		{
			ToggleBottomIndicators(show: true);
		}
		return value;
	}

	private void ToggleTopIndicators(bool show)
	{
		if (topShown == show)
		{
			return;
		}
		topShown = show;
		if (show)
		{
			foreach (ScrollIndicator topIndicator in topIndicators)
			{
				topIndicator.Show();
			}
			return;
		}
		foreach (ScrollIndicator topIndicator2 in topIndicators)
		{
			topIndicator2.Hide();
		}
	}

	private void ToggleBottomIndicators(bool show)
	{
		if (bottomShown == show)
		{
			return;
		}
		bottomShown = show;
		if (show)
		{
			foreach (ScrollIndicator bottomIndicator in bottomIndicators)
			{
				bottomIndicator.Show();
			}
			return;
		}
		foreach (ScrollIndicator bottomIndicator2 in bottomIndicators)
		{
			bottomIndicator2.Hide();
		}
	}

	private void DoScroll(Vector2 amount)
	{
		float y = contentRT.localPosition.y + amount.y;
		contentRT.localPosition = ClampScroll(new Vector2(0f, y));
	}

	public void SetScrollTarget(Transform targetTransform, bool isMouse = false)
	{
		UpdateIfDirty();
		if (!isMouse)
		{
			float num = Mathf.Abs(targetTransform.localPosition.y);
			if (centreFocus)
			{
				num -= extent.y;
			}
			contentRT.localPosition = ClampScroll(new Vector2(0f, num));
		}
	}

	public void ResetScroll()
	{
		if ((bool)contentRT)
		{
			contentRT.localPosition = ClampScroll(Vector2.zero);
		}
	}
}
