using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	public class FollowTargetPosition : FsmStateAction
	{
		public FsmOwnerDefault Target;

		public FsmVector2 FollowPos;

		public FsmFloat LerpFactor;

		private Transform self;

		public override void Reset()
		{
			Target = null;
			FollowPos = null;
			LerpFactor = null;
		}

		public override void OnEnter()
		{
			GameObject safe = Target.GetSafe(this);
			if ((bool)safe)
			{
				self = safe.transform;
				if ((bool)self)
				{
					return;
				}
			}
			Finish();
		}

		public override void OnUpdate()
		{
			_ = LerpFactor.Value;
			Vector2 a = self.position;
			Vector2 value = FollowPos.Value;
			self.SetPosition2D(Vector2.Lerp(a, value, LerpFactor.Value));
		}
	}
}
