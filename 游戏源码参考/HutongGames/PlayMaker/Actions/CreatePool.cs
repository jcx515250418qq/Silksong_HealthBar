namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Object Pool")]
	[Tooltip("Creates an Object Pool")]
	public class CreatePool : FsmStateAction
	{
		public override void OnEnter()
		{
			if (base.Owner != null)
			{
				base.Owner.Recycle();
			}
			Finish();
		}
	}
}
