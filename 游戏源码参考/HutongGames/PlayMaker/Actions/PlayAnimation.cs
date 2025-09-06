using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Animation)]
	[Tooltip("Plays an Animation on a Game Object. You can add named animation clips to the object in the Unity editor, or with the Add Animation Clip action. NOTE: The game object must have an Animation component.")]
	public class PlayAnimation : BaseAnimationAction
	{
		[RequiredField]
		[CheckForComponent(typeof(Animation))]
		[Tooltip("The Game Object to play the animation on. NOTE: Must have an Animation Component.")]
		public FsmOwnerDefault gameObject;

		[UIHint(UIHint.Animation)]
		[Tooltip("The name of the animation to play. Use the browse button to find animations on the specified Game Object.")]
		public FsmString animName;

		[Tooltip("Whether to stop all currently playing animations, or just the animations on the same layer as this animation.")]
		public PlayMode playMode;

		[HasFloatSlider(0f, 5f)]
		[Tooltip("Time to cross-fade between animations (seconds).")]
		public FsmFloat blendTime;

		[Tooltip("Event to send when the animation is finished playing. NOTE: Not sent with Loop or PingPong wrap modes!")]
		public FsmEvent finishEvent;

		[Tooltip("Event to send when the animation loops. If you want to send this event to another FSM use Set Event Target. NOTE: This event is only sent with Loop and PingPong wrap modes.")]
		public FsmEvent loopEvent;

		[Tooltip("Stop playing the animation when this state is exited.")]
		public bool stopOnExit;

		private AnimationState anim;

		private float prevAnimtTime;

		public override void Reset()
		{
			gameObject = null;
			animName = null;
			playMode = PlayMode.StopAll;
			blendTime = 0.3f;
			finishEvent = null;
			loopEvent = null;
			stopOnExit = false;
		}

		public override void OnEnter()
		{
			DoPlayAnimation();
		}

		private void DoPlayAnimation()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(gameObject);
			if (!UpdateCache(ownerDefaultTarget))
			{
				Finish();
				return;
			}
			if (string.IsNullOrEmpty(animName.Value))
			{
				LogWarning("Missing animName!");
				Finish();
				return;
			}
			anim = base.animation[animName.Value];
			if (anim == null)
			{
				LogWarning("Missing animation: " + animName.Value);
				Finish();
				return;
			}
			float value = blendTime.Value;
			if (value < 0.001f)
			{
				base.animation.Play(animName.Value, playMode);
			}
			else
			{
				base.animation.CrossFade(animName.Value, value, playMode);
			}
			prevAnimtTime = anim.time;
		}

		public override void OnUpdate()
		{
			if (!(base.Fsm.GetOwnerDefaultTarget(gameObject) == null) && !(anim == null))
			{
				if (!anim.enabled || (anim.wrapMode == WrapMode.ClampForever && anim.time > anim.length))
				{
					base.Fsm.Event(finishEvent);
					Finish();
				}
				if (anim.wrapMode != WrapMode.ClampForever && anim.time > anim.length && prevAnimtTime < anim.length)
				{
					base.Fsm.Event(loopEvent);
				}
			}
		}

		public override void OnExit()
		{
			if (stopOnExit)
			{
				StopAnimation();
			}
		}

		private void StopAnimation()
		{
			if (base.animation != null)
			{
				base.animation.Stop(animName.Value);
			}
		}
	}
}
