using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Animator)]
	[Tooltip("Interrupts the automatic target matching. CompleteMatch will make the GameObject match the target completely at the next frame.")]
	public class AnimatorInterruptMatchTarget : ComponentAction<Animator>
	{
		[RequiredField]
		[CheckForComponent(typeof(Animator))]
		[Tooltip("The GameObject with an Animator Component.")]
		public FsmOwnerDefault gameObject;

		[Tooltip("Will make the GameObject match the target completely at the next frame")]
		public FsmBool completeMatch;

		public override void Reset()
		{
			gameObject = null;
			completeMatch = true;
		}

		public override void OnEnter()
		{
			if (UpdateCache(base.Fsm.GetOwnerDefaultTarget(gameObject)))
			{
				cachedComponent.InterruptMatchTarget(completeMatch.Value);
			}
			Finish();
		}
	}
}
