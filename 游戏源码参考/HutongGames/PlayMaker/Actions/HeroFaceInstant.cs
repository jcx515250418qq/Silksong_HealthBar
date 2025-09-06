using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	public class HeroFaceInstant : FsmStateAction
	{
		public FsmOwnerDefault Target;

		private HeroController hc;

		public override void Reset()
		{
			Target = null;
		}

		public override void OnEnter()
		{
			GameObject safe = Target.GetSafe(this);
			if (!safe)
			{
				Finish();
				return;
			}
			hc = HeroController.instance;
			Transform transform = hc.transform;
			if (safe.transform.position.x - transform.position.x > 0f)
			{
				hc.FaceRight();
			}
			else
			{
				hc.FaceLeft();
			}
			Finish();
		}
	}
}
