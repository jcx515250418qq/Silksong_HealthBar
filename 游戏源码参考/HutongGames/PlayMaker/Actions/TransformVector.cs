using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	public abstract class TransformVector<T> : FsmStateAction where T : NamedVariable
	{
		public enum Transformations
		{
			TransformPoint = 0,
			TransformDirection = 1,
			TransformVector = 2,
			InverseTransformPoint = 3,
			InverseTransformDirection = 4,
			InverseTransformVector = 5
		}

		public FsmOwnerDefault Transform;

		public Transformations Transformation;

		public T Vector;

		[UIHint(UIHint.Variable)]
		public T StoreResult;

		public bool EveryFrame;

		public override void Reset()
		{
			Transform = null;
			Transformation = Transformations.TransformPoint;
			Vector = null;
			StoreResult = null;
			EveryFrame = false;
		}

		public override void OnEnter()
		{
			DoAction();
			if (!EveryFrame)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			DoAction();
		}

		private void DoAction()
		{
			GameObject safe = Transform.GetSafe(this);
			if ((bool)safe)
			{
				Func<Vector3, Vector3> func = null;
				switch (Transformation)
				{
				case Transformations.TransformPoint:
					func = safe.transform.TransformPoint;
					break;
				case Transformations.TransformDirection:
					func = safe.transform.TransformDirection;
					break;
				case Transformations.TransformVector:
					func = safe.transform.TransformVector;
					break;
				case Transformations.InverseTransformPoint:
					func = safe.transform.InverseTransformPoint;
					break;
				case Transformations.InverseTransformDirection:
					func = safe.transform.InverseTransformDirection;
					break;
				case Transformations.InverseTransformVector:
					func = safe.transform.InverseTransformVector;
					break;
				}
				if (func != null)
				{
					SetStoreResult(func(GetInputVector()));
				}
			}
		}

		protected abstract void SetStoreResult(Vector3 value);

		protected abstract Vector3 GetInputVector();
	}
}
