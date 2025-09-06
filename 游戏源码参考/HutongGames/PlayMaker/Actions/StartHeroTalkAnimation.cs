namespace HutongGames.PlayMaker.Actions
{
	public sealed class StartHeroTalkAnimation : FsmStateAction
	{
		[RequiredField]
		[CheckForComponent(typeof(NPCControlBase))]
		public FsmOwnerDefault Target;

		public override void Reset()
		{
			Target = null;
		}

		public override void OnEnter()
		{
			NPCControlBase safe = Target.GetSafe<NPCControlBase>(this);
			if (safe != null)
			{
				safe.BeginHeroTalkAnimation();
			}
			Finish();
		}
	}
}
