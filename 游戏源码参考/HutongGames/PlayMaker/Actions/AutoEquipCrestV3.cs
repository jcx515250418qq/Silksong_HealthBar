namespace HutongGames.PlayMaker.Actions
{
	public class AutoEquipCrestV3 : FsmStateAction
	{
		[ObjectType(typeof(ToolCrest))]
		public FsmObject Crest;

		public FsmBool SkipToAppear;

		public FsmBool IsTemp;

		public override void Reset()
		{
			Crest = null;
			SkipToAppear = null;
			IsTemp = null;
		}

		public override void OnEnter()
		{
			if (SkipToAppear.Value)
			{
				BindOrbHudFrame.SkipToNextAppear = true;
			}
			ToolItemManager.AutoEquip(Crest.Value as ToolCrest, IsTemp.Value);
			BindOrbHudFrame.SkipToNextAppear = false;
			Finish();
		}
	}
}
