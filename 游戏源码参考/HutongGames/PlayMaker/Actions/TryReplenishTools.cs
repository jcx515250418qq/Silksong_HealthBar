namespace HutongGames.PlayMaker.Actions
{
	public class TryReplenishTools : FSMUtility.CheckFsmStateAction
	{
		public FsmBool DoReplenish;

		[ObjectType(typeof(ToolItemManager.ReplenishMethod))]
		public FsmEnum Method;

		public override bool IsTrue => ToolItemManager.TryReplenishTools(DoReplenish.Value, (ToolItemManager.ReplenishMethod)(object)Method.Value);

		public override void Reset()
		{
			base.Reset();
			DoReplenish = null;
			Method = null;
		}
	}
}
