namespace HutongGames.PlayMaker.Actions
{
	public class SetTalkTableOverride : FsmStateAction
	{
		[RequiredField]
		[CheckForComponent(typeof(PlayMakerNPC))]
		public FsmOwnerDefault Target;

		[ObjectType(typeof(RandomAudioClipTable))]
		public FsmObject PlayerVoiceTableOverride;

		public override void Reset()
		{
			Target = null;
			PlayerVoiceTableOverride = null;
		}

		public override void OnEnter()
		{
			PlayMakerNPC safe = Target.GetSafe<PlayMakerNPC>(this);
			if (safe != null)
			{
				RandomAudioClipTable randomAudioClipTable = PlayerVoiceTableOverride.Value as RandomAudioClipTable;
				if (randomAudioClipTable != null)
				{
					safe.SetTalkTableOverride(randomAudioClipTable);
				}
				else
				{
					safe.RemoveTalkTableOverride();
				}
			}
			Finish();
		}
	}
}
