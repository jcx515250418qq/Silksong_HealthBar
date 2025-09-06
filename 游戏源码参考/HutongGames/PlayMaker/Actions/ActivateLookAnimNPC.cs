namespace HutongGames.PlayMaker.Actions
{
	public class ActivateLookAnimNPC : FSMUtility.GetComponentFsmStateAction<LookAnimNPC>
	{
		public FsmBool Activate;

		[HideIf("HideFacing")]
		public FsmBool IsFacingLeft;

		public bool HideFacing()
		{
			return !Activate.Value;
		}

		public override void Reset()
		{
			base.Reset();
			Activate = null;
			IsFacingLeft = new FsmBool
			{
				UseVariable = true
			};
		}

		protected override void DoAction(LookAnimNPC lookAnim)
		{
			if (Activate.Value)
			{
				if (IsFacingLeft.IsNone)
				{
					lookAnim.Activate();
				}
				else
				{
					lookAnim.Activate(IsFacingLeft.Value);
				}
			}
			else
			{
				lookAnim.Deactivate();
			}
		}
	}
}
