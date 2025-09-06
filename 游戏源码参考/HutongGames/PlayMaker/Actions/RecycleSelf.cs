namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Object Pool")]
	[Tooltip("Recycles the Owner of the Fsm. Useful for Object Pool spawned Prefabs that need to kill themselves, e.g., a projectile that explodes on impact.")]
	public class RecycleSelf : FsmStateAction
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
