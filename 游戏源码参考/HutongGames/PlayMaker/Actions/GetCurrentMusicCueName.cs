namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Audio)]
	[Tooltip("Get's the name of the currently playing music cue.")]
	public class GetCurrentMusicCueName : FsmStateAction
	{
		public FsmString musicCueName;

		public override void Reset()
		{
			musicCueName = new FsmString
			{
				UseVariable = true
			};
		}

		public override void OnEnter()
		{
			GameManager unsafeInstance = GameManager.UnsafeInstance;
			if (unsafeInstance == null)
			{
				musicCueName.Value = "";
			}
			else
			{
				MusicCue currentMusicCue = unsafeInstance.AudioManager.CurrentMusicCue;
				if (currentMusicCue == null)
				{
					musicCueName.Value = "";
				}
				else
				{
					musicCueName.Value = currentMusicCue.name;
				}
			}
			Finish();
		}
	}
}
