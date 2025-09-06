using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	public class SetRotation2DLerp : FsmStateAction
	{
		public FsmOwnerDefault Target;

		public FsmFloat TargetRotationZ;

		public FsmFloat LerpSpeed;

		public Space Space;

		private Transform transform;

		public override void Reset()
		{
			Target = null;
			TargetRotationZ = null;
			LerpSpeed = null;
			Space = Space.Self;
		}

		public override void OnEnter()
		{
			GameObject safe = Target.GetSafe(this);
			if (!safe)
			{
				Finish();
			}
			else
			{
				transform = safe.transform;
			}
		}

		public override void OnUpdate()
		{
			Quaternion a = Space switch
			{
				Space.World => transform.rotation, 
				Space.Self => transform.localRotation, 
				_ => throw new ArgumentOutOfRangeException(), 
			};
			Quaternion b = Quaternion.Euler(0f, 0f, TargetRotationZ.Value);
			a = Quaternion.Lerp(a, b, LerpSpeed.Value * Time.deltaTime);
			switch (Space)
			{
			case Space.World:
				transform.rotation = a;
				break;
			case Space.Self:
				transform.localRotation = a;
				break;
			default:
				throw new ArgumentOutOfRangeException();
			}
		}
	}
}
