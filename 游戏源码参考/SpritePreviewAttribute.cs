using UnityEngine;

public class SpritePreviewAttribute : PropertyAttribute
{
	public float previewHeight;

	public SpritePreviewAttribute(float previewHeight = 64f)
	{
		this.previewHeight = previewHeight;
	}
}
