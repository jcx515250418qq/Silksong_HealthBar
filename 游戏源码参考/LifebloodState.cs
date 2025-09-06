using GlobalSettings;
using UnityEngine;

public class LifebloodState : MonoBehaviour
{
	public int healAmount = 5;

	private const float HEAL_TIME = 0.75f;

	private const float HP_MULTIPLIER = 1f;

	private bool dead;

	private bool healingIsActive = true;

	private int maxHP;

	private float timer;

	private HealthManager healthManager;

	private SpriteFlash spriteFlash;

	private GameObject healEffect;

	private TagDamageTaker tagDamageTaker;

	private PersistentBoolItem persistent;

	private void Awake()
	{
		persistent = GetComponent<PersistentBoolItem>() ?? base.gameObject.AddComponent<PersistentBoolItem>();
	}

	private void Start()
	{
		healthManager = base.gameObject.GetComponent<HealthManager>();
		spriteFlash = base.gameObject.GetComponent<SpriteFlash>();
		healthManager.TookDamage += TakeDamage;
		tagDamageTaker = GetComponent<TagDamageTaker>();
		Color lifebloodTintColour = Effects.LifebloodTintColour;
		healEffect = Object.Instantiate(Effects.LifebloodHealEffect, base.transform.position, base.transform.rotation, base.transform);
		healEffect.transform.position = new Vector3(healEffect.transform.position.x, healEffect.transform.position.y, 0.0011f);
		EnemyHitEffectsRegular component = base.gameObject.GetComponent<EnemyHitEffectsRegular>();
		if (component != null)
		{
			component.OverrideBloodColor = true;
			component.BloodColorOverride = lifebloodTintColour;
		}
		healthManager.hp = (int)((float)healthManager.hp * 1f);
		maxHP = healthManager.hp;
		healthManager.SetShellShards(0);
	}

	private void Update()
	{
		if (dead || !healingIsActive || ((bool)tagDamageTaker && tagDamageTaker.IsTagged))
		{
			return;
		}
		int hp = healthManager.hp;
		if (hp < maxHP)
		{
			if (timer < 0.75f)
			{
				timer += Time.deltaTime;
				return;
			}
			hp += healAmount;
			if (hp > maxHP)
			{
				hp = maxHP;
			}
			healthManager.hp = hp;
			healEffect.SetActive(value: true);
			spriteFlash.flashHealBlue();
			timer -= 0.75f;
		}
		else
		{
			timer = 0f;
		}
	}

	private void TakeDamage()
	{
		timer = 0f;
	}

	private void OnDeath()
	{
		LifebloodGlob lifebloodGlob = Effects.LifebloodGlob.Spawn(base.transform.position);
		dead = true;
		PersistentItemData<bool> itemData = persistent.ItemData;
		lifebloodGlob.SetTempQuestHandler(delegate
		{
			itemData.IsSemiPersistent = false;
			itemData.Value = true;
			SceneData.instance.PersistentBools.SetValue(itemData);
		});
	}

	public void SetIsLifebloodHealing(bool set)
	{
		timer = 0f;
		healingIsActive = set;
	}

	public void UpdateMaxHP(int new_maxHP)
	{
		maxHP = new_maxHP;
	}
}
