using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.GameObject)]
	[Tooltip("Gets the Parent of a Game Object's Parent")]
	public class GetGrandParent : FsmStateAction
	{
		[RequiredField]
		public FsmOwnerDefault gameObject;

		[UIHint(UIHint.Variable)]
		public FsmGameObject storeResult;

		public override void Reset()
		{
			gameObject = null;
			storeResult = null;
		}

		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(gameObject);
			if ((bool)ownerDefaultTarget && (bool)ownerDefaultTarget.transform.parent)
			{
				storeResult.Value = ((ownerDefaultTarget.transform.parent.parent == null) ? null : ownerDefaultTarget.transform.parent.parent.gameObject);
			}
			else
			{
				storeResult.Value = null;
			}
			Finish();
		}
	}
	[ActionCategory(ActionCategory.GameObject)]
	[Tooltip("Gets the parent of a Game Object's parent (the grandparent).")]
	public class GetGrandparent : FsmStateAction
	{
		[RequiredField]
		public FsmOwnerDefault gameObject;

		[UIHint(UIHint.Variable)]
		public FsmGameObject storeResult;

		public override void Reset()
		{
			gameObject = null;
			storeResult = null;
		}

		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(gameObject);
			if ((bool)ownerDefaultTarget && (bool)ownerDefaultTarget.transform.parent && (bool)ownerDefaultTarget.transform.parent.parent)
			{
				storeResult.Value = ownerDefaultTarget.transform.parent.parent.gameObject;
			}
			else
			{
				storeResult.Value = null;
			}
			Finish();
		}
	}
}
