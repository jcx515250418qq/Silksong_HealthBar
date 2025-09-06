using System;
using System.Collections;
using UnityEngine;

public class HeroTalkAnimation : MonoBehaviour
{
	public enum AnimationTypes
	{
		Custom = -1,
		LookForward = 0,
		LookUp = 1,
		LookDown = 2,
		LookHalfUp = 3,
		Kneeling = 4,
		LookHalfDown = 5
	}

	[Serializable]
	private struct AnimationGroup
	{
		public string TalkAnim;

		public string TalkBackwardAnim;

		public string ListenAnim;

		public string ListenBackwardAnim;

		public string TurnBackwardAnim;

		public string TurnForwardAnim;

		public string TalkBackwardAnimSafe
		{
			get
			{
				if (!string.IsNullOrEmpty(TalkBackwardAnim))
				{
					return TalkBackwardAnim;
				}
				return TalkAnim;
			}
		}

		public string ListenBackwardAnimSafe
		{
			get
			{
				if (!string.IsNullOrEmpty(ListenBackwardAnim))
				{
					return ListenBackwardAnim;
				}
				return ListenAnim;
			}
		}
	}

	[SerializeField]
	[ArrayForEnum(typeof(AnimationTypes))]
	private AnimationGroup[] animationGroups;

	[SerializeField]
	private AnimationGroup hurtAnims;

	[SerializeField]
	[ArrayForEnum(typeof(AnimationTypes))]
	private AnimationGroup[] hurtAnimationGroups;

	[SerializeField]
	[ArrayForEnum(typeof(AnimationTypes))]
	private AnimationGroup[] windyHurtAnimationGroups;

	[SerializeField]
	private RandomAudioClipTable talkAudioTable;

	private bool wasTalking;

	private bool wasAnimating;

	private int linesSinceLastSpeak;

	private NPCControlBase sourceAnimNpc;

	private bool hasControl;

	private WaitForTk2dAnimatorClipFinish endHurtAnimWait;

	private Action onEndHurtAnimCompletion;

	private tk2dSpriteAnimator animator;

	private HeroAnimationController heroAnim;

	private HeroController hc;

	private bool isBlocked;

	private Transform watchTarget;

	private Coroutine watchTargetRoutine;

	private bool isTurning;

	private bool isFacingBackward;

	private Action onEndedFacingForward;

	private static HeroTalkAnimation _instance;

	private static RandomAudioClipTable overrideTalkAudioTable;

	private static int talkOverrideID;

	public static RandomAudioClipTable TalkAudioTable
	{
		get
		{
			if (overrideTalkAudioTable != null)
			{
				return overrideTalkAudioTable;
			}
			if (!_instance)
			{
				return null;
			}
			return _instance.talkAudioTable;
		}
		set
		{
			SetTalkTableOverride(value);
		}
	}

	public static bool IsEndingHurtAnim
	{
		get
		{
			if ((bool)_instance)
			{
				return _instance.endHurtAnimWait != null;
			}
			return false;
		}
	}

	private bool IsHurtAnimOverridden
	{
		get
		{
			if (sourceAnimNpc != null)
			{
				return sourceAnimNpc.OverrideHeroHurtAnim;
			}
			return false;
		}
	}

	public static int SetTalkTableOverride(RandomAudioClipTable randomAudioClipTable)
	{
		overrideTalkAudioTable = randomAudioClipTable;
		return ++talkOverrideID;
	}

	public static bool RemoveTalkTableOverride(int ID)
	{
		if (ID == talkOverrideID)
		{
			overrideTalkAudioTable = null;
			return true;
		}
		return false;
	}

	private void OnValidate()
	{
		ArrayForEnumAttribute.EnsureArraySize(ref animationGroups, typeof(AnimationTypes));
		ArrayForEnumAttribute.EnsureArraySize(ref hurtAnimationGroups, typeof(AnimationTypes));
		ArrayForEnumAttribute.EnsureArraySize(ref windyHurtAnimationGroups, typeof(AnimationTypes));
	}

	private void Awake()
	{
		OnValidate();
		hc = GetComponent<HeroController>();
		animator = GetComponent<tk2dSpriteAnimator>();
		heroAnim = GetComponent<HeroAnimationController>();
	}

	private void Start()
	{
		_instance = this;
	}

	private void OnDestroy()
	{
		if (_instance == this)
		{
			_instance = null;
		}
	}

	public static void EnterConversation(NPCControlBase sourceNpc)
	{
		if ((bool)_instance)
		{
			_instance.InternalEnterConversation(sourceNpc);
		}
	}

	public static void ExitConversation()
	{
		if ((bool)_instance)
		{
			_instance.InternalExitConversation();
		}
	}

	public static void SetTalking(bool setTalking, bool setAnimating)
	{
		if ((bool)_instance)
		{
			_instance.InternalSetTalking(setTalking, setAnimating);
		}
	}

