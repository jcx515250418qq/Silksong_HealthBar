using UnityEngine;
using UnityEngine.UI;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.UI)]
	[Tooltip("Sets the wholeNumbers property of a UI Slider component. This defines if the slider will be constrained to integer values.")]
	public class UiSliderSetWholeNumbers : ComponentAction<Slider>
	{
		[RequiredField]
		[CheckForComponent(typeof(Slider))]
		[Tooltip("The GameObject with the UI Slider component.")]
		public FsmOwnerDefault gameObject;

		[RequiredField]
		[Tooltip("Should the slider be constrained to integer values?")]
		public FsmBool wholeNumbers;

		[Tooltip("Reset when exiting this state.")]
		public FsmBool resetOnExit;

		private Slider slider;

		private bool originalValue;

		public override void Reset()
		{
			gameObject = null;
			wholeNumbers = null;
			resetOnExit = null;
		}

		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(gameObject);
			if (UpdateCache(ownerDefaultTarget))
			{
				slider = cachedComponent;
			}
			originalValue = slider.wholeNumbers;
			DoSetValue();
			Finish();
		}

		private void DoSetValue()
		{
			if (slider != null)
			{
				slider.wholeNumbers = wholeNumbers.Value;
			}
		}

		public override void OnExit()
		{
			if (!(slider == null) && resetOnExit.Value)
			{
				slider.wholeNumbers = originalValue;
			}
		}
	}
}
