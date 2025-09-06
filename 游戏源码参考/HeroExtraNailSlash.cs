using UnityEngine;

public class HeroExtraNailSlash : MonoBehaviour
{
	[SerializeField]
	private SpriteRenderer[] tintSprites;

	[SerializeField]
	private tk2dSprite[] tintTk2dSprites;

	private DamageEnemies[] damagers;

	private void Awake()
	{
		damagers = GetComponentsInChildren<DamageEnemies>(includeInactive: true);
	}

	private void OnEnable()
	{
		HeroController instance = HeroController.instance;
		NailImbuementConfig currentImbuement = instance.NailImbuement.CurrentImbuement;
		if (currentImbuement == null)
		{
			return;
		}
		NailElements currentElement = instance.NailImbuement.CurrentElement;
		SpriteRenderer[] array = tintSprites;
		foreach (SpriteRenderer spriteRenderer in array)
		{
			if ((bool)spriteRenderer)
			{
				spriteRenderer.color = currentImbuement.NailTintColor;
			}
		}
		tk2dSprite[] array2 = tintTk2dSprites;
		foreach (tk2dSprite tk2dSprite2 in array2)
		{
			if ((bool)tk2dSprite2)
			{
				tk2dSprite2.color = currentImbuement.NailTintColor;
			}
		}
		DamageEnemies[] array3 = damagers;
		foreach (DamageEnemies damageEnemies in array3)
		{
			if ((bool)damageEnemies)
			{
				damageEnemies.NailElement = currentElement;
				damageEnemies.NailImbuement = currentImbuement;
			}
		}
	}

	private void OnDisable()
	{
		DamageEnemies[] array = damagers;
		foreach (DamageEnemies damageEnemies in array)
		{
			if ((bool)damageEnemies)
			{
				damageEnemies.NailElement = NailElements.None;
				damageEnemies.NailImbuement = null;
			}
		}
		SpriteRenderer[] array2 = tintSprites;
		foreach (SpriteRenderer spriteRenderer in array2)
		{
			if ((bool)spriteRenderer)
			{
				spriteRenderer.color = Color.white;
			}
		}
		tk2dSprite[] array3 = tintTk2dSprites;
		foreach (tk2dSprite tk2dSprite2 in array3)
		{
			if ((bool)tk2dSprite2)
			{
				tk2dSprite2.color = Color.white;
			}
		}
	}
}
