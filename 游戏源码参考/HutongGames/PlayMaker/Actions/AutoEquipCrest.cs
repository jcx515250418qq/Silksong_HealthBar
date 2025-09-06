namespace HutongGames.PlayMaker.Actions
{
	public class AutoEquipCrest : FsmStateAction
	{
		[ObjectType(typeof(ToolCrest))]
		public FsmObject Crest;

		public override void Reset()
		{
			Crest = null;
		}

		public override void OnEnter()
		{
			ToolItemManager.AutoEquip(Crest.Value as ToolCrest, markTemp: false);
			Finish();
		}
	}
}
