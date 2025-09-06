namespace HutongGames.PlayMaker.Actions
{
	public class CheckIfToolUnlocked : FSMUtility.CheckFsmStateAction
	{
		[ObjectType(typeof(ToolItem))]
		public FsmObject Tool;

		public override bool IsTrue
		{
			get
			{
				if (Tool.IsNone)
				{
					return false;
				}
				return ((ToolItem)Tool.Value).IsUnlocked;
			}
		}

		public override void Reset()
		{
			base.Reset();
			Tool = null;
		}
	}
}
