using System;
using System.Collections;
using UnityEngine;

public class HealthFlyer : MonoBehaviour, IHitResponder
{
	[SerializeField]
	private string fallAnim;

	[SerializeField]
	private string landAnim;

	[SerializeField]
	private string flyAnim;

	[Space]
	[SerializeField]
	private CollisionEnterEvent landDetector;

	[SerializeField]
	private RandomAudioClipTable landAudio;

	[Space]
	[SerializeField]
	private GameObject corpsePrefab;

	[SerializeField]
	private GameObject splatEffectChild;

	[SerializeField]
	private GameObject strikeNailPrefab;

	[SerializeField]
	private GameObject slashImpactPrefab;

	[SerializeField]
	private GameObject fireballHitPrefab;

	[SerializeField]
	private AudioEvent deathSound1;

	[SerializeField]
	private AudioEvent deathSound2;

	[SerializeField]
	private GameObject pool;

	[SerializeField]
	private GameObject screenFlash;

	[SerializeField]
	private Color bloodColor;

	[SerializeField]
	private CameraShakeTarget killShake;

	[SerializeField]
	private EnemyJournalRecord journalData;

	[Space]
	[SerializeField]
	private PlayMakerFSM flyBehaviour;

	[SerializeField]
	private AudioSource flyLoopAudio;

	private tk2dSpriteAnimator animator;

	private Rigidbody2D body;

	private bool alive;

	private bool landed;

	private const float ACTIVATE_DELAY = 0.25f;

	private double activateTime;

	private static readonly int _colorId = Shader.PropertyToID("_Color");

	private void Awake()
	{
		body = GetComponent<Rigidbody2D>();
		animator = GetComponent<tk2dSpriteAnimator>();
		if ((bool)animator)
		{
			tk2dSpriteAnimator obj = animator;
			obj.AnimationCompleted = (Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip>)Delegate.Combine(obj.AnimationCompleted, new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip>(OnAnimationCompleted));
		}
		if (!landDetector)
		{
			return;
		}
		landDetector.CollisionEnteredDirectional += delegate(CollisionEnterEvent.Direction direction, Collision2D _)
		{
			if (!landed && direction == CollisionEnterEvent.Direction.Bottom)
			{
				DoLand();
			}
		};
	}

	private void OnEnable()
	{
		base.transform.SetScaleMatching(UnityEngine.Random.Range(1.35f, 1.5f));
		activateTime = Time.timeAsDouble + 0.25;
		alive = true;
		landed = false;
		body.gravityScale = 1f;
		if ((bool)animator)
		{
			animator.Play(fallAnim);
		}
	}

	private IEnumerator Heal()
	{
		Action doHeal = null;
		doHeal = delegate
		{
			GameManager instance = GameManager.instance;
			instance.AddBlueHealthQueued();
			instance.UnloadingLevel -= doHeal;
			doHeal = null;
		};
		GameManager.instance.UnloadingLevel += doHeal;
		if ((bool)HeroController.instance && Vector2.Distance(base.transform.position, HeroController.instance.transform.position) > 40f)
		{
			base.gameObject.SetActive(value: false);
		}
		yield return new WaitForSeconds(1.2f);
		if ((bool)screenFlash)
		{
			GameObject obj = screenFlash.Spawn();
			obj.GetComponent<Renderer>().material.SetColor(_colorId, new Color(0f, 0.7f, 1f));
			PlayMakerFSM playMakerFSM = FSMUtility.LocateFSM(obj, "Fade Away");
			if ((bool)playMakerFSM)
			{
				FSMUtility.SetFloat(playMakerFSM, "Alpha", 0.75f);
			}
		}
		doHeal?.Invoke();
		base.gameObject.SetActive(value: false);
	}

