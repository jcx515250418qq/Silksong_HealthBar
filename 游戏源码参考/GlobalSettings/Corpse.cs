using UnityEngine;

namespace GlobalSettings
{
	[CreateAssetMenu(menuName = "Hornet/Global Settings/Global Corpse Settings")]
	public class Corpse : GlobalSettingsBase<Corpse>
	{
		[SerializeField]
		private Color landTint;

		[SerializeField]
		private float landTintFadeTime;

		[SerializeField]
		private float landTintWaitTime;

		[SerializeField]
		private AnimationCurve landTintCurve;

		[SerializeField]
		private AnimationCurve landDesaturationCurve;

		[Space]
		[SerializeField]
		private Color spellBurnColor;

		[SerializeField]
		private Color spellBurnColorBlackThread;

		[SerializeField]
		private ParticleEffectsLerpEmission spellBurnEffect;

		[SerializeField]
		private float spellBurnDuration;

		[SerializeField]
		private ParticleEffectsLerpEmission fireBurnEffect;

		[SerializeField]
		private ParticleEffectsLerpEmission poisonBurnEffect;

		[SerializeField]
		private ParticleEffectsLerpEmission soulBurnEffect;

		[SerializeField]
		private ParticleEffectsLerpEmission zapBurnEffect;

		[Header("Death")]
		[SerializeField]
		private GameObject enemyLavaDeath;

		[SerializeField]
		private float minCorpseFlingMagnitudeMult;

		public static Color LandTint => Get().landTint;

		public static float LandTintFadeTime => Get().landTintFadeTime;

		public static float LandTintWaitTime => Get().landTintWaitTime;

		public static AnimationCurve LandTintCurve => Get().landTintCurve;

		public static AnimationCurve LandDesaturationCurve => Get().landDesaturationCurve;

		public static Color SpellBurnColor => Get().spellBurnColor;

		public static Color SpellBurnColorBlackThread => Get().spellBurnColorBlackThread;

		public static ParticleEffectsLerpEmission SpellBurnEffect => Get().spellBurnEffect;

		public static float SpellBurnDuration => Get().spellBurnDuration;

		public static ParticleEffectsLerpEmission FireBurnEffect => Get().fireBurnEffect;

		public static ParticleEffectsLerpEmission PoisonBurnEffect => Get().poisonBurnEffect;

		public static ParticleEffectsLerpEmission SoulBurnEffect => Get().soulBurnEffect;

		public static ParticleEffectsLerpEmission ZapBurnEffect => Get().zapBurnEffect;

		public static GameObject EnemyLavaDeath => Get().enemyLavaDeath;

		public static float MinCorpseFlingMagnitudeMult => Get().minCorpseFlingMagnitudeMult;

		[RuntimeInitializeOnLoadMethod]
		public static void PreWarm()
		{
			GlobalSettingsBase<Corpse>.StartPreloadAddressable("Global Corpse Settings");
		}

		public static void Unload()
		{
			GlobalSettingsBase<Corpse>.StartUnload();
		}

		private static Corpse Get()
		{
			return GlobalSettingsBase<Corpse>.Get("Global Corpse Settings");
		}
	}
}
