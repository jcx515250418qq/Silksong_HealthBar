using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Physics2D)]
	[Tooltip("Gets info on the last joint break 2D event.")]
	public class GetJointBreak2dInfo : FsmStateAction
	{
		[UIHint(UIHint.Variable)]
		[ObjectType(typeof(Joint2D))]
		[Tooltip("Get the broken joint.")]
		public FsmObject brokenJoint;

		[UIHint(UIHint.Variable)]
		[Tooltip("Get the reaction force exerted by the broken joint. Unity 5.3+")]
		public FsmVector2 reactionForce;

		[UIHint(UIHint.Variable)]
		[Tooltip("Get the magnitude of the reaction force exerted by the broken joint. Unity 5.3+")]
		public FsmFloat reactionForceMagnitude;

		[UIHint(UIHint.Variable)]
		[Tooltip("Get the reaction torque exerted by the broken joint. Unity 5.3+")]
		public FsmFloat reactionTorque;

		public override void Reset()
		{
			brokenJoint = null;
			reactionForce = null;
			reactionTorque = null;
		}

		private void StoreInfo()
		{
			if (!(base.Fsm.BrokenJoint2D == null))
			{
				brokenJoint.Value = base.Fsm.BrokenJoint2D;
				reactionForce.Value = base.Fsm.BrokenJoint2D.reactionForce;
				reactionForceMagnitude.Value = base.Fsm.BrokenJoint2D.reactionForce.magnitude;
				reactionTorque.Value = base.Fsm.BrokenJoint2D.reactionTorque;
			}
		}

		public override void OnEnter()
		{
			StoreInfo();
			Finish();
		}
	}
}
