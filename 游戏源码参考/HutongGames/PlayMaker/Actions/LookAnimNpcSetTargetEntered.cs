using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	public class LookAnimNpcSetTargetEntered : FSMUtility.GetComponentFsmStateAction<LookAnimNPC>
	{
		public FsmGameObject LookTarget;

		public override void Reset()
		{
			base.Reset();
			LookTarget = null;
		}

		protected override void DoAction(LookAnimNPC lookAnim)
		{
			GameObject value = LookTarget.Value;
			lookAnim.TargetEntered(value);
		}
	}
}
