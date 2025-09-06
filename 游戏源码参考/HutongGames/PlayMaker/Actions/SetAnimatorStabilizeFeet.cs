using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Animator)]
	[Tooltip("If true, automatically stabilize feet during transition and blending")]
	public class SetAnimatorStabilizeFeet : ComponentAction<Animator>
	{
		[RequiredField]
		[CheckForComponent(typeof(Animator))]
		[Tooltip("The GameObject with an Animator Component.")]
		public FsmOwnerDefault gameObject;

		[Tooltip("If true, automatically stabilize feet during transition and blending")]
		public FsmBool stabilizeFeet;

		public override void Reset()
		{
			gameObject = null;
			stabilizeFeet = null;
		}

		public override void OnEnter()
		{
			if (UpdateCache(base.Fsm.GetOwnerDefaultTarget(gameObject)))
			{
				cachedComponent.stabilizeFeet = stabilizeFeet.Value;
			}
			Finish();
		}
	}
}
