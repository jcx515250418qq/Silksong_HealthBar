using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
[RequireComponent(typeof(Image))]
public class UIImageKeepNativeSize : MonoBehaviour
{
	private Image image;

	private Sprite sprite;

	private void Awake()
	{
		image = GetComponent<Image>();
	}

	private void OnRectTransformDimensionsChange()
	{
		if ((bool)image)
		{
			image.SetNativeSize();
		}
	}

	private void LateUpdate()
	{
		if (image.sprite != sprite)
		{
			sprite = image.sprite;
			image.SetNativeSize();
		}
	}
}
