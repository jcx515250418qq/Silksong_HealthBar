using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Physics 2d")]
	public class SetAngularDrag2d : RigidBody2dActionBase
	{
		[RequiredField]
		[CheckForComponent(typeof(Rigidbody2D))]
		public FsmOwnerDefault gameObject;

		public FsmFloat angularDrag;

		public bool everyFrame;

		public override void Reset()
		{
			angularDrag = null;
			everyFrame = false;
		}

		public override void Awake()
		{
			base.Fsm.HandleFixedUpdate = true;
		}

		public override void OnPreprocess()
		{
			base.Fsm.HandleFixedUpdate = true;
		}

		public override void OnEnter()
		{
			CacheRigidBody2d(base.Fsm.GetOwnerDefaultTarget(gameObject));
			DoSetDrag();
			if (!everyFrame)
			{
				Finish();
			}
		}

		public override void OnFixedUpdate()
		{
			DoSetDrag();
			if (!everyFrame)
			{
				Finish();
			}
		}

		private void DoSetDrag()
		{
			if (!(rb2d == null) && !angularDrag.IsNone)
			{
				rb2d.angularDamping = angularDrag.Value;
			}
		}
	}
}
