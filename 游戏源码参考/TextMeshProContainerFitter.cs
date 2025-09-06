using TMProOld;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
[RequireComponent(typeof(TextContainer), typeof(TMP_Text))]
public class TextMeshProContainerFitter : MonoBehaviour, ILayoutController
{
	[SerializeField]
	private bool preferredWidth;

	[SerializeField]
	private bool preferredHeight;

	[SerializeField]
	private LayoutElement layoutElement;

	private TMP_Text text;

	private TextContainer container;

	private string previousText;

	private bool previousPreferredWidth;

	private bool previousPreferredHeight;

	private void Awake()
	{
		GetComponents();
	}

	private void Start()
	{
		UpdateValues(doWidth: true, doHeight: true);
	}

	private void Update()
	{
		if (!text.text.Equals(previousText) || preferredWidth != previousPreferredWidth || preferredHeight != previousPreferredHeight)
		{
			UpdateValues(doWidth: true, doHeight: true);
		}
	}

	private void GetComponents()
	{
		if (!text)
		{
			text = GetComponent<TMP_Text>();
		}
		if (!container)
		{
			container = GetComponent<TextContainer>();
		}
	}

	private void UpdateValues(bool doWidth, bool doHeight)
	{
		GetComponents();
		previousText = text.text;
		previousPreferredHeight = preferredHeight;
		previousPreferredWidth = preferredWidth;
		Vector2 size = container.size;
		Vector2 preferredValues = text.GetPreferredValues(previousText, (doWidth && preferredWidth) ? float.PositiveInfinity : size.x, (doHeight && preferredHeight) ? float.PositiveInfinity : size.y);
		if (preferredWidth)
		{
			size.x = preferredValues.x;
		}
		if (preferredHeight)
		{
			size.y = preferredValues.y;
		}
		if ((bool)layoutElement)
		{
			if (preferredWidth)
			{
				layoutElement.minWidth = size.x;
			}
			if (preferredHeight)
			{
				layoutElement.minHeight = size.y;
			}
		}
		else
		{
			container.size = size;
		}
	}

	public void SetLayoutHorizontal()
	{
		UpdateValues(doWidth: true, doHeight: false);
	}

	public void SetLayoutVertical()
	{
		UpdateValues(doWidth: false, doHeight: true);
	}

	public void SetLayoutAll()
	{
		UpdateValues(doWidth: true, doHeight: true);
	}
}
