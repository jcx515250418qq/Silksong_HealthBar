using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class ScrollBarHandle : MonoBehaviour
{
	public Scrollbar scrollBar;

	private RectTransform trans;

	private void Awake()
	{
		trans = GetComponent<RectTransform>();
		if (!scrollBar)
		{
			scrollBar = GetComponentInParent<Scrollbar>();
		}
	}

	private void Start()
	{
		if ((bool)scrollBar)
		{
			scrollBar.onValueChanged.AddListener(UpdatePosition);
		}
	}

	private void UpdatePosition(float value)
	{
		trans.pivot = new Vector2(0.5f, value);
		trans.anchorMin = new Vector2(0.5f, value);
		trans.anchorMax = new Vector2(0.5f, value);
		trans.anchoredPosition.Set(trans.anchoredPosition.x, 0f);
	}
}
