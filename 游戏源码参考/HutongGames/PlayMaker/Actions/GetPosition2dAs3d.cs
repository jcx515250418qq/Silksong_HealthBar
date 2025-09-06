using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	public class GetPosition2dAs3d : FsmStateAction
	{
		public FsmOwnerDefault SourceXY;

		public FsmOwnerDefault SourceZ;

		[UIHint(UIHint.Variable)]
		public FsmVector3 StoreVector;

		public override void Reset()
		{
			SourceXY = null;
			SourceZ = null;
			StoreVector = null;
		}

		public override void OnEnter()
		{
			GameObject safe = SourceXY.GetSafe(this);
			GameObject safe2 = SourceZ.GetSafe(this);
			Vector2 vector = (safe ? ((Vector2)safe.transform.position) : Vector2.zero);
			float z = (safe2 ? safe2.transform.position.z : 0f);
			StoreVector.Value = new Vector3(vector.x, vector.y, z);
			Finish();
		}
	}
}
