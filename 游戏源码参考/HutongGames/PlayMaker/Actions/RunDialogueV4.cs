using TeamCherry.Localization;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Dialogue")]
	public class RunDialogueV4 : RunDialogueBase
	{
		public FsmString CustomText;

		[HideIf("UsesCustomText")]
		public FsmString Sheet;

		[HideIf("UsesCustomText")]
		public FsmString Key;

		public FsmColor TextColor;

		[ObjectType(typeof(RandomAudioClipTable))]
		public FsmObject NpcVoiceTableOverride;

		private NPCSpeakingAudio speakingAudio;

		private RandomAudioClipTable previousTable;

		protected override string DialogueText
		{
			get
			{
				if (UsesCustomText())
				{
					return CustomText.Value;
				}
				if (!CheatManager.IsDialogueDebugEnabled)
				{
					return new LocalisedString(Sheet.Value, Key.Value).ToString(allowBlankText: false);
				}
				return Sheet.Value + " / " + Key.Value;
			}
		}

		public bool UsesCustomText()
		{
			return !CustomText.IsNone;
		}

		public override void Reset()
		{
			base.Reset();
			CustomText = new FsmString
			{
				UseVariable = true
			};
			Sheet = null;
			Key = null;
			TextColor = new FsmColor
			{
				UseVariable = true
			};
			NpcVoiceTableOverride = null;
		}

		protected override DialogueBox.DisplayOptions GetDisplayOptions(DialogueBox.DisplayOptions defaultOptions)
		{
			if (!TextColor.IsNone)
			{
				defaultOptions.TextColor = TextColor.Value;
			}
			return defaultOptions;
		}

		protected override void StartDialogue(PlayMakerNPC component)
		{
			RandomAudioClipTable randomAudioClipTable = NpcVoiceTableOverride.Value as RandomAudioClipTable;
			if (!NpcVoiceTableOverride.IsNone && (bool)randomAudioClipTable)
			{
				speakingAudio = component.GetComponent<NPCSpeakingAudio>();
				if (!speakingAudio)
				{
					speakingAudio = component.gameObject.AddComponent<NPCSpeakingAudio>();
				}
				if ((bool)speakingAudio)
				{
					previousTable = speakingAudio.Table;
					speakingAudio.Table = randomAudioClipTable;
				}
			}
			base.StartDialogue(component);
		}

		public override void OnExit()
		{
			base.OnExit();
			if ((bool)speakingAudio)
			{
				speakingAudio.Table = previousTable;
				speakingAudio = null;
				previousTable = null;
			}
		}
	}
}
