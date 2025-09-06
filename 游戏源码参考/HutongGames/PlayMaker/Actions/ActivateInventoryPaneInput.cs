namespace HutongGames.PlayMaker.Actions
{
	public class ActivateInventoryPaneInput : FsmStateAction
	{
		public FsmOwnerDefault Target;

		public FsmBool Activate;

		public override void Reset()
		{
			Target = null;
			Activate = null;
		}

		public override void OnEnter()
		{
			Target.GetSafe(this).GetComponent<InventoryPaneInput>().enabled = Activate.Value;
			Finish();
		}
	}
}
