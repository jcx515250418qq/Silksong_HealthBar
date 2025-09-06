namespace HutongGames.PlayMaker.Actions
{
	public class CheckIfCrestEquipped : FSMUtility.CheckFsmStateAction
	{
		[ObjectType(typeof(ToolCrest))]
		public FsmObject Crest;

		public override bool IsTrue
		{
			get
			{
				ToolCrest toolCrest = Crest.Value as ToolCrest;
				if (toolCrest != null)
				{
					return toolCrest.IsEquipped;
				}
				return false;
			}
		}

		public override void Reset()
		{
			base.Reset();
			Crest = null;
		}
	}
}