	public static void SetBlocked(bool value)
	{
		if ((bool)_instance)
		{
			_instance.isBlocked = value;
			if (value)
			{
				_instance.hasControl = false;
			}
		}
	}

	public static void SetWatchTarget(Transform target, Action onEndedFacingForward)
	{
		if (!_instance)
		{
			return;
		}
		_instance.onEndedFacingForward = onEndedFacingForward;
		if ((bool)target)
		{
			if (_instance.hc.HasAnimationControl)
			{
				_instance.TakeAnimationControl();
			}
			_instance.watchTarget = target;
			if (_instance.watchTargetRoutine == null)
			{
				_instance.watchTargetRoutine = _instance.StartCoroutine(_instance.KeepFacingTarget());
			}
		}
		else
		{
			_instance.watchTarget = null;
			if (_instance.watchTargetRoutine == null)
			{
				onEndedFacingForward?.Invoke();
			}
		}
	}

	private void SkipWaiting()
	{
		if (onEndHurtAnimCompletion != null)
		{
			onEndHurtAnimCompletion();
			onEndHurtAnimCompletion = null;
		}
		if (endHurtAnimWait != null)
		{
			endHurtAnimWait.Cancel();
			endHurtAnimWait = null;
		}
	}

	private void InternalEnterConversation(NPCControlBase sourceNpc)
	{
		wasTalking = false;
		sourceAnimNpc = sourceNpc;
		if (hc.AnimCtrl.IsHurt() && hc.AnimCtrl.IsPlayingHurtAnim && IsHurtAnimOverridden)
		{
			if (endHurtAnimWait == null)
			{
				onEndHurtAnimCompletion = delegate
				{
					InternalSetTalking(setTalking: false, setAnimating: true);
				};
				EndHurtAnim();
			}
		}
		else if (sourceAnimNpc.HeroAnimation != 0)
		{
			SkipWaiting();
			TakeAnimationControl();
			InternalSetTalking(setTalking: false, setAnimating: true);
		}
	}

	private void TakeAnimationControl()
	{
		hc.StopAnimationControl();
		hasControl = true;
	}

	private void EndHurtAnim()
	{
		TakeAnimationControl();
		tk2dSpriteAnimationClip clip2 = heroAnim.GetClip("Hurt To Idle");
		animator.Play(clip2);
		endHurtAnimWait = new WaitForTk2dAnimatorClipFinish(animator, delegate(tk2dSpriteAnimator anim, tk2dSpriteAnimationClip clip)
		{
			if (clip.name == "Hurt To Idle")
			{
				tk2dSpriteAnimationClip clip3 = heroAnim.GetClip("Idle");
				anim.Play(clip3);
			}
			endHurtAnimWait = null;
			if (onEndHurtAnimCompletion != null)
			{
				onEndHurtAnimCompletion();
				onEndHurtAnimCompletion = null;
			}
		});
	}

	private void InternalExitConversation()
	{
		SkipWaiting();
		isBlocked = false;
		if (hasControl)
		{
			hasControl = false;
			if (!hc.HasAnimationControl)
			{
				hc.StartAnimationControl();
			}
		}
	}

	private void InternalSetTalking(bool setTalking, bool setAnimating)
	{
		if (setTalking)
		{
			if (setAnimating)
			{
				if (wasTalking && linesSinceLastSpeak < 1)
				{
					linesSinceLastSpeak++;
				}
				else
				{
					linesSinceLastSpeak = 0;
					NPCSpeakingAudio.PlayVoice(TalkAudioTable, base.transform.position);
				}
			}
		}
		else
		{
			linesSinceLastSpeak = 0;
		}
		wasTalking = setTalking;
		wasAnimating = setAnimating;
		if (((bool)sourceAnimNpc && sourceAnimNpc.HeroAnimation == AnimationTypes.Custom) || isBlocked)
		{
			return;
		}
		if (setAnimating)
		{
			hasControl = true;
		}
		if (hasControl && !isTurning && endHurtAnimWait == null)
		{
			if (hc.HasAnimationControl && setAnimating)
			{
				TakeAnimationControl();
			}
			if (!hc.HasAnimationControl)
			{
				PlayCorrectAnimation(setTalking, skipToLoop: false);
			}
		}
	}

	private AnimationGroup GetAnimGroup(AnimationTypes animationType)
	{
		if (hc.AnimCtrl.IsHurt())
		{
			if (hc.cState.inWindRegion || hc.cState.inUpdraft)
			{
				return windyHurtAnimationGroups[(int)animationType];
			}
			return hurtAnimationGroups[(int)animationType];
		}
		return animationGroups[(int)animationType];
	}

