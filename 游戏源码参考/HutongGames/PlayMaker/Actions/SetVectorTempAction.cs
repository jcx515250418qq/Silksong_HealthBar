using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	public abstract class SetVectorTempAction : FsmStateAction
	{
		public FsmOwnerDefault Target;

		public FsmVector3 Vector;

		[HideIf("VectorIsNotNone")]
		public FsmFloat X;

		[HideIf("VectorIsNotNone")]
		public FsmFloat Y;

		[HideIf("VectorIsNotNone")]
		public FsmFloat Z;

		[HideIf("HideSpace")]
		public Space Space;

		public bool VectorIsNotNone()
		{
			return !Vector.IsNone;
		}

		public virtual bool HideSpace()
		{
			return false;
		}

		public override void Reset()
		{
			Target = null;
			Vector = null;
			X = null;
			Y = null;
			Z = null;
			Space = Space.World;
		}

		public override void OnEnter()
		{
			GameObject obj = Target.GetSafe(this);
			if (!obj)
			{
				Finish();
				return;
			}
			Vector3 initialVector = GetVector(obj.transform);
			if (VectorIsNotNone())
			{
				SetVector(obj.transform, Vector.Value);
			}
			else
			{
				Vector3 vector = initialVector;
				if (!X.IsNone)
				{
					vector.x = X.Value;
				}
				if (!Y.IsNone)
				{
					vector.y = Y.Value;
				}
				if (!Z.IsNone)
				{
					vector.z = Z.Value;
				}
				SetVector(obj.transform, vector);
			}
			RecycleResetHandler.Add(obj, (Action)delegate
			{
				SetVector(obj.transform, initialVector);
			});
			Finish();
		}

		protected abstract void SetVector(Transform transform, Vector3 vector);

		protected abstract Vector3 GetVector(Transform transform);
	}
}
