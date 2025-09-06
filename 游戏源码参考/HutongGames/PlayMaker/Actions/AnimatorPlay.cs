using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Animator)]
	[Tooltip("Plays a state. This could be used to synchronize your animation with audio or synchronize an Animator over the network.")]
	public class AnimatorPlay : ComponentAction<Animator>
	{
		[RequiredField]
		[CheckForComponent(typeof(Animator))]
		[Tooltip("The GameObject with an Animator Component.")]
		public FsmOwnerDefault gameObject;

		[Tooltip("The name of the state that will be played.")]
		public FsmString stateName;

		[Tooltip("The layer where the state is.")]
		public FsmInt layer;

		[Tooltip("The normalized time at which the state will play")]
		public FsmFloat normalizedTime;

		[Tooltip("Repeat every frame. Useful when using normalizedTime to manually control the animation.")]
		public bool everyFrame;

		private Animator animator => cachedComponent;

		public override void Reset()
		{
			gameObject = null;
			stateName = null;
			layer = new FsmInt
			{
				UseVariable = true
			};
			normalizedTime = new FsmFloat
			{
				UseVariable = true
			};
			everyFrame = false;
		}

		public override void OnEnter()
		{
			DoAnimatorPlay();
			if (!everyFrame)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			DoAnimatorPlay();
		}

		private void DoAnimatorPlay()
		{
			if (!UpdateCache(base.Fsm.GetOwnerDefaultTarget(gameObject)))
			{
				Finish();
				return;
			}
			int num = (layer.IsNone ? (-1) : layer.Value);
			float num2 = (normalizedTime.IsNone ? float.NegativeInfinity : normalizedTime.Value);
			animator.Play(stateName.Value, num, num2);
		}
	}
}
