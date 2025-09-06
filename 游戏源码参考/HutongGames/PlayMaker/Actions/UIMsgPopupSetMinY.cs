namespace HutongGames.PlayMaker.Actions
{
	public sealed class UIMsgPopupSetMinY : FsmStateAction
	{
		public FsmFloat value;

		public override void Reset()
		{
			value = null;
		}

		public override void OnEnter()
		{
			UIMsgPopupBaseBase.MinYPos = value.Value;
			Finish();
		}
	}
}