	private AnimationGroup GetCurrentAnimGroup()
	{
		if (hc.AnimCtrl.IsHurt() && !IsHurtAnimOverridden)
		{
			if (!sourceAnimNpc || sourceAnimNpc.HeroAnimation < AnimationTypes.LookForward)
			{
				return hurtAnimationGroups[0];
			}
			AnimationTypes heroAnimation = sourceAnimNpc.HeroAnimation;
			if (heroAnimation != AnimationTypes.Custom && heroAnimation != AnimationTypes.Kneeling)
			{
				if (hc.cState.inWindRegion || hc.cState.inUpdraft)
				{
					return windyHurtAnimationGroups[(int)heroAnimation];
				}
				return hurtAnimationGroups[(int)heroAnimation];
			}
		}
		if (!sourceAnimNpc || sourceAnimNpc.HeroAnimation < AnimationTypes.LookForward)
		{
			return animationGroups[0];
		}
		return animationGroups[(int)sourceAnimNpc.HeroAnimation];
	}

	private IEnumerator KeepFacingTarget()
	{
		bool setAnimating = !wasAnimating;
		while (true)
		{
			if (endHurtAnimWait != null)
			{
				if (!watchTarget)
				{
					SkipWaiting();
				}
				yield return null;
				continue;
			}
			float num;
			if ((bool)watchTarget)
			{
				float watchTargetDirection = GetWatchTargetDirection();
				num = (float)(hc.cState.facingRight ? 1 : (-1)) * watchTargetDirection;
			}
			else
			{
				num = 1f;
			}
			if (num < 0f)
			{
				if (!isFacingBackward)
				{
					isTurning = true;
					AnimationGroup currentAnimGroup = GetCurrentAnimGroup();
					yield return new WaitForSeconds(animator.PlayAnimGetTime(heroAnim.GetClip(currentAnimGroup.TurnBackwardAnim)));
					isFacingBackward = true;
				}
			}
			else if (isFacingBackward)
			{
				isTurning = true;
				AnimationGroup currentAnimGroup2 = GetCurrentAnimGroup();
				yield return new WaitForSeconds(animator.PlayAnimGetTime(heroAnim.GetClip(currentAnimGroup2.TurnForwardAnim)));
				isFacingBackward = false;
			}
			if (isTurning || setAnimating != wasAnimating)
			{
				isTurning = false;
				setAnimating = wasAnimating;
				PlayCorrectAnimation(wasTalking, skipToLoop: true);
			}
			if (!watchTarget && !isFacingBackward)
			{
				break;
			}
			yield return null;
		}
		isFacingBackward = false;
		watchTargetRoutine = null;
		onEndedFacingForward?.Invoke();
	}

	private float GetWatchTargetDirection()
	{
		Vector2 vector = base.transform.position;
		return Mathf.Sign(((Vector2)watchTarget.position).x - vector.x);
	}

	private void PlayCorrectAnimation(bool isTalking, bool skipToLoop)
	{
		AnimationGroup currentAnimGroup = GetCurrentAnimGroup();
		tk2dSpriteAnimationClip clip = heroAnim.GetClip(isFacingBackward ? currentAnimGroup.TalkBackwardAnimSafe : currentAnimGroup.TalkAnim);
		tk2dSpriteAnimationClip tk2dSpriteAnimationClip2;
		if (isTalking)
		{
			tk2dSpriteAnimationClip2 = clip;
		}
		else
		{
			tk2dSpriteAnimationClip2 = heroAnim.GetClip(isFacingBackward ? currentAnimGroup.ListenBackwardAnimSafe : currentAnimGroup.ListenAnim);
			if (tk2dSpriteAnimationClip2.wrapMode == tk2dSpriteAnimationClip.WrapMode.LoopSection && animator.IsPlaying(clip) && tk2dSpriteAnimationClip2 != clip)
			{
				skipToLoop = true;
			}
		}
		if (skipToLoop)
		{
			animator.PlayFromFrame(tk2dSpriteAnimationClip2, tk2dSpriteAnimationClip2.loopStart);
		}
		else
		{
			animator.Play(tk2dSpriteAnimationClip2);
		}
	}

	public void PlayCorrectLookAnimation(AnimationTypes animType, bool skipToLoop)
	{
		AnimationGroup currentAnimGroup = GetCurrentAnimGroup();
		AnimationGroup animGroup = GetAnimGroup(animType);
		tk2dSpriteAnimationClip clip = heroAnim.GetClip(isFacingBackward ? currentAnimGroup.TalkBackwardAnimSafe : currentAnimGroup.TalkAnim);
		tk2dSpriteAnimationClip clip2 = heroAnim.GetClip(isFacingBackward ? animGroup.ListenBackwardAnimSafe : animGroup.ListenAnim);
		if (clip2.wrapMode == tk2dSpriteAnimationClip.WrapMode.LoopSection && animator.IsPlaying(clip))
		{
			skipToLoop = true;
		}
		if (skipToLoop)
		{
			animator.PlayFromFrame(clip2, clip2.loopStart);
		}
		else
		{
			animator.Play(clip2);
		}
	}

	public static void PlayLookAnimation(AnimationTypes animType, bool skipToLoop)
	{
		if (!(_instance == null))
		{
			_instance.PlayCorrectLookAnimation(animType, skipToLoop);
		}
	}
}
