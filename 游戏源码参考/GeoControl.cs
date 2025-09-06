using System;
using TeamCherry.SharedUtils;
using UnityEngine;

public class GeoControl : CurrencyObject<GeoControl>, IBreakOnContact
{
	[SerializeField]
	private string airAnim;

	[SerializeField]
	private string landAnim;

	[SerializeField]
	private string idleAnim;

	[SerializeField]
	private string gleamAnim;

	[SerializeField]
	private MinMaxFloat gleamDelayRange;

	[SerializeField]
	private GameObject thiefCharmEffectPrefab;

	[Space]
	[SerializeField]
	private CostReference valueReference;

	private tk2dSpriteAnimator anim;

	private float gleamDelay;

	private tk2dSpriteAnimationClip gleamClip;

	private tk2dSpriteAnimationClip airClip;

	private tk2dSpriteAnimationClip landClip;

	private tk2dSpriteAnimationClip currentClip;

	private bool hasAnim;

	private bool hasGleamAnim;

	private bool hasValueReference;

	private bool hasAnimator;

	private Animator animator;

	private int airHash;

	private int gleamHash;

	private int idleHash;

	private int landHash;

	private bool started;

	protected override CurrencyType? CurrencyType => global::CurrencyType.Money;

	private void CacheAnimations()
	{
		if (hasAnim)
		{
			landClip = anim.GetClipByName(landAnim) ?? anim.GetClipByName(idleAnim);
			airClip = anim.GetClipByName(airAnim);
			gleamClip = anim.GetClipByName(gleamAnim);
			hasGleamAnim = gleamClip != null;
		}
	}

	private void CacheAnimator()
	{
		if (hasAnimator)
		{
			return;
		}
		animator = GetComponent<Animator>();
		hasAnimator = animator != null;
		if (hasAnimator)
		{
			airHash = Animator.StringToHash(airAnim);
			gleamHash = Animator.StringToHash(gleamAnim);
			idleHash = Animator.StringToHash(idleAnim);
			if (!string.IsNullOrEmpty(landAnim))
			{
				landHash = Animator.StringToHash(landAnim);
			}
			else
			{
				landHash = idleHash;
			}
		}
	}

	protected override void Awake()
	{
		base.Awake();
		anim = GetComponent<tk2dSpriteAnimator>();
		CacheAnimator();
		hasAnim = anim;
		if (hasAnim)
		{
			tk2dSpriteAnimator obj = anim;
			obj.AnimationCompleted = (Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip>)Delegate.Combine(obj.AnimationCompleted, new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip>(OnAnimationCompleted));
			CacheAnimations();
		}
		hasValueReference = valueReference;
	}

	protected override void Start()
	{
		base.Start();
		started = true;
		ComponentSingleton<GeoControlCallbackHooks>.Instance.OnUpdate += OnUpdate;
	}

	private void OnValidate()
	{
		hasAnimator = false;
		CacheAnimator();
	}

	protected override void OnEnable()
	{
		if (started)
		{
			ComponentSingleton<GeoControlCallbackHooks>.Instance.OnUpdate += OnUpdate;
		}
		base.OnEnable();
		PlayAir();
	}

	protected override void OnDisable()
	{
		ComponentSingleton<GeoControlCallbackHooks>.Instance.OnUpdate -= OnUpdate;
		base.OnDisable();
	}

	public override void CollectPopup()
	{
		PlayerData instance = PlayerData.instance;
		if (!string.IsNullOrEmpty(firstGetPDBool))
		{
			if (!instance.HasSeenGeoMid && "HasSeenGeoMid" == firstGetPDBool)
			{
				instance.HasSeenGeoMid = true;
			}
			else if (!instance.HasSeenGeoBig && "HasSeenGeoBig" == firstGetPDBool)
			{
				instance.HasSeenGeoBig = true;
			}
		}
		if (!instance.HasSeenGeo)
		{
			instance.HasSeenGeo = true;
			if (!popupName.IsEmpty)
			{
				UIMsgDisplay uIMsgDisplay = default(UIMsgDisplay);
				uIMsgDisplay.Name = popupName;
				uIMsgDisplay.Icon = popupSprite;
				uIMsgDisplay.IconScale = 1f;
				CollectableUIMsg.Spawn(uIMsgDisplay);
			}
		}
	}

	public void OnUpdate()
	{
		if ((hasGleamAnim || gleamHash != 0) && !(gleamDelay <= 0f))
		{
			gleamDelay -= Time.deltaTime;
			if (!(gleamDelay > 0f))
			{
				PlayGleam();
			}
		}
	}

	protected override void Land()
	{
		base.Land();
		PlayLand();
	}

	protected override void LeftGround()
	{
		base.LeftGround();
		PlayAir();
	}

	protected override bool Collected()
	{
		if (!hasValueReference)
		{
			return false;
		}
		CurrencyManager.AddGeo(valueReference.Value);
		return true;
	}

	private void OnAnimationCompleted(tk2dSpriteAnimator animator, tk2dSpriteAnimationClip clip)
	{
		if (clip.name == landAnim || clip.name == gleamAnim)
		{
			animator.Play(idleAnim);
			gleamDelay = gleamDelayRange.GetRandomValue();
		}
	}

	public void SpawnThiefCharmEffect()
	{
		if ((bool)thiefCharmEffectPrefab)
		{
			thiefCharmEffectPrefab.Spawn(base.transform, new Vector3(0f, 0f, -0.001f));
		}
	}

	private void PlayGleam()
	{
		if (hasAnimator)
		{
			animator.SetTrigger(gleamHash);
			gleamDelay = gleamDelayRange.GetRandomValue();
		}
		if (hasAnim)
		{
			anim.Play(gleamClip);
		}
	}

	private void PlayLand()
	{
		if (hasAnimator)
		{
			if (airHash != 0)
			{
				animator.SetBool(airHash, value: false);
			}
			else
			{
				animator.Play(landHash);
			}
			CancelGleam();
			gleamDelay = gleamDelayRange.GetRandomValue();
		}
		if (hasAnim && currentClip != landClip)
		{
			currentClip = landClip;
			anim.Play(landClip);
		}
	}

	private void PlayAir()
	{
		if (hasAnimator)
		{
			animator.SetBool(airHash, value: true);
			CancelGleam();
		}
		if (hasAnim && currentClip != airClip)
		{
			currentClip = airClip;
			anim.Play(airClip);
		}
	}

	private void CancelGleam()
	{
		if (hasAnimator)
		{
			animator.ResetTrigger(gleamHash);
			gleamDelay = 0f;
		}
	}
}
