using UnityEngine;

public struct UIMsgDisplay : ICollectableUIMsgItem, IUIMsgPopupItem
{
	public string Name;

	public Sprite Icon;

	public float IconScale;

	public Object RepresentingObject;

	public float GetUIMsgIconScale()
	{
		return IconScale;
	}

	public bool HasUpgradeIcon()
	{
		return false;
	}

	public string GetUIMsgName()
	{
		return Name;
	}

	public Sprite GetUIMsgSprite()
	{
		return Icon;
	}

	public Object GetRepresentingObject()
	{
		return RepresentingObject;
	}
}
