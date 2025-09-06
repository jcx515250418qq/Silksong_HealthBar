using TeamCherry.Localization;

namespace HutongGames.PlayMaker.Actions
{
	public class ShowCustomNeedolinMsgFromTemplate : FsmStateAction
	{
		public class TextWrapper : ILocalisedTextCollection
		{
			private readonly LocalisedTextCollectionData data;

			public bool IsActive => data.IsActive;

			public TextWrapper(LocalisedString template)
			{
				data = new LocalisedTextCollectionData(template);
			}

			public LocalisedString GetRandom(LocalisedString skipString)
			{
				return data.GetRandom(skipString);
			}
		}

		public LocalisedFsmString Template;

		public FsmFloat Timer;

		private TextWrapper textWrapper;

		public override void Reset()
		{
			Template = null;
			Timer = new FsmFloat
			{
				UseVariable = true
			};
		}

		public override void OnEnter()
		{
			textWrapper = new TextWrapper(Template);
			if (Template.Key.Value == "")
			{
				Finish();
			}
			else
			{
				NeedolinMsgBox.AddText(textWrapper, skipStartDelay: true, maxPriority: true);
			}
		}

		public override void OnUpdate()
		{
			if (Timer.Value > 0f && base.State.StateTime >= Timer.Value)
			{
				End();
			}
		}

		public override void OnExit()
		{
			End();
		}

		private void End()
		{
			if (textWrapper != null)
			{
				NeedolinMsgBox.RemoveText(textWrapper);
				Finish();
			}
		}
	}
}
