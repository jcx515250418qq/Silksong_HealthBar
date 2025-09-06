using UnityEngine;

public class SpawnedCorpseEffectTint : MonoBehaviour
{
	private tk2dSprite[] sprites;

	private Color[] spriteColors;

	private void Awake()
	{
		sprites = GetComponentsInChildren<tk2dSprite>();
		spriteColors = new Color[sprites.Length];
		for (int i = 0; i < sprites.Length; i++)
		{
			spriteColors[i] = sprites[i].color;
		}
	}

	private void OnDisable()
	{
		for (int i = 0; i < sprites.Length; i++)
		{
			sprites[i].color = spriteColors[i];
		}
	}

	public void SetTint(Color color)
	{
		for (int i = 0; i < sprites.Length; i++)
		{
			sprites[i].color = spriteColors[i] * color;
		}
	}
}
