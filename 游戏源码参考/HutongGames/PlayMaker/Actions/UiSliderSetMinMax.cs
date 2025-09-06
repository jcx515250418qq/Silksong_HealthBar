using UnityEngine;
using UnityEngine.UI;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.UI)]
	[Tooltip("Sets the minimum and maximum limits for the value of a UI Slider component. Optionally resets on exit")]
	public class UiSliderSetMinMax : ComponentAction<Slider>
	{
		[RequiredField]
		[CheckForComponent(typeof(Slider))]
		[Tooltip("The GameObject with the UI Slider component.")]
		public FsmOwnerDefault gameObject;

		[Tooltip("The minimum value of the UI Slider component. Leave as None for no effect")]
		public FsmFloat minValue;

		[Tooltip("The maximum value of the UI Slider component. Leave as None for no effect")]
		public FsmFloat maxValue;

		[Tooltip("Reset when exiting this state.")]
		public FsmBool resetOnExit;

		[Tooltip("Repeats every frame")]
		public bool everyFrame;

		private Slider slider;

		private float originalMinValue;

		private float originalMaxValue;

		public override void Reset()
		{
			gameObject = null;
			minValue = new FsmFloat
			{
				UseVariable = true
			};
			maxValue = new FsmFloat
			{
				UseVariable = true
			};
			resetOnExit = null;
			everyFrame = false;
		}

		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(gameObject);
			if (UpdateCache(ownerDefaultTarget))
			{
				slider = cachedComponent;
			}
			if (resetOnExit.Value)
			{
				originalMinValue = slider.minValue;
				originalMaxValue = slider.maxValue;
			}
			DoSetValue();
			if (!everyFrame)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			DoSetValue();
		}

		private void DoSetValue()
		{
			if (!(slider == null))
			{
				if (!minValue.IsNone)
				{
					slider.minValue = minValue.Value;
				}
				if (!maxValue.IsNone)
				{
					slider.maxValue = maxValue.Value;
				}
			}
		}

		public override void OnExit()
		{
			if (!(slider == null) && resetOnExit.Value)
			{
				slider.minValue = originalMinValue;
				slider.maxValue = originalMaxValue;
			}
		}
	}
}
