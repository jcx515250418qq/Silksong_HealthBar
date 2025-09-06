using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.GUI)]
	[Tooltip("GUI Horizontal Slider connected to a Float Variable.")]
	public class GUIHorizontalSlider : GUIAction
	{
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The float variable to link the slider to. Moving the slider changes the value, and changes in the value move the slider.")]
		public FsmFloat floatVariable;

		[RequiredField]
		[Tooltip("The value of the float variable when slider is all the way to the left.")]
		public FsmFloat leftValue;

		[RequiredField]
		[Tooltip("The value of the float variable when slider is all the way to the right.")]
		public FsmFloat rightValue;

		[Tooltip("Optional GUIStyle for the slider track.")]
		public FsmString sliderStyle;

		[Tooltip("Optional GUIStyle for the slider thumb.")]
		public FsmString thumbStyle;

		public override void Reset()
		{
			base.Reset();
			floatVariable = null;
			leftValue = 0f;
			rightValue = 100f;
			sliderStyle = "horizontalslider";
			thumbStyle = "horizontalsliderthumb";
		}

		public override void OnGUI()
		{
			base.OnGUI();
			if (floatVariable != null)
			{
				floatVariable.Value = GUI.HorizontalSlider(rect, floatVariable.Value, leftValue.Value, rightValue.Value, (sliderStyle.Value != "") ? sliderStyle.Value : "horizontalslider", (thumbStyle.Value != "") ? thumbStyle.Value : "horizontalsliderthumb");
			}
		}
	}
}
