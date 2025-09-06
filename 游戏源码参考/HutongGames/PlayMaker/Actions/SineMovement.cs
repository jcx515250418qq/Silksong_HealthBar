using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	public class SineMovement : FsmStateAction
	{
		public FsmOwnerDefault Target;

		public FsmVector3 Offset;

		public Space Space;

		public FsmFloat Speed;

		private Transform transform;

		private Vector3 initialPosition;

		public override void Reset()
		{
			Target = null;
			Offset = null;
			Space = Space.Self;
			Speed = null;
		}

		public override void OnEnter()
		{
			GameObject safe = Target.GetSafe(this);
			if ((bool)safe)
			{
				transform = safe.transform;
				if (Space == Space.Self)
				{
					initialPosition = transform.localPosition;
				}
				else
				{
					initialPosition = transform.position;
				}
			}
			if (!transform)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			Vector3 vector = Offset.Value * Mathf.Sin(base.State.StateTime * Speed.Value);
			if (Space == Space.Self)
			{
				transform.localPosition = initialPosition + vector;
			}
			else
			{
				transform.position = initialPosition + vector;
			}
		}
	}
}
