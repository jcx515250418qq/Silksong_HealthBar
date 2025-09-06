using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Physics2D)]
	public class MultiplyGravity : ComponentAction<Rigidbody2D>
	{
		[RequiredField]
		[CheckForComponent(typeof(Rigidbody2D))]
		[Tooltip("The GameObject with a Rigidbody 2d attached")]
		public FsmOwnerDefault gameObject;

		[RequiredField]
		[Tooltip("The gravity scale effect")]
		public FsmFloat multiplyBy;

		public override void Reset()
		{
			gameObject = null;
			multiplyBy = 1f;
		}

		public override void OnEnter()
		{
			DoSetGravityScale();
			Finish();
		}

		private void DoSetGravityScale()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(gameObject);
			if (UpdateCache(ownerDefaultTarget))
			{
				base.rigidbody2d.gravityScale *= multiplyBy.Value;
			}
		}
	}
}
