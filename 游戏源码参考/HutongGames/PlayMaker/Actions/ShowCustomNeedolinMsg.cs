using TeamCherry.Localization;

namespace HutongGames.PlayMaker.Actions
{
	public class ShowCustomNeedolinMsg : FsmStateAction
	{
		public class TextWrapper : ILocalisedTextCollection
		{
			public LocalisedString Text;

			public bool IsActive => true;

			public LocalisedString GetRandom(LocalisedString skipString)
			{
				return Text;
			}
		}

		public LocalisedFsmString Text;

		public FsmFloat Timer;

		private TextWrapper textWrapper;

		public override void Reset()
		{
			Text = null;
			Timer = new FsmFloat
			{
				UseVariable = true
			};
		}

		public override void OnEnter()
		{
			textWrapper = new TextWrapper
			{
				Text = Text
			};
			NeedolinMsgBox.AddText(textWrapper, skipStartDelay: true, maxPriority: true);
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
