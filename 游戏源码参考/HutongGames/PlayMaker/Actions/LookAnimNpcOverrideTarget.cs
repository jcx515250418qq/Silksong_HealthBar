using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	public class LookAnimNpcOverrideTarget : FSMUtility.GetComponentFsmStateAction<LookAnimNPC>
	{
		public FsmGameObject OverrideTarget;

		public override void Reset()
		{
			base.Reset();
			OverrideTarget = null;
		}

		protected override void DoAction(LookAnimNPC lookAnim)
		{
			GameObject value = OverrideTarget.Value;
			lookAnim.TargetOverride = (value ? value.transform : null);
		}
	}
}
