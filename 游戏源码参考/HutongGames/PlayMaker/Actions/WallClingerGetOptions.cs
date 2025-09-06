using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Enemy AI")]
	public class WallClingerGetOptions : FsmStateAction
	{
		[CheckForComponent(typeof(WallClinger))]
		[RequiredField]
		public FsmOwnerDefault Target;

		private WallClinger clinger;

		[UIHint(UIHint.Variable)]
		public FsmFloat MoveSpeed;

		[UIHint(UIHint.Variable)]
		public FsmString ClimbUpAnim;

		[UIHint(UIHint.Variable)]
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
			MoveSpeed.Value = clinger.MoveSpeed;
			ClimbUpAnim.Value = clinger.ClimbUpAnim;
			ClimbDownAnim.Value = clinger.ClimbDownAnim;
			Finish();
		}
	}
}
