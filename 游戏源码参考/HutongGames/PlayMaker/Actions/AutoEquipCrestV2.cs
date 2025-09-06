namespace HutongGames.PlayMaker.Actions
{
	public class AutoEquipCrestV2 : FsmStateAction
	{
		[ObjectType(typeof(ToolCrest))]
		public FsmObject Crest;

		public FsmBool SkipToAppear;

		public override void Reset()
		{
			Crest = null;
			SkipToAppear = null;
		}

		public override void OnEnter()
		{
			if (SkipToAppear.Value)
			{
				BindOrbHudFrame.SkipToNextAppear = true;
			}
			ToolItemManager.AutoEquip(Crest.Value as ToolCrest, markTemp: false);
			BindOrbHudFrame.SkipToNextAppear = false;
			Finish();
		}
	}
}
