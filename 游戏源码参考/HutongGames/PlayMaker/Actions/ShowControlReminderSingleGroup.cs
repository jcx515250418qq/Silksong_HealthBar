using System;
using GlobalEnums;
using TeamCherry.Localization;

namespace HutongGames.PlayMaker.Actions
{
	public class ShowControlReminderSingleGroup : FsmStateAction
	{
		[Serializable]
		public class Group
		{
			public LocalisedFsmString Text;

			public LocalisedFsmString Prompt;

			[ObjectType(typeof(HeroActionButton))]
			public FsmEnum Button;
		}

		public FsmString DisappearEvent;

		public FsmString PlayerDataBool;

		public FsmFloat FadeInDelay;

		public FsmFloat FadeInTime;

		public FsmFloat FadeOutTime;

		public Group[] Groups;

		public FsmBool DisappearOnButtonPress;

		public override void Reset()
		{
			DisappearEvent = null;
			PlayerDataBool = null;
			FadeInDelay = null;
			FadeInTime = 1f;
			FadeOutTime = 0.5f;
			Groups = null;
			DisappearOnButtonPress = null;
		}

		public override void OnEnter()
		{
			Group[] groups = Groups;
			foreach (Group group in groups)
			{
				ControlReminder.PushSingle(new ControlReminder.SingleConfig
				{
					DisappearEvent = DisappearEvent.Value,
					PlayerDataBool = PlayerDataBool.Value,
					FadeInDelay = FadeInDelay.Value,
					FadeInTime = FadeInTime.Value,
					FadeOutTime = FadeOutTime.Value,
					Text = group.Text,
					Prompt = group.Prompt,
					Button = (HeroActionButton)(object)group.Button.Value,
					DisappearOnButtonPress = DisappearOnButtonPress.Value
				});
			}
			ControlReminder.ShowPushed();
			Finish();
		}
	}
}
