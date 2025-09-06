using UnityEngine;

public class PoisonTintSpriteRenderer : PoisonTintBase
{
	private SpriteRenderer sprite;

	protected override Color Colour
	{
		get
		{
			return sprite.color;
		}
		set
		{
			sprite.color = value;
		}
	}

	protected override void Awake()
	{
		sprite = GetComponent<SpriteRenderer>();
		base.Awake();
	}

	protected override void EnableKeyword(string keyword)
	{
		sprite.material.EnableKeyword(keyword);
	}

	protected override void DisableKeyword(string keyword)
	{
		sprite.material.DisableKeyword(keyword);
	}

	protected override void SetFloat(int propId, float value)
	{
		sprite.material.SetFloat(propId, value);
	}
}
