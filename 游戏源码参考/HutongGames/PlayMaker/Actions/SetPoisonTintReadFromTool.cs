namespace HutongGames.PlayMaker.Actions
{
	public class SetPoisonTintReadFromTool : FsmStateAction
	{
		[CheckForComponent(typeof(PoisonTintBase))]
		[RequiredField]
		public FsmOwnerDefault Target;

		[RequiredField]
		[ObjectType(typeof(ToolItem))]
		public FsmObject Tool;

		public override void Reset()
		{
			Target = null;
			Tool = null;
		}

		public override void OnEnter()
		{
			Target.GetSafe(this).GetComponent<PoisonTintBase>().ReadFromTool = Tool.Value as ToolItem;
			Finish();
		}
	}
}
