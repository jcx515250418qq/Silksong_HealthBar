using GlobalEnums;

namespace HutongGames.PlayMaker.Actions
{
	public class SetInputHandlerSkipMode : FsmStateAction
	{
		[ObjectType(typeof(SkipPromptMode))]
		public FsmEnum SkipMode;

		public override void Reset()
		{
			SkipMode = null;
		}

		public override void OnEnter()
		{
			if (!SkipMode.IsNone)
			{
				SkipPromptMode skipMode = (SkipPromptMode)(object)SkipMode.Value;
				ManagerSingleton<InputHandler>.Instance.SetSkipMode(skipMode);
			}
			Finish();
		}
	}
}
