using UnityEngine;
using UnityEngine.Audio;

namespace GlobalSettings
{
	[CreateAssetMenu(menuName = "Hornet/Global Settings/Global Effects Settings")]
	public class Effects : GlobalSettingsBase<Effects>
	{
		[Header("Hit Effects")]
		[SerializeField]
		private GameObject bloodParticlePrefab;

		[SerializeField]
		private GameObject rageHitEffectPrefab;

		[SerializeField]
		private GameObject rageHitHealthEffectPrefab;

		[SerializeField]
		private AudioEvent enemyCoalHurtSound;

		[SerializeField]
		private GameObject weakHitEffectPrefab;

		[SerializeField]
		private CameraShakeTarget weakHitEffectShake;

		[Space]
		[SerializeField]
		private GameObject enemyWitchPoisonHitEffectPrefab;

		[SerializeField]
		private GameObject enemyWitchPoisonHurtEffectPrefab;

		[SerializeField]
		private BloodSpawner.GeneralConfig enemyWitchPoisonBloodBurst;

		[Space]
		[SerializeField]
		private EnemyHitEffectsProfile lightningHitEffects;

		[Space]
		[SerializeField]
		private GameObject reapHitEffectPrefab;

		[Space]
		[SerializeField]
		private GameObject spikeSlashEffectPrefab;

		[Space]
		[SerializeField]
		private NailImbuementConfig fireNail;

		[Header("Nail Clash Tink")]
		[SerializeField]
		private CameraShakeTarget nailClashTinkShake;

		[SerializeField]
		private AudioEvent nailClashParrySound;

		[SerializeField]
		private GameObject nailClashParryEffect;

		[SerializeField]
		private GameObject nailClashParryEffectSmall;

		[Space]
		[SerializeField]
		private GameObject enemyNailTerrainThunk;

		[Space]
		[SerializeField]
		private GameObject tinkEffectDullPrefab;

		[Header("Camera Shakes")]
		[SerializeField]
		private CameraShakeTarget blockedHitShake;

		[SerializeField]
		private CameraShakeTarget blockedHitShakeNoFreeze;

		[Header("Enemies")]
		[SerializeField]
		private GameObject silkPossesionObjSing;

		[SerializeField]
		private GameObject silkPossesionObjSingNoPuppet;

		[SerializeField]
		private GameObject silkPossesionObjSingEnd;

		[Space]
		[SerializeField]
		private Color lifebloodTintColour;

		[SerializeField]
		private GameObject lifebloodEffectPrefab;

		[SerializeField]
		private GameObject lifebloodHealEffect;

		[SerializeField]
		private LifebloodGlob lifebloodGlob;

		[SerializeField]
		private Quest lifeBloodQuest;

		[Space]
		[SerializeField]
		private GameObject enemyPhysicalPusher;

		[SerializeField]
		private RandomAudioClipTable enemyDamageTickSoundTable;

		[Header("Black Thread")]
		[SerializeField]
		private GameObject blackThreadEnemyStartEffect;

		[SerializeField]
		private GameObject blackThreadEnemyEffect;

		[SerializeField]
		private GameObject blackThreadEnemyDeathEffect;

		[SerializeField]
		private AnimationCurve blackThreadEnemyAttackTintCurve;

		[SerializeField]
		private float blackThreadEnemyAttackTintDuration;

		[SerializeField]
		private AnimationCurve blackThreadEnemyPulseTintCurve;

		[SerializeField]
		private float blackThreadEnemyPulseTintDuration;

		[SerializeField]
		private GameObject blackThreadPooledEffect;

		[SerializeField]
		private AudioEvent blackThreadHitSound;

		[SerializeField]
		private GameObject hitFlashBlack;

		[SerializeField]
		private GameObject hitShade;

		[SerializeField]
		private GameObject hitShadeMinimal;

		[SerializeField]
		private BlackThreadAttack[] blackThreadAttacksDefault;

		[SerializeField]
		private AudioMixerGroup blackThreadVoiceMixerGroup;

		[Header("Masking")]
		[SerializeField]
		private Material cutoutSpriteMaterial;

		[Header("Environment")]
		[SerializeField]
		private Color mossEffectsTintDust;

		[Space]
		[SerializeField]
		private Material defaultLitMaterial;

		[SerializeField]
		private Material defaultUnlitMaterial;

		[Header("Glow Response")]
		[SerializeField]
		private AnimationCurve glowResponsePulseCurve;

		[SerializeField]
		private float glowResponsePulseDuration;

		[SerializeField]
		private float glowResponsePulseFrameRate;

		[Header("Maggots")]
		[SerializeField]
		private AudioEvent beginMaggotedSound;

		public static GameObject BloodParticlePrefab => Get().bloodParticlePrefab;

		public static GameObject RageHitEffectPrefab => Get().rageHitEffectPrefab;

		public static GameObject RageHitHealthEffectPrefab => Get().rageHitHealthEffectPrefab;

		public static AudioEvent EnemyCoalHurtSound => Get().enemyCoalHurtSound;

		public static GameObject WeakHitEffectPrefab => Get().weakHitEffectPrefab;

		public static CameraShakeTarget WeakHitEffectShake => Get().weakHitEffectShake;

		public static GameObject EnemyWitchPoisonHitEffectPrefab => Get().enemyWitchPoisonHitEffectPrefab;

		public static GameObject EnemyWitchPoisonHurtEffectPrefab => Get().enemyWitchPoisonHurtEffectPrefab;

