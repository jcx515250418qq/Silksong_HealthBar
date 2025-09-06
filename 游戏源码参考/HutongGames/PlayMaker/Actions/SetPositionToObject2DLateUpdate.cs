using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	public class SetPositionToObject2DLateUpdate : FsmStateAction
	{
		[RequiredField]
		[Tooltip("The GameObject to position.")]
		public FsmOwnerDefault GameObject;

		public FsmGameObject TargetObject;

		public FsmFloat XOffset;

		public FsmFloat YOffset;

		public override void Reset()
		{
			GameObject = null;
			TargetObject = null;
			XOffset = null;
			YOffset = null;
		}

		public override void OnPreprocess()
		{
			base.Fsm.HandleLateUpdate = true;
		}

		public override void OnEnter()
		{
			DoSetPosition();
		}

		public override void OnLateUpdate()
		{
			DoSetPosition();
		}

		private void DoSetPosition()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(GameObject);
			if (!(ownerDefaultTarget == null) && !TargetObject.IsNone && !(TargetObject.Value == null))
			{
				Vector3 vector = TargetObject.Value.transform.position;
				if (!XOffset.IsNone)
				{
					vector = new Vector3(vector.x + XOffset.Value, vector.y, vector.z);
				}
				if (!YOffset.IsNone)
				{
					vector = new Vector3(vector.x, vector.y + YOffset.Value, vector.z);
				}
				vector = new Vector3(vector.x, vector.y, ownerDefaultTarget.transform.position.z);
				ownerDefaultTarget.transform.position = vector;
			}
		}
	}
}
