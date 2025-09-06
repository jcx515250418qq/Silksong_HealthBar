namespace HutongGames.PlayMaker.Actions
{
	[Tooltip("Needed to make an action to wrap this function because Playmaker CallMethod actions don't run on ScriptableObjects.")]
	public class UnlockCrest : FsmStateAction
	{
		[ObjectType(typeof(ToolCrest))]
		public FsmObject Crest;

		public override void Reset()
		{
			base.Reset();
			Crest = null;
		}

		public override void OnEnter()
		{
			ToolCrest toolCrest = Crest.Value as ToolCrest;
			if (toolCrest != null)
			{
				toolCrest.Unlock();
			}
			Finish();
		}
	}
}