		public static BloodSpawner.GeneralConfig EnemyWitchPoisonBloodBurst => Get().enemyWitchPoisonBloodBurst;

		public static EnemyHitEffectsProfile LightningHitEffects => Get().lightningHitEffects;

		public static GameObject ReapHitEffectPrefab => Get().reapHitEffectPrefab;

		public static GameObject SpikeSlashEffectPrefab => Get().spikeSlashEffectPrefab;

		public static NailImbuementConfig FireNail => Get().fireNail;

		public static CameraShakeTarget NailClashTinkShake => Get().nailClashTinkShake;

		public static AudioEvent NailClashParrySound => Get().nailClashParrySound;

		public static GameObject NailClashParryEffect => Get().nailClashParryEffect;

		public static GameObject NailClashParryEffectSmall => Get().nailClashParryEffectSmall;

		public static GameObject EnemyNailTerrainThunk => Get().enemyNailTerrainThunk;

		public static GameObject TinkEffectDullPrefab => Get().tinkEffectDullPrefab;

		public static CameraShakeTarget BlockedHitShake => Get().blockedHitShake;

		public static CameraShakeTarget BlockedHitShakeNoFreeze => Get().blockedHitShakeNoFreeze;

		public static GameObject SilkPossesionObjSing => Get().silkPossesionObjSing;

		public static GameObject SilkPossesionObjSingNoPuppet => Get().silkPossesionObjSingNoPuppet;

		public static GameObject SilkPossesionObjSingEnd => Get().silkPossesionObjSingEnd;

		public static Color LifebloodTintColour => Get().lifebloodTintColour;

		public static GameObject LifebloodEffectPrefab => Get().lifebloodEffectPrefab;

		public static GameObject LifebloodHealEffect => Get().lifebloodHealEffect;

		public static LifebloodGlob LifebloodGlob => Get().lifebloodGlob;

		public static Quest LifeBloodQuest => Get().lifeBloodQuest;

		public static GameObject EnemyPhysicalPusher => Get().enemyPhysicalPusher;

		public static RandomAudioClipTable EnemyDamageTickSoundTable => Get().enemyDamageTickSoundTable;

		public static GameObject BlackThreadEnemyStartEffect => Get().blackThreadEnemyStartEffect;

		public static GameObject BlackThreadEnemyEffect => Get().blackThreadEnemyEffect;

		public static GameObject BlackThreadEnemyDeathEffect => Get().blackThreadEnemyDeathEffect;

		public static AnimationCurve BlackThreadEnemyAttackTintCurve => Get().blackThreadEnemyAttackTintCurve;

		public static float BlackThreadEnemyAttackTintDuration => Get().blackThreadEnemyAttackTintDuration;

		public static AnimationCurve BlackThreadEnemyPulseTintCurve => Get().blackThreadEnemyPulseTintCurve;

		public static float BlackThreadEnemyPulseTintDuration => Get().blackThreadEnemyPulseTintDuration;

		public static GameObject BlackThreadPooledEffect => Get().blackThreadPooledEffect;

		public static BlackThreadAttack[] BlackThreadAttacksDefault => Get().blackThreadAttacksDefault;

		public static AudioMixerGroup BlackThreadVoiceMixerGroup => Get().blackThreadVoiceMixerGroup;

		public static Material CutoutSpriteMaterial => Get().cutoutSpriteMaterial;

		public static Color MossEffectsTintDust => Get().mossEffectsTintDust;

		public static Material DefaultLitMaterial => Get().defaultLitMaterial;

		public static Material DefaultUnlitMaterial => Get().defaultUnlitMaterial;

		public static AnimationCurve GlowResponsePulseCurve => Get().glowResponsePulseCurve;

		public static float GlowResponsePulseDuration => Get().glowResponsePulseDuration;

		public static float GlowResponsePulseFrameRate => Get().glowResponsePulseFrameRate;

		public static AudioEvent BeginMaggotedSound => Get().beginMaggotedSound;

		[RuntimeInitializeOnLoadMethod]
		public static void PreWarm()
		{
			GlobalSettingsBase<Effects>.StartPreloadAddressable("Global Effects Settings");
		}

		public static void Unload()
		{
			GlobalSettingsBase<Effects>.StartUnload();
		}

		private static Effects Get()
		{
			return GlobalSettingsBase<Effects>.Get("Global Effects Settings");
		}

		public static void DoBlackThreadHit(GameObject gameObject, HitInstance hitInstance, Vector3 effectOrigin)
		{
			Effects effects = Get();
			Vector3 position = gameObject.transform.TransformPoint(effectOrigin);
			effects.blackThreadHitSound.SpawnAndPlayOneShot(position);
			GameObject gameObject2 = ((hitInstance.HitEffectsType == EnemyHitEffectsProfile.EffectsTypes.Full) ? effects.hitShade.Spawn(position) : effects.hitShadeMinimal.Spawn(position));
			Transform transform = gameObject2.transform;
			transform.eulerAngles = DirectionUtils.GetCardinalDirection(hitInstance.Direction) switch
			{
				2 => new Vector3(0f, -90f, 0f), 
				0 => new Vector3(0f, 90f, 0f), 
				1 => new Vector3(-90f, 90f, 0f), 
				3 => new Vector3(-90f, 90f, 0f), 
				_ => gameObject2.transform.eulerAngles, 
			};
		}
	}
}
