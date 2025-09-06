namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Audio)]
	[Tooltip("Get's the currently playing music cue.")]
	public class GetCurrentMusicCue : FsmStateAction
	{
		[ObjectType(typeof(MusicCue))]
		public FsmObject musicCue;

		public override void Reset()
		{
			musicCue = new FsmObject
			{
				UseVariable = true
			};
		}

		public override void OnEnter()
		{
			GameManager unsafeInstance = GameManager.UnsafeInstance;
			if (unsafeInstance == null)
			{
				musicCue.Value = null;
			}
			else
			{
				musicCue.Value = unsafeInstance.AudioManager.CurrentMusicCue;
			}
			Finish();
		}
	}
}
