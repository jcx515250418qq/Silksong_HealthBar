using System;
using System.Collections;
using UnityEngine;

public class HeroChargeEffects : ManagerSingleton<HeroChargeEffects>
{
	[SerializeField]
	private CameraShakeTarget burstCameraShake;

	[Space]
	[SerializeField]
	private float benchUseDelay;

	[SerializeField]
	private tk2dSpriteAnimator bindSilk;

	private string chargeAnim;

	private CollectableItem useItem;

	private bool wasAtBench;

	private bool takeAnimationControl;

	private bool didChargeBurst;

	private bool didChargeEnd;

	private bool blockedInput;

	private MeshRenderer bindSilkRenderer;

	private tk2dSpriteAnimator animator;

	private SpriteFlash spriteFlash;

	public bool IsCharging { get; private set; }

	public event Action ChargeBurst;

	public event Action ChargeEnd;

	protected override void Awake()
	{
		base.Awake();
		animator = GetComponent<tk2dSpriteAnimator>();
		spriteFlash = GetComponent<SpriteFlash>();
		if ((bool)bindSilk)
		{
			bindSilkRenderer = bindSilk.GetComponent<MeshRenderer>();
		}
	}

	private void OnDisable()
	{
		if (blockedInput)
		{
			HeroController instance = HeroController.instance;
			blockedInput = false;
			instance.RemoveInputBlocker(this);
		}
	}

	public void StartCharge(Color tintColor)
	{
		HeroController instance = HeroController.instance;
		bool flag = (wasAtBench = PlayerData.instance.atBench);
		didChargeBurst = false;
		didChargeEnd = false;
		IsCharging = true;
		if (flag)
		{
			chargeAnim = "Charge Up Bench";
			if (!useItem || !useItem.SkipBenchUseEffect)
			{
				if ((bool)bindSilkRenderer)
				{
					bindSilkRenderer.enabled = true;
				}
				if ((bool)bindSilk)
				{
					bindSilk.PlayFromFrame("Charge Up Bench Silk", 0);
				}
			}
		}
		else
		{
			chargeAnim = (instance.cState.onGround ? "Charge Up" : "Charge Up Air");
		}
		takeAnimationControl = false;
		if (!flag)
		{
			if (instance.controlReqlinquished && !InteractManager.BlockingInteractable)
			{
				DoChargeBurst();
				DoChargeEnd();
				return;
			}
			if (instance.cState.onGround)
			{
				blockedInput = true;
				instance.AddInputBlocker(this);
			}
			instance.StopAnimationControl();
			takeAnimationControl = true;
		}
		animator.PlayFromFrame(chargeAnim, 0);
		tk2dSpriteAnimator obj = animator;
		obj.AnimationEventTriggered = (Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip, int>)Delegate.Combine(obj.AnimationEventTriggered, new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip, int>(OnAnimationEventTriggered));
		tk2dSpriteAnimator obj2 = animator;
		obj2.AnimationCompleted = (Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip>)Delegate.Combine(obj2.AnimationCompleted, new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip>(OnAnimationCompleted));
		animator.AnimationChanged += OnAnimationChanged;
	}

	private void OnAnimationEventTriggered(tk2dSpriteAnimator anim, tk2dSpriteAnimationClip clip, int frame)
	{
		DoChargeBurst();
		anim.AnimationEventTriggered = (Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip, int>)Delegate.Remove(anim.AnimationEventTriggered, new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip, int>(OnAnimationEventTriggered));
	}

	private void DoChargeBurst()
	{
		didChargeBurst = true;
		IsCharging = false;
		burstCameraShake.DoShake(this);
		if (this.ChargeBurst != null)
		{
			this.ChargeBurst();
		}
		if ((bool)useItem)
		{
			useItem.ConsumeItemResponse();
			useItem = null;
		}
		if ((wasAtBench || HeroController.instance.controlReqlinquished) && (bool)spriteFlash)
		{
			spriteFlash.flashFocusHeal();
		}
	}

	private void OnAnimationCompleted(tk2dSpriteAnimator anim, tk2dSpriteAnimationClip clip)
	{
		if (!didChargeEnd)
		{
			didChargeEnd = true;
			if (!didChargeBurst)
			{
				DoChargeBurst();
			}
			DoChargeEnd();
			anim.AnimationCompleted = (Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip>)Delegate.Remove(anim.AnimationCompleted, new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip>(OnAnimationCompleted));
			anim.AnimationChanged -= OnAnimationChanged;
		}
	}

	private void OnAnimationChanged(tk2dSpriteAnimator tk2dSpriteAnimator, tk2dSpriteAnimationClip previousclip, tk2dSpriteAnimationClip newclip)
	{
		OnAnimationCompleted(tk2dSpriteAnimator, previousclip);
		tk2dSpriteAnimator.AnimationChanged -= OnAnimationChanged;
	}

	private void DoChargeEnd()
	{
		if (wasAtBench)
		{
			EventRegister.SendEvent(EventRegisterEvents.BenchRegainControl);
		}
		else if (takeAnimationControl)
		{
			HeroController.instance.StartAnimationControlToIdle();
		}
		if (blockedInput)
		{
			HeroController instance = HeroController.instance;
			blockedInput = false;
			instance.RemoveInputBlocker(this);
		}
		if (this.ChargeEnd != null)
		{
			this.ChargeEnd();
		}
	}

	public void DoUseBenchItem(CollectableItem item)
	{
		useItem = item;
		EventRegister.SendEvent(EventRegisterEvents.BenchRelinquishControl);
		StartCoroutine(StartChargeDelayed(benchUseDelay, Color.white));
	}

	private IEnumerator StartChargeDelayed(float delay, Color chargeColor)
	{
		yield return new WaitForSeconds(delay);
		StartCharge(chargeColor);
	}
}
