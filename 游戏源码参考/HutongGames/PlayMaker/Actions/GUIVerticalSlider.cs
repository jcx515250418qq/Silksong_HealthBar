using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.GUI)]
	[Tooltip("GUI Vertical Slider connected to a Float Variable.")]
	public class GUIVerticalSlider : GUIAction
	{
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The Float Variable linked to the slider value.")]
		public FsmFloat floatVariable;

		[RequiredField]
		[Tooltip("The value of the variable at the top of the slider.")]
		public FsmFloat topValue;

		[RequiredField]
		[Tooltip("The value of the variable at the bottom of the slider.")]
		public FsmFloat bottomValue;

		[Tooltip("Optional GUIStyle for the slider track.")]
		public FsmString sliderStyle;

		[Tooltip("Optional GUIStyle for the slider thumb.")]
		public FsmString thumbStyle;

		public override void Reset()
		{
			base.Reset();
			floatVariable = null;
			topValue = 100f;
			bottomValue = 0f;
			sliderStyle = "verticalslider";
			thumbStyle = "verticalsliderthumb";
			width = 0.1f;
		}

		public override void OnGUI()
		{
			base.OnGUI();
			if (floatVariable != null)
			{
				floatVariable.Value = GUI.VerticalSlider(rect, floatVariable.Value, topValue.Value, bottomValue.Value, (sliderStyle.Value != "") ? sliderStyle.Value : "verticalslider", (thumbStyle.Value != "") ? thumbStyle.Value : "verticalsliderthumb");
			}
		}
	}
}
