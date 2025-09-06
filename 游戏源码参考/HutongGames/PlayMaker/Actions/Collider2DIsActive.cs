using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	public class Collider2DIsActive : FSMUtility.CheckFsmStateEveryFrameAction
	{
		public FsmOwnerDefault Target;

		public override bool IsTrue
		{
			get
			{
				GameObject safe = Target.GetSafe(this);
				if (!safe)
				{
					return false;
				}
				Collider2D component = safe.GetComponent<Collider2D>();
				if ((bool)component)
				{
					return component.isActiveAndEnabled;
				}
				return false;
			}
		}

		public override void Reset()
		{
			base.Reset();
			Target = null;
		}
	}
}
