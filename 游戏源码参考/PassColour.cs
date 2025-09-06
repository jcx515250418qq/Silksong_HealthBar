using System;
using UnityEngine;

public class PassColour : MonoBehaviour
{
	[Serializable]
	private class Other
	{
		public SpriteRenderer Sprite;

		public tk2dSprite tk2dSprite;

		public ParticleSystem ParticleSystem;

		public Color MultiplyColor;
	}

	[SerializeField]
	private Other[] passTo;

	public void SetColour(Color color)
	{
		Other[] array = passTo;
		foreach (Other other in array)
		{
			Color color2 = color.MultiplyElements(other.MultiplyColor);
			if ((bool)other.Sprite)
			{
				other.Sprite.color = color2;
			}
			if ((bool)other.tk2dSprite)
			{
				other.tk2dSprite.color = color2;
			}
			if ((bool)other.ParticleSystem)
			{
				ParticleSystem.MainModule main = other.ParticleSystem.main;
				main.startColor = color2;
			}
		}
	}
}
