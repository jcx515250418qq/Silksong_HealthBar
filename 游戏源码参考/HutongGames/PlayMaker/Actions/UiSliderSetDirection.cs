using UnityEngine;
using UnityEngine.UI;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.UI)]
	[Tooltip("Sets the direction of a UI Slider component.")]
	public class UiSliderSetDirection : ComponentAction<Slider>
	{
		[RequiredField]
		[CheckForComponent(typeof(Slider))]
		[Tooltip("The GameObject with the UI Slider component.")]
		public FsmOwnerDefault gameObject;

		[RequiredField]
		[Tooltip("The direction of the UI Slider component.")]
		[ObjectType(typeof(Slider.Direction))]
		public FsmEnum direction;

		[Tooltip("Include the  RectLayouts. Leave to none for no effect")]
		public FsmBool includeRectLayouts;

		[Tooltip("Reset when exiting this state.")]
		public FsmBool resetOnExit;

		private Slider slider;

		private Slider.Direction originalValue;

		public override void Reset()
		{
			gameObject = null;
			direction = Slider.Direction.LeftToRight;
			includeRectLayouts = new FsmBool
			{
				UseVariable = true
			};
			resetOnExit = null;
		}

		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(gameObject);
			if (UpdateCache(ownerDefaultTarget))
			{
				slider = cachedComponent;
			}
			originalValue = slider.direction;
			DoSetValue();
		}

		private void DoSetValue()
		{
			if (!(slider == null))
			{
				if (includeRectLayouts.IsNone)
				{
					slider.direction = (Slider.Direction)(object)direction.Value;
				}
				else
				{
					slider.SetDirection((Slider.Direction)(object)direction.Value, includeRectLayouts.Value);
				}
			}
		}

		public override void OnExit()
		{
			if (!(slider == null) && resetOnExit.Value)
			{
				if (includeRectLayouts.IsNone)
				{
					slider.direction = originalValue;
				}
				else
				{
					slider.SetDirection(originalValue, includeRectLayouts.Value);
				}
			}
		}
	}
}
