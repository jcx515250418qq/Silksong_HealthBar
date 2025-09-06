namespace HutongGames.PlayMaker.Actions
{
	public class ScreenFaderPreset : FsmStateAction
	{
		public enum PresetEvents
		{
			FadeOut = 0,
			FadeIn = 1
		}

		[RequiredField]
		[ObjectType(typeof(PresetEvents))]
		public FsmEnum Preset;

		public override void Reset()
		{
			Preset = null;
		}

		public override void OnEnter()
		{
			GameManager instance = GameManager.instance;
			if (!(instance == null))
			{
				PlayMakerFSM screenFader_fsm = instance.screenFader_fsm;
				switch ((PresetEvents)(object)Preset.Value)
				{
				case PresetEvents.FadeIn:
					screenFader_fsm.SendEvent("SCENE FADE IN");
					break;
				case PresetEvents.FadeOut:
					screenFader_fsm.SendEvent("SCENE FADE OUT");
					break;
				}
				Finish();
			}
		}
	}
}
