using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Animation)]
	[Tooltip("Plays a Random Animation on a Game Object. You can set the relative weight of each animation to control how often they are selected.")]
	public class PlayRandomAnimation : BaseAnimationAction
	{
		[RequiredField]
		[CheckForComponent(typeof(Animation))]
		[Tooltip("Game Object to play the animation on.")]
		public FsmOwnerDefault gameObject;

		[CompoundArray("Animations", "Animation", "Weight")]
		[UIHint(UIHint.Animation)]
		[Tooltip("An array of animations to pick randomly from.")]
		public FsmString[] animations;

		[HasFloatSlider(0f, 1f)]
		[Tooltip("The relative probability of each animation being picked. E.g. a weight of 2 is twice a likely to be picked as a weight of 1.")]
		public FsmFloat[] weights;

		[Tooltip("How to treat previously playing animations.")]
		public PlayMode playMode;

		[HasFloatSlider(0f, 5f)]
		[Tooltip("Time taken to blend to this animation.")]
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
			animations = new FsmString[0];
			weights = new FsmFloat[0];
			playMode = PlayMode.StopAll;
			blendTime = 0.3f;
			finishEvent = null;
			loopEvent = null;
			stopOnExit = false;
		}

		public override void OnEnter()
		{
			DoPlayRandomAnimation();
		}

		private void DoPlayRandomAnimation()
		{
			if (animations.Length != 0)
			{
				int randomWeightedIndex = ActionHelpers.GetRandomWeightedIndex(weights);
				if (randomWeightedIndex != -1)
				{
					DoPlayAnimation(animations[randomWeightedIndex].Value);
				}
			}
		}

		private void DoPlayAnimation(string animName)
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(gameObject);
			if (!UpdateCache(ownerDefaultTarget))
			{
				Finish();
				return;
			}
			if (string.IsNullOrEmpty(animName))
			{
				LogWarning("Missing animName!");
				Finish();
				return;
			}
			anim = base.animation[animName];
			if (anim == null)
			{
				LogWarning("Missing animation: " + animName);
				Finish();
				return;
			}
			float value = blendTime.Value;
			if (value < 0.001f)
			{
				base.animation.Play(animName, playMode);
			}
			else
			{
				base.animation.CrossFade(animName, value, playMode);
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
				base.animation.Stop(anim.name);
			}
		}
	}
}
