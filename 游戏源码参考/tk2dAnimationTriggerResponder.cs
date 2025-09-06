using System;
using System.Linq;
using HutongGames.PlayMaker;
using UnityEngine;
using UnityEngine.Events;

public class tk2dAnimationTriggerResponder : MonoBehaviour
{
	[SerializeField]
	private tk2dSpriteAnimator animator;

	[SerializeField]
	private string eventInfo;

	[Space]
	[SerializeField]
	private PlayMakerFSM fsmTarget;

	[ModifiableProperty]
	[Conditional("fsmTarget", true, false, false)]
	[InspectorValidation("IsFsmEventValid")]
	[SerializeField]
	private string fsmEvent;

	[Space]
	public UnityEvent EventTriggered;

	private bool? IsFsmEventValid(string eventName)
	{
		if (!fsmTarget)
		{
			return null;
		}
		return fsmTarget.FsmEvents.Any((FsmEvent e) => e.Name.Equals(eventName));
	}

	private void OnEnable()
	{
		if ((bool)animator)
		{
			tk2dSpriteAnimator obj = animator;
			obj.AnimationEventTriggered = (Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip, int>)Delegate.Combine(obj.AnimationEventTriggered, new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip, int>(OnAnimationEventTriggered));
		}
	}

	private void OnDisable()
	{
		if ((bool)animator)
		{
			tk2dSpriteAnimator obj = animator;
			obj.AnimationEventTriggered = (Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip, int>)Delegate.Remove(obj.AnimationEventTriggered, new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip, int>(OnAnimationEventTriggered));
		}
	}

	public void ReSubEvents()
	{
		if ((bool)animator)
		{
			tk2dSpriteAnimator obj = animator;
			obj.AnimationEventTriggered = (Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip, int>)Delegate.Remove(obj.AnimationEventTriggered, new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip, int>(OnAnimationEventTriggered));
			tk2dSpriteAnimator obj2 = animator;
			obj2.AnimationEventTriggered = (Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip, int>)Delegate.Combine(obj2.AnimationEventTriggered, new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip, int>(OnAnimationEventTriggered));
		}
	}

	private void OnAnimationEventTriggered(tk2dSpriteAnimator _, tk2dSpriteAnimationClip clip, int frame)
	{
		if (string.IsNullOrEmpty(eventInfo) || clip == null || clip.frames[frame].eventInfo.Equals(eventInfo))
		{
			if ((bool)fsmTarget)
			{
				fsmTarget.SendEvent(fsmEvent);
			}
			EventTriggered.Invoke();
		}
	}
}
