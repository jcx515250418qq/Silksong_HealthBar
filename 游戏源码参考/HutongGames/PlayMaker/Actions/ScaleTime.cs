using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Time)]
	[Tooltip("Scales time: 1 = normal, 0.5 = half speed, 2 = double speed.")]
	public class ScaleTime : FsmStateAction
	{
		[RequiredField]
		[HasFloatSlider(0f, 4f)]
		[Tooltip("Scales time: 1 = normal, 0.5 = half speed, 2 = double speed.")]
		public FsmFloat timeScale;

		[Tooltip("Adjust the fixed physics time step to match the time scale.")]
		public FsmBool adjustFixedDeltaTime;

		[Tooltip("Repeat every frame. Useful when animating the value.")]
		public bool everyFrame;

		public override void Reset()
		{
			timeScale = 1f;
			adjustFixedDeltaTime = true;
			everyFrame = false;
		}

		public override void OnEnter()
		{
			DoTimeScale();
			if (!everyFrame)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			DoTimeScale();
		}

		private void DoTimeScale()
		{
			Time.timeScale = timeScale.Value;
			if (adjustFixedDeltaTime.Value)
			{
				Time.fixedDeltaTime = 0.02f * Time.timeScale;
			}
		}
	}
}
