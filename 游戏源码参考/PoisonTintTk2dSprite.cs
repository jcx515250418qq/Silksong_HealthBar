using UnityEngine;

public class PoisonTintTk2dSprite : PoisonTintBase
{
	private tk2dSprite sprite;

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
		sprite = GetComponent<tk2dSprite>();
		base.Awake();
	}

	protected override void EnableKeyword(string keyword)
	{
		if ((bool)sprite)
		{
			sprite.EnableKeyword(keyword);
		}
	}

	protected override void DisableKeyword(string keyword)
	{
		if ((bool)sprite)
		{
			sprite.DisableKeyword(keyword);
		}
	}

	protected override void SetFloat(int propId, float value)
	{
		if ((bool)sprite)
		{
			sprite.SetFloat(propId, value);
		}
	}
}
