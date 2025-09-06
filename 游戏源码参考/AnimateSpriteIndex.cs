using UnityEngine;

[ExecuteInEditMode]
public class AnimateSpriteIndex : MonoBehaviour
{
	[SerializeField]
	private SpriteRenderer spriteRenderer;

	[SerializeField]
	private int index;

	[SerializeField]
	private Sprite[] sprites;

	private int previousIndex = -1;

	private void LateUpdate()
	{
		UpdateSprite();
	}

	private void UpdateSprite()
	{
		if (index != previousIndex)
		{
			index = Mathf.Clamp(index, 0, sprites.Length - 1);
			previousIndex = index;
			if (sprites.Length != 0 && (bool)spriteRenderer)
			{
				spriteRenderer.sprite = sprites[index];
			}
		}
	}
}
