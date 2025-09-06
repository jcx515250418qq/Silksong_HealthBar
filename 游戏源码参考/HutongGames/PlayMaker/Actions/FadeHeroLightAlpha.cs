using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	public class FadeHeroLightAlpha : FsmStateAction
	{
		public FsmOwnerDefault Target;

		public FsmFloat Alpha;

		public FsmFloat FadeDuration;

		public FsmBool SetAlphaOnExit;

		[UIHint(UIHint.Variable)]
		public FsmFloat StoreInitialAlpha;

		private HeroLight light;

		private float startingAlpha;

		public override void Reset()
		{
			Target = null;
			Alpha = null;
			FadeDuration = null;
			SetAlphaOnExit = null;
			StoreInitialAlpha = null;
		}

		public override void OnEnter()
		{
			GameObject safe = Target.GetSafe(this);
			if ((bool)safe)
			{
				light = safe.GetComponent<HeroLight>();
			}
			if ((bool)light)
			{
				startingAlpha = light.Alpha;
				StoreInitialAlpha.Value = startingAlpha;
				if (!(FadeDuration.Value > 0f))
				{
					light.Alpha = Alpha.Value;
					Finish();
				}
			}
			else
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			float progress = GetProgress();
			light.Alpha = Mathf.Lerp(startingAlpha, Alpha.Value, progress);
			if (progress >= 1f)
			{
				Finish();
			}
		}

		public override void OnExit()
		{
			if ((bool)light && SetAlphaOnExit.Value)
			{
				light.Alpha = Alpha.Value;
			}
		}

		public override float GetProgress()
		{
			return base.State.StateTime / FadeDuration.Value;
		}
	}
}
