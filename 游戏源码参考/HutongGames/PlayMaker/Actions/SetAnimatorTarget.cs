using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Animator)]
	[Tooltip("Sets an AvatarTarget and a targetNormalizedTime for the current state")]
	public class SetAnimatorTarget : ComponentAction<Animator>
	{
		[RequiredField]
		[CheckForComponent(typeof(Animator))]
		[Tooltip("The GameObject with the Animator Component.")]
		public FsmOwnerDefault gameObject;

		[Tooltip("The avatar target")]
		public AvatarTarget avatarTarget;

		[Tooltip("The current state Time that is queried")]
		public FsmFloat targetNormalizedTime;

		[Tooltip("Repeat every frame during OnAnimatorMove. Useful when changing over time.")]
		public bool everyFrame;

		public override void Reset()
		{
			gameObject = null;
			avatarTarget = AvatarTarget.Body;
			targetNormalizedTime = null;
			everyFrame = false;
		}

		public override void OnPreprocess()
		{
			base.Fsm.HandleAnimatorMove = true;
		}

		public override void OnEnter()
		{
			SetTarget();
			if (!everyFrame)
			{
				Finish();
			}
		}

		public override void DoAnimatorMove()
		{
			SetTarget();
		}

		private void SetTarget()
		{
			if (UpdateCache(base.Fsm.GetOwnerDefaultTarget(gameObject)))
			{
				cachedComponent.SetTarget(avatarTarget, targetNormalizedTime.Value);
			}
		}
	}
}
