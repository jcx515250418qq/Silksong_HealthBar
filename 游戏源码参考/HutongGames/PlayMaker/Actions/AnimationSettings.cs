using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Animation)]
	[Tooltip("Applies animation settings to the specified animation. Note: Settings are applied once, on entering the state, not continuously. Use\u00a0{{Set Animation Speed}},\u00a0{{Set Animation Time}}\u00a0etc. if you need to update those animation\u00a0settings every frame.\\nSee\u00a0<a href=\"https://docs.unity3d.com/Manual/AnimationScripting.html\" rel =\"nofollow\" target=\"_blank\">Unity Animation Docs</a>\u00a0for detailed descriptions of Wrap Mode, Blend Mode, Speed and Layer settings.")]
	public class AnimationSettings : BaseAnimationAction
	{
		[RequiredField]
		[CheckForComponent(typeof(Animation))]
		[Tooltip("A GameObject with an Animation Component.")]
		public FsmOwnerDefault gameObject;

		[RequiredField]
		[UIHint(UIHint.Animation)]
		[Tooltip("The name of the animation. Use the browse button to select from animations on the Game Object (if available).")]
		public FsmString animName;

		[Tooltip("Set how the animation wraps (Loop, PingPong etc.). NOTE: Because of the way WrapMode is defined by Unity you cannot select Once, but Clamp is the same as Once.")]
		public WrapMode wrapMode;

		[Tooltip("How the animation is blended with other animations on the Game Object.")]
		public AnimationBlendMode blendMode;

		[HasFloatSlider(0f, 5f)]
		[Tooltip("Speed up or slow down the animation. 1 is normal speed, 0.5 is half speed...")]
		public FsmFloat speed;

		[Tooltip("You can play animations on different layers to combine them into a final animation. See the Unity Animation docs for more details.")]
		public FsmInt layer;

		public override void Reset()
		{
			gameObject = null;
			animName = null;
			wrapMode = WrapMode.Loop;
			blendMode = AnimationBlendMode.Blend;
			speed = 1f;
			layer = 0;
		}

		public override void OnEnter()
		{
			DoAnimationSettings();
			Finish();
		}

		private void DoAnimationSettings()
		{
			if (string.IsNullOrEmpty(animName.Value))
			{
				return;
			}
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(gameObject);
			if (!UpdateCache(ownerDefaultTarget))
			{
				return;
			}
			AnimationState animationState = base.animation[animName.Value];
			if (animationState == null)
			{
				LogWarning("Missing animation: " + animName.Value);
				return;
			}
			animationState.wrapMode = wrapMode;
			animationState.blendMode = blendMode;
			if (!layer.IsNone)
			{
				animationState.layer = layer.Value;
			}
			if (!speed.IsNone)
			{
				animationState.speed = speed.Value;
			}
		}
	}
}
