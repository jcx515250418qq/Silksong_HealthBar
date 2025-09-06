using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	public class TransformVector3 : TransformVector<FsmVector3>
	{
		protected override Vector3 GetInputVector()
		{
			return Vector.Value;
		}

		protected override void SetStoreResult(Vector3 value)
		{
			StoreResult.Value = value;
		}
	}
}
