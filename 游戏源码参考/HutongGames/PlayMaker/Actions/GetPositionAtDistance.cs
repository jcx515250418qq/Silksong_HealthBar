using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	public class GetPositionAtDistance : FsmStateAction
	{
		public FsmOwnerDefault Target;

		public FsmVector3 PositionFrom;

		public FsmFloat Distance;

		public override void Reset()
		{
			Target = null;
			PositionFrom = null;
			Distance = null;
		}

		public override void OnEnter()
		{
			GameObject safe = Target.GetSafe(this);
			if (!safe)
			{
				Finish();
				return;
			}
			Vector3 position = safe.transform.position;
			Vector2 vector = PositionFrom.Value;
			Vector2 vector2 = (Vector2)position - vector;
			if (vector2.magnitude <= Distance.Value)
			{
				Finish();
				return;
			}
			Vector2 vector3 = vector2.normalized * Distance.Value;
			safe.transform.SetPosition2D(vector + vector3);
		}
	}
}
