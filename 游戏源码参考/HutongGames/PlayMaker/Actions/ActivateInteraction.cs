namespace HutongGames.PlayMaker.Actions
{
	[Tooltip("Activate / Disable Interaction System")]
	public class ActivateInteraction : FsmStateAction
	{
		[Tooltip("Activate / Disable Interaction System")]
		public FsmBool Activate;

		public override void Reset()
		{
			Activate = null;
		}

		public override void OnEnter()
		{
			InteractManager.IsDisabled = !Activate.Value;
			Finish();
		}
	}
}
