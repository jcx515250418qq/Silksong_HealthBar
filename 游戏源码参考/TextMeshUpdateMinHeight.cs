using TMProOld;
using UnityEngine;
using UnityEngine.UI;

public sealed class TextMeshUpdateMinHeight : MonoBehaviour
{
	[SerializeField]
	private TextMeshPro textMeshPro;

	[SerializeField]
	private LayoutElement layoutElement;

	private bool started;

	private void Start()
	{
		started = true;
		UpdateMinValue();
	}

	private void OnEnable()
	{
		if (started)
		{
			UpdateMinValue();
		}
	}

	private void OnValidate()
	{
		if (textMeshPro == null)
		{
			textMeshPro = GetComponent<TextMeshPro>();
		}
		if (layoutElement == null)
		{
			layoutElement = GetComponent<LayoutElement>();
		}
	}

	private void UpdateMinValue()
	{
		if ((bool)textMeshPro && (bool)layoutElement)
		{
			Vector2 preferredValues = textMeshPro.GetPreferredValues();
			layoutElement.minHeight = preferredValues.y;
			Debug.Log($"{this} Setting height to {layoutElement.minHeight} for {textMeshPro.text}", this);
		}
	}
}
