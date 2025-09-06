using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	public class FollowTargetPositionV2 : FsmStateAction
	{
		public FsmOwnerDefault Target;

		public FsmVector2 FollowPos;

		public FsmFloat LerpSpeed;

		private Transform self;

		public override void Reset()
		{
			Target = null;
			FollowPos = null;
			LerpSpeed = null;
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
			Vector2 a = self.position;
			Vector2 value = FollowPos.Value;
			self.SetPosition2D(Vector2.Lerp(a, value, LerpSpeed.Value * Time.deltaTime));
		}
	}
}
