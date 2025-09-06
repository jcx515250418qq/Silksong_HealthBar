namespace HutongGames.PlayMaker.Actions
{
	public class SetDarknessLevel : FsmStateAction
	{
		public FsmInt SetLevel;

		[UIHint(UIHint.Variable)]
		public FsmInt StoreCurrentLevel;

		public override void Reset()
		{
			SetLevel = null;
			StoreCurrentLevel = null;
		}

		public override void OnEnter()
		{
			if (base.Owner.activeInHierarchy)
			{
				StoreCurrentLevel.Value = DarknessRegion.GetDarknessLevel();
				if (!SetLevel.IsNone)
				{
					DarknessRegion.SetDarknessLevel(SetLevel.Value);
				}
				Finish();
			}
		}
	}
}
