namespace HutongGames.PlayMaker.Actions
{
	public class GetIsCrestUnlocked : FSMUtility.CheckFsmStateAction
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
					return toolCrest.IsUnlocked;
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
