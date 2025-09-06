using UnityEngine;

public interface ICollectableUIMsgItem : IUIMsgPopupItem
{
	Sprite GetUIMsgSprite();

	string GetUIMsgName();

	float GetUIMsgIconScale();

	bool HasUpgradeIcon();
}
