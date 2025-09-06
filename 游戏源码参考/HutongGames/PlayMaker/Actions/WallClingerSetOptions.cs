using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Enemy AI")]
	public class WallClingerSetOptions : FsmStateAction
	{
		[CheckForComponent(typeof(WallClinger))]
		[RequiredField]
		public FsmOwnerDefault Target;

		private WallClinger clinger;

		public FsmFloat MoveSpeed;

		public FsmString ClimbUpAnim;

		public FsmString ClimbDownAnim;

		public override void Reset()
		{
			Target = null;
			clinger = null;
			MoveSpeed = null;
			ClimbUpAnim = null;
			ClimbDownAnim = null;
		}

		public override void OnEnter()
		{
			GameObject safe = Target.GetSafe(this);
			if ((bool)safe)
			{
				clinger = safe.GetComponent<WallClinger>();
			}
			if (!clinger)
			{
				Finish();
				return;
			}
			if (!MoveSpeed.IsNone)
			{
				clinger.MoveSpeed = MoveSpeed.Value;
			}
			if (!ClimbUpAnim.IsNone)
			{
				clinger.ClimbUpAnim = ClimbUpAnim.Value;
			}
			if (!ClimbDownAnim.IsNone)
			{
				clinger.ClimbDownAnim = ClimbDownAnim.Value;
			}
			Finish();
		}
	}
}
