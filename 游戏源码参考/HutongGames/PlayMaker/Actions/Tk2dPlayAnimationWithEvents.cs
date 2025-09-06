using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("2D Toolkit/SpriteAnimator")]
	[Tooltip("Plays a sprite animation. \nCan receive animation events and animation complete event. \nNOTE: The Game Object must have a tk2dSpriteAnimator attached.")]
	public class Tk2dPlayAnimationWithEvents : FsmStateAction
	{
		[RequiredField]
		[Tooltip("The Game Object to work with. NOTE: The Game Object must have a tk2dSpriteAnimator component attached.")]
		[CheckForComponent(typeof(tk2dSpriteAnimator))]
		public FsmOwnerDefault gameObject;

		[RequiredField]
		[Tooltip("The clip name to play")]
		public FsmString clipName;

		[Tooltip("Trigger event defined in the clip. The event holds the following triggers infos: the eventInt, eventInfo and eventFloat properties")]
		public FsmEvent animationTriggerEvent;

		[Tooltip("Animation complete event. The event holds the clipId reference")]
		public FsmEvent animationCompleteEvent;

		private tk2dSpriteAnimator _sprite;

		private bool hasExpectedClip;

		private tk2dSpriteAnimationClip expectedClip;

		private void _getSprite()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(gameObject);
			if (!(ownerDefaultTarget == null))
			{
				_sprite = ownerDefaultTarget.GetComponent<tk2dSpriteAnimator>();
			}
		}

		public override void Reset()
		{
			gameObject = null;
			clipName = null;
			animationTriggerEvent = null;
			animationCompleteEvent = null;
		}

		public override void OnEnter()
		{
			hasExpectedClip = false;
			_getSprite();
			DoPlayAnimationWithEvents();
		}

		private void DoPlayAnimationWithEvents()
		{
			if (_sprite == null)
			{
				LogWarning("Missing tk2dSpriteAnimator component");
				return;
			}
			IHeroAnimationController component = _sprite.GetComponent<IHeroAnimationController>();
			if (component != null)
			{
				expectedClip = component.GetClip(clipName.Value);
				_sprite.Play(expectedClip);
			}
			else
			{
				expectedClip = _sprite.GetClipByName(clipName.Value);
				_sprite.Play(expectedClip);
			}
			hasExpectedClip = expectedClip != null;
			bool flag = false;
			if (animationTriggerEvent != null)
			{
				_sprite.AnimationEventTriggered = AnimationEventDelegate;
				flag = true;
			}
			if (animationCompleteEvent != null)
			{
				_sprite.AnimationCompleted = AnimationCompleteDelegate;
				flag = true;
			}
			if (!hasExpectedClip && flag)
			{
				base.Fsm.Event(animationTriggerEvent);
				base.Fsm.Event(animationCompleteEvent);
			}
			if (!flag || !hasExpectedClip)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			if (hasExpectedClip && expectedClip != _sprite.CurrentClip)
			{
				base.Fsm.Event(animationTriggerEvent);
				base.Fsm.Event(animationCompleteEvent);
				Finish();
			}
		}

		public override void OnExit()
		{
			if (!(_sprite == null))
			{
				tk2dSpriteAnimator sprite = _sprite;
				sprite.AnimationEventTriggered = (Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip, int>)Delegate.Remove(sprite.AnimationEventTriggered, new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip, int>(AnimationEventDelegate));
				tk2dSpriteAnimator sprite2 = _sprite;
				sprite2.AnimationCompleted = (Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip>)Delegate.Remove(sprite2.AnimationCompleted, new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip>(AnimationCompleteDelegate));
			}
		}

		private void AnimationEventDelegate(tk2dSpriteAnimator sprite, tk2dSpriteAnimationClip clip, int frameNum)
		{
			tk2dSpriteAnimationFrame frame = clip.GetFrame(frameNum);
			Fsm.EventData.IntData = frame.eventInt;
			Fsm.EventData.StringData = frame.eventInfo;
			Fsm.EventData.FloatData = frame.eventFloat;
			base.Fsm.Event(animationTriggerEvent);
		}

		private void AnimationCompleteDelegate(tk2dSpriteAnimator sprite, tk2dSpriteAnimationClip clip)
		{
			int intData = -1;
			tk2dSpriteAnimationClip[] array = ((sprite.Library != null) ? sprite.Library.clips : null);
			if (array != null)
			{
				for (int i = 0; i < array.Length; i++)
				{
					if (array[i] == clip)
					{
						intData = i;
						break;
					}
				}
			}
			Fsm.EventData.IntData = intData;
			base.Fsm.Event(animationCompleteEvent);
		}
	}
}
