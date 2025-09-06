namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Audio)]
	[ActionTarget(typeof(MusicCue), "musicCue", false)]
	[Tooltip("Plays music cues.")]
	public class ApplyMusicCue : FsmStateAction
	{
		[Tooltip("Music cue to play.")]
		[ObjectType(typeof(MusicCue))]
		public FsmObject musicCue;

		[Tooltip("Delay before starting transition")]
		public FsmFloat delayTime;

		[Tooltip("Transition duration.")]
		public FsmFloat transitionTime;

		public override void Awake()
		{
			MusicCue musicCue = this.musicCue.Value as MusicCue;
			if ((bool)musicCue)
			{
				musicCue.Preload(base.Owner);
			}
		}

		public override void Reset()
		{
			musicCue = null;
			delayTime = 0f;
			transitionTime = 0f;
		}

		public override void OnEnter()
		{
			MusicCue musicCue = this.musicCue.Value as MusicCue;
			GameManager instance = GameManager.instance;
			if (!(musicCue == null) && !(instance == null))
			{
				instance.AudioManager.ApplyMusicCue(musicCue, delayTime.Value, transitionTime.Value, applySnapshot: false);
			}
			Finish();
		}
	}
}
