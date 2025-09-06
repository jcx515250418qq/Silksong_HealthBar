using UnityEngine;
using UnityEngine.UI;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.UI)]
	[Tooltip("Gets the wholeNumbers property of a UI Slider component. If true, the Slider is constrained to integer values")]
	public class UiSliderGetWholeNumbers : ComponentAction<Slider>
	{
		[RequiredField]
		[CheckForComponent(typeof(Slider))]
		[Tooltip("The GameObject with the UI Slider component.")]
		public FsmOwnerDefault gameObject;

		[UIHint(UIHint.Variable)]
		[Tooltip("Is the Slider constrained to integer values?")]
		public FsmBool wholeNumbers;

		[Tooltip("Event sent if slider is showing integers")]
		public FsmEvent isShowingWholeNumbersEvent;

		[Tooltip("Event sent if slider is showing floats")]
		public FsmEvent isNotShowingWholeNumbersEvent;

		private Slider slider;

		public override void Reset()
		{
			gameObject = null;
			isShowingWholeNumbersEvent = null;
			isNotShowingWholeNumbersEvent = null;
			wholeNumbers = null;
		}

		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(gameObject);
			if (UpdateCache(ownerDefaultTarget))
			{
				slider = cachedComponent;
			}
			DoGetValue();
			Finish();
		}

		private void DoGetValue()
		{
			bool flag = false;
			if (slider != null)
			{
				flag = slider.wholeNumbers;
			}
			wholeNumbers.Value = flag;
			base.Fsm.Event(flag ? isShowingWholeNumbersEvent : isNotShowingWholeNumbersEvent);
		}
	}
}
