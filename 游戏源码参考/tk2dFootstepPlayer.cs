using System;
using UnityEngine;

public sealed class tk2dFootstepPlayer : FootstepPlayer
{
	[Serializable]
	public sealed class tk2dEventTrigger
	{
		public EventType triggerType;

		[ModifiableProperty]
		[Conditional("IsString", true, true, true)]
		public string eventString;

		[ModifiableProperty]
		[Conditional("IsInt", true, true, true)]
		public int eventInt;

		[ModifiableProperty]
		[Conditional("IsFloat", true, true, true)]
		public float eventFloat;

		private bool IsString()
		{
			return triggerType == EventType.String;
		}

		private bool IsInt()
		{
			return triggerType == EventType.Int;
		}

		private bool IsFloat()
		{
			return triggerType == EventType.Float;
		}

		public bool TriggerMatched(tk2dSpriteAnimationFrame frame)
		{
			return triggerType switch
			{
				EventType.String => frame.eventInfo == eventString, 
				EventType.Int => frame.eventInt == eventInt, 
				EventType.Float => frame.eventFloat == eventFloat, 
				_ => false, 
			};
		}
	}

	public enum EventType
	{
		String = 0,
		Int = 1,
		Float = 2
	}

	[SerializeField]
	private tk2dSpriteAnimator animator;

	[SerializeField]
	private tk2dEventTrigger footstepTrigger = new tk2dEventTrigger
	{
		eventString = "Footstep"
	};

	private bool registeredEvent;

	protected override void Awake()
	{
		base.Awake();
		if (animator == null)
		{
			animator = GetComponent<tk2dSpriteAnimator>();
			_ = animator == null;
		}
	}

	protected override void OnValidate()
	{
		base.OnValidate();
		if (animator == null)
		{
			animator = GetComponent<tk2dSpriteAnimator>();
		}
	}

	private void OnEnable()
	{
		RegisterEvents();
	}

	private void OnDisable()
	{
		UnregisterEvents();
	}

	private void RegisterEvents()
	{
		if (!registeredEvent && (bool)animator)
		{
			registeredEvent = true;
			animator.AnimationEventTriggeredEvent += AnimationEventTriggered;
		}
	}

	private void UnregisterEvents()
	{
		if (registeredEvent && (bool)animator)
		{
			registeredEvent = false;
			animator.AnimationEventTriggeredEvent -= AnimationEventTriggered;
		}
	}

	private void AnimationEventTriggered(tk2dSpriteAnimator animator, tk2dSpriteAnimationClip clip, int frame)
	{
		if (footstepTrigger.TriggerMatched(clip.frames[frame]))
		{
			PlayFootstep();
		}
	}
}
