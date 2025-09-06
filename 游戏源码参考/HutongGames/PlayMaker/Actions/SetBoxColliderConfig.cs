namespace HutongGames.PlayMaker.Actions
{
	public class SetBoxColliderConfig : FSMUtility.GetComponentFsmStateAction<BoxCollider2DConfigs>
	{
		[RequiredField]
		public FsmInt Index;

		public override void Reset()
		{
			base.Reset();
			Index = null;
		}

		protected override void DoAction(BoxCollider2DConfigs component)
		{
			component.SetConfig(Index.Value);
		}
	}
}