	public IHitResponder.HitResponse Hit(HitInstance damageInstance)
	{
		if (Time.timeAsDouble < activateTime)
		{
			return IHitResponder.Response.None;
		}
		if (!alive)
		{
			return IHitResponder.Response.None;
		}
		alive = false;
		if ((bool)corpsePrefab)
		{
			UnityEngine.Object.Instantiate(corpsePrefab, base.transform.position, base.transform.rotation);
		}
		if ((bool)splatEffectChild)
		{
			splatEffectChild.SetActive(value: true);
		}
		if ((bool)journalData)
		{
			journalData.Get();
		}
		bool flag = false;
		if (damageInstance.IsNailDamage)
		{
			flag = true;
			if ((bool)strikeNailPrefab)
			{
				strikeNailPrefab.Spawn(base.transform.position);
			}
			if ((bool)slashImpactPrefab)
			{
				GameObject gameObject = slashImpactPrefab.Spawn(base.transform.position);
				switch (DirectionUtils.GetCardinalDirection(damageInstance.GetOverriddenDirection(base.transform, HitInstance.TargetType.Regular)))
				{
				case 1:
					gameObject.transform.SetRotation2D(UnityEngine.Random.Range(70f, 110f));
					gameObject.transform.localScale = new Vector3(0.9f, 0.9f, 1f);
					break;
				case 3:
					gameObject.transform.SetRotation2D(UnityEngine.Random.Range(250f, 290f));
					gameObject.transform.localScale = new Vector3(0.9f, 0.9f, 1f);
					break;
				case 2:
					gameObject.transform.SetRotation2D(UnityEngine.Random.Range(340f, 380f));
					gameObject.transform.localScale = new Vector3(-0.9f, 0.9f, 1f);
					break;
				case 0:
					gameObject.transform.SetRotation2D(UnityEngine.Random.Range(340f, 380f));
					gameObject.transform.localScale = new Vector3(0.9f, 0.9f, 1f);
					break;
				}
			}
		}
		else if (damageInstance.AttackType == AttackTypes.Spell || damageInstance.AttackType == AttackTypes.NailBeam)
		{
			flag = true;
			if ((bool)fireballHitPrefab)
			{
				GameObject obj = fireballHitPrefab.Spawn(base.transform.position);
				obj.transform.localScale = new Vector3(0.5f, 0.5f, 1f);
				obj.transform.SetPositionZ(0.0031f);
			}
		}
		else if (damageInstance.AttackType == AttackTypes.Generic)
		{
			flag = true;
		}
		deathSound1.SpawnAndPlayOneShot(base.transform.position);
		deathSound2.SpawnAndPlayOneShot(base.transform.position);
		killShake.DoShake(this);
		BloodSpawner.SpawnBlood(base.transform.position, 12, 18, 4f, 22f, 30f, 150f, bloodColor);
		Renderer component = GetComponent<Renderer>();
		if ((bool)component)
		{
			component.enabled = false;
		}
		if (flag)
		{
			if ((bool)pool)
			{
				pool.transform.SetPositionZ(-0.2f);
				FlingUtils.ChildrenConfig config = default(FlingUtils.ChildrenConfig);
				config.Parent = pool;
				config.AmountMin = 8;
				config.AmountMax = 10;
				config.SpeedMin = 15f;
				config.SpeedMax = 20f;
				config.AngleMin = 30f;
				config.AngleMax = 150f;
				config.OriginVariationX = 0f;
				config.OriginVariationY = 0f;
				FlingUtils.FlingChildren(config, base.transform, Vector3.zero, null);
			}
			StartCoroutine(Heal());
		}
		else
		{
			base.gameObject.SetActive(value: false);
		}
		return IHitResponder.Response.GenericHit;
	}

	public void DoLand()
	{
		landed = true;
		if ((bool)animator)
		{
			animator.Play(landAnim);
		}
		landAudio.SpawnAndPlayOneShot(base.transform.position);
	}

	private void StartFly()
	{
		if ((bool)animator)
		{
			animator.Play(flyAnim);
		}
		body.gravityScale = 0f;
		if ((bool)flyBehaviour)
		{
			flyBehaviour.SendEvent("FLY");
		}
		if ((bool)flyLoopAudio)
		{
			flyLoopAudio.Play();
		}
	}

	private void OnAnimationCompleted(tk2dSpriteAnimator _, tk2dSpriteAnimationClip clip)
	{
		if (clip.name == landAnim)
		{
			StartFly();
		}
	}
}
