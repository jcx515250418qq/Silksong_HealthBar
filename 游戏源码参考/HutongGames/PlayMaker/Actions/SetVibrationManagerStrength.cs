namespace HutongGames.PlayMaker.Actions
{
	public sealed class SetVibrationManagerStrength : FsmStateAction
	{
		public FsmFloat strength;

		public FsmFloat fadeDuration;

		public FsmBool resetOnExit;

		[Tooltip("Will restore original strength if not set")]
		public FsmFloat strengthOnExit;

		public FsmFloat restoreFadeDuration;

		public FsmBool blockNextSceneAutoFade;

		private float originalStrength;

		public override void Reset()
		{
			strength = null;
			fadeDuration = null;
			resetOnExit = null;
			strengthOnExit = new FsmFloat
			{
				UseVariable = true
			};
			restoreFadeDuration = new FsmFloat
			{
				UseVariable = true
			};
			blockNextSceneAutoFade = null;
		}

		public override void OnEnter()
		{
			originalStrength = VibrationManager.InternalStrength;
			VibrationManager.FadeVibration(strength.Value, fadeDuration.Value);
			if (blockNextSceneAutoFade.Value)
			{
				GameManager instance = GameManager.instance;
				if ((bool)instance)
				{
					instance.BlockNextVibrationFadeIn = true;
				}
			}
			else if (strength.Value > 0f && !blockNextSceneAutoFade.Value)
			{
				GameManager instance2 = GameManager.instance;
				if ((bool)instance2)
				{
					instance2.BlockNextVibrationFadeIn = false;
				}
			}
			Finish();
		}

		public override void OnExit()
		{
			if (resetOnExit.Value)
			{
				float duration = (restoreFadeDuration.IsNone ? fadeDuration.Value : restoreFadeDuration.Value);
				VibrationManager.FadeVibration(strengthOnExit.IsNone ? originalStrength : strengthOnExit.Value, duration);
			}
		}
	}
}
