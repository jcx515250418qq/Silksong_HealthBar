using GlobalEnums;
using TeamCherry.Localization;

namespace HutongGames.PlayMaker.Actions
{
	public class ShowControlReminderSingle : FsmStateAction
	{
		public FsmString AppearEvent;

		public FsmString DisappearEvent;

		public FsmString PlayerDataBool;

		public FsmFloat FadeInDelay;

		public FsmFloat FadeInTime;

		public FsmFloat FadeOutTime;

		public LocalisedFsmString Text;

		public LocalisedFsmString Prompt;

		[ObjectType(typeof(HeroActionButton))]
		public FsmEnum Button;

		public FsmBool DisappearOnButtonPress;

		public override void Reset()
		{
			AppearEvent = null;
			DisappearEvent = null;
			PlayerDataBool = null;
			FadeInDelay = null;
			FadeInTime = 1f;
			FadeOutTime = 0.5f;
			Text = null;
			Prompt = null;
			Button = null;
			DisappearOnButtonPress = null;
		}

		public override void OnEnter()
		{
			ControlReminder.AddReminder(new ControlReminder.SingleConfig
			{
				AppearEvent = AppearEvent.Value,
				DisappearEvent = DisappearEvent.Value,
				PlayerDataBool = PlayerDataBool.Value,
				FadeInDelay = FadeInDelay.Value,
				FadeInTime = FadeInTime.Value,
				FadeOutTime = FadeOutTime.Value,
				Text = Text,
				Prompt = Prompt,
				Button = (HeroActionButton)(object)Button.Value,
				DisappearOnButtonPress = DisappearOnButtonPress.Value
			}, string.IsNullOrEmpty(AppearEvent.Value));
			Finish();
		}
	}
}
