using System.Collections.Generic;
using GlobalSettings;
using UnityEngine;

public class HeroShamanRuneEffect : MonoBehaviour
{
	[SerializeField]
	private DamageEnemies damager;

	[SerializeField]
	private GameObject rune;

	[Space]
	[SerializeField]
	private GameObject runeSpawnEffect;

	[SerializeField]
	private Vector3 spawnOffset;

	[SerializeField]
	private Vector3 spawnScale = Vector3.one;

	[SerializeField]
	private float spawnDelay;

	[SerializeField]
	private float spawnMult = 1f;

	[Space]
	[SerializeField]
	private List<SpriteRenderer> zapTintSprites;

	[SerializeField]
	private List<ParticleSystem> zapTintParticles;

	[SerializeField]
	private GameObject[] disableIfZap;

	private Dictionary<SpriteRenderer, Color> initialSpriteColours;

	private Dictionary<ParticleSystem, ParticleSystem.MinMaxGradient> initialParticleColours;

	private bool hasStarted;

	private float initialDamageMult;

	private AttackTypes initialAttackType;

	private void Awake()
	{
		if ((bool)damager)
		{
			initialDamageMult = damager.DamageMultiplier;
			initialAttackType = damager.attackType;
		}
		zapTintSprites.RemoveNulls();
		zapTintParticles.RemoveNulls();
		if (zapTintSprites.Count > 0)
		{
			initialSpriteColours = new Dictionary<SpriteRenderer, Color>(zapTintSprites.Count);
			foreach (SpriteRenderer zapTintSprite in zapTintSprites)
			{
				initialSpriteColours[zapTintSprite] = zapTintSprite.color;
			}
		}
		if (zapTintParticles.Count <= 0)
		{
			return;
		}
		initialParticleColours = new Dictionary<ParticleSystem, ParticleSystem.MinMaxGradient>(zapTintParticles.Count);
		foreach (ParticleSystem zapTintParticle in zapTintParticles)
		{
			initialParticleColours[zapTintParticle] = zapTintParticle.main.startColor;
		}
	}

	private void OnEnable()
	{
		if (hasStarted)
		{
			Refresh();
		}
	}

	private void Start()
	{
		hasStarted = true;
		Refresh();
	}

	public void Refresh()
	{
		bool isEquipped = Gameplay.SpellCrest.IsEquipped;
		if ((bool)damager)
		{
			damager.DamageMultiplier = (isEquipped ? (initialDamageMult * Gameplay.SpellCrestRuneDamageMult) : initialDamageMult);
			damager.attackType = (isEquipped ? AttackTypes.Spell : initialAttackType);
		}
		if ((bool)rune)
		{
			rune.SetActive(isEquipped);
		}
		bool flag = isEquipped && Gameplay.ZapImbuementTool.IsEquipped;
		Color color = (flag ? Gameplay.ZapDamageTintColour : Color.white);
		disableIfZap.SetAllActive(!flag);
		if (initialSpriteColours != null)
		{
			foreach (KeyValuePair<SpriteRenderer, Color> initialSpriteColour in initialSpriteColours)
			{
				initialSpriteColour.Key.color = initialSpriteColour.Value * color;
			}
		}
		if (initialParticleColours != null)
		{
			foreach (KeyValuePair<ParticleSystem, ParticleSystem.MinMaxGradient> initialParticleColour in initialParticleColours)
			{
				ParticleSystem.MainModule main = initialParticleColour.Key.main;
				ParticleSystem.MinMaxGradient value = initialParticleColour.Value;
				switch (value.mode)
				{
				case ParticleSystemGradientMode.Color:
					value.color *= color;
					break;
				case ParticleSystemGradientMode.TwoColors:
					value.colorMin *= color;
					value.colorMax *= color;
					break;
				}
				main.startColor = value;
			}
		}
		if (isEquipped && (bool)runeSpawnEffect)
		{
			Transform transform = (rune ? rune.transform : base.transform);
			GameObject obj = runeSpawnEffect.Spawn(transform.TransformPoint(spawnOffset));
			obj.transform.localScale = transform.TransformVector(spawnScale);
			FollowTransform component = obj.GetComponent<FollowTransform>();
			if ((bool)component)
			{
				component.Target = transform;
			}
			FollowRotation component2 = obj.GetComponent<FollowRotation>();
			if ((bool)component2)
			{
				component2.Target = transform;
			}
			ParticleSystem component3 = runeSpawnEffect.GetComponent<ParticleSystem>();
			ParticleSystem component4 = obj.GetComponent<ParticleSystem>();
			ParticleSystem.MainModule main2 = component4.main;
			main2.startDelay = spawnDelay;
			ParticleSystem.EmissionModule emission = component4.emission;
			emission.rateOverTimeMultiplier = spawnMult * component3.emission.rateOverTimeMultiplier;
			component4.Stop(withChildren: true, ParticleSystemStopBehavior.StopEmittingAndClear);
			component4.Play();
		}
	}
}
