using UnityEngine;

public class NailSlash : NailAttackBase
{
	public string animName;

	[Space]
	[SerializeField]
	[AssetPickerDropdown]
	private HeroSlashBounceConfig bounceConfig;

	private tk2dSpriteAnimator anim;

	private MeshRenderer mesh;

	private NailSlashTravel travel;

	private bool queuedDownspikeBounce;

	private int animTriggerCounter;

	private PolygonCollider2D poly;

	private AudioSource audio;

	[SerializeField]
	[ModifiableProperty]
	[Conditional("dontAddRecoiler", false, false, false)]
	private bool drillPull;

	[SerializeField]
	private bool dontAddRecoiler;

	[Space]
	[SerializeField]
	private bool setSlashComponent;

	private bool hasPoly;

	private float originalPitch;

	public bool IsSlashOut { get; private set; }

	public bool IsStartingSlash { get; private set; }

	protected override void Awake()
	{
		base.Awake();
		audio = GetComponent<AudioSource>();
		anim = GetComponent<tk2dSpriteAnimator>();
		poly = GetComponent<PolygonCollider2D>();
		hasPoly = poly != null;
		mesh = GetComponent<MeshRenderer>();
		travel = GetComponent<NailSlashTravel>();
		originalPitch = audio.pitch;
		if (!travel)
		{
			hc.FlippedSprite += OnHeroFlipped;
		}
		poly.enabled = false;
		if ((bool)mesh)
		{
			mesh.enabled = false;
		}
		if (!dontAddRecoiler)
		{
			NailSlashRecoil.Add(base.gameObject, enemyDamager, drillPull);
		}
	}

	private void OnDestroy()
	{
		hc.FlippedSprite -= OnHeroFlipped;
	}

	public void StartSlash()
	{
		if (setSlashComponent)
		{
			hc.SetSlashComponent(this);
		}
		OnSlashStarting();
		float num = originalPitch;
		if (hc.IsUsingQuickening)
		{
			num += 0.05f;
		}
		audio.pitch = num;
		audio.Play();
		animTriggerCounter = 0;
		poly.enabled = false;
		queuedDownspikeBounce = false;
		base.IsDamagerActive = true;
		IsStartingSlash = true;
		PlaySlash();
		if (drillPull)
		{
			hc.DrillDash(hc.transform.localScale.x < 0f);
		}
	}

	public void CancelAttack()
	{
		CancelAttack(forceHide: true);
	}

	public void CancelAttack(bool forceHide)
	{
		IsStartingSlash = false;
		SetCollidersActive(value: false);
		if ((bool)mesh && (forceHide || ((bool)bounceConfig && bounceConfig.HideSlashOnBounceCancel)))
		{
			mesh.enabled = false;
		}
		base.IsDamagerActive = false;
		anim.AnimationEventTriggered = null;
		queuedDownspikeBounce = false;
		OnCancelAttack();
	}

	private void PlaySlash()
	{
		if ((bool)mesh)
		{
			mesh.enabled = true;
		}
		anim.AnimationEventTriggered = OnAnimationEventTriggered;
		anim.AnimationCompleted = OnAnimationCompleted;
		float num = (hc.IsUsingQuickening ? hc.Config.QuickAttackSpeedMult : 1f);
		tk2dSpriteAnimationClip clipByName = anim.GetClipByName(animName);
		anim.Play(clipByName, Mathf.Epsilon, clipByName.fps * num);
		OnPlaySlash();
	}

	private void OnAnimationEventTriggered(tk2dSpriteAnimator animator, tk2dSpriteAnimationClip clip, int frame)
	{
		if (clip.frames[frame].eventInfo == "Bounce")
		{
			if (queuedDownspikeBounce)
			{
				queuedDownspikeBounce = false;
				DownBounce();
			}
			else
			{
				queuedDownspikeBounce = true;
			}
			return;
		}
		animTriggerCounter++;
		if (animTriggerCounter == 1)
		{
			IsStartingSlash = false;
			SetCollidersActive(value: true);
			if ((bool)base.ExtraDamager)
			{
				base.ExtraDamager.SetActive(value: true);
			}
			IsSlashOut = true;
		}
		if (animTriggerCounter == 2)
		{
			SetCollidersActive(value: false);
			if ((bool)base.ExtraDamager)
			{
				base.ExtraDamager.SetActive(value: false);
			}
			base.IsDamagerActive = false;
			enemyDamager.EndDamage();
			IsSlashOut = false;
		}
	}

	public void SetCollidersActive(bool value)
	{
		if (hasPoly)
		{
			poly.enabled = value;
		}
		if ((bool)clashTinkPoly)
		{
			clashTinkPoly.enabled = value;
		}
	}

	private void OnAnimationCompleted(tk2dSpriteAnimator sprite, tk2dSpriteAnimationClip clip)
	{
		CancelAttack();
		anim.AnimationCompleted = null;
	}

	private void OnHeroFlipped()
	{
		if (base.isActiveAndEnabled)
		{
			CancelAttack();
		}
	}

	private void DownBounce()
	{
		hc.CancelAttack();
		DoDownspikeBounce();
	}

	public void DoDownspikeBounce()
	{
		hc.DownspikeBounce(harpoonRecoil: false, bounceConfig);
	}

	public override void QueueBounce()
	{
		base.QueueBounce();
		if (queuedDownspikeBounce)
		{
			queuedDownspikeBounce = false;
			DownBounce();
		}
		else
		{
			hc.DownspikeBounce(harpoonRecoil: false);
			queuedDownspikeBounce = true;
		}
	}
}
