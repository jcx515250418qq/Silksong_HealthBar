using UnityEngine;

public interface ITagDamageTakerOwner
{
	SpriteFlash SpriteFlash { get; }

	Vector2 TagDamageEffectPos { get; }

	Transform transform { get; }

	bool ApplyTagDamage(DamageTag.DamageTagInstance damageTagInstance);
}
