using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Color)]
	[Tooltip("Sets the RGBA channels of a Color Variable. To leave any channel unchanged, set variable to 'None'.")]
	public class SetColorRGBA : FsmStateAction
	{
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The Color Variable to set.")]
		public FsmColor colorVariable;

		[HasFloatSlider(0f, 1f)]
		[Tooltip("Set the red channel of the color variable.")]
		public FsmFloat red;

		[HasFloatSlider(0f, 1f)]
		[Tooltip("Set the green channel of the color variable.")]
		public FsmFloat green;

		[HasFloatSlider(0f, 1f)]
		[Tooltip("Set the blue channel of the color variable.")]
		public FsmFloat blue;

		[HasFloatSlider(0f, 1f)]
		[Tooltip("Set the alpha channel of the color variable.")]
		public FsmFloat alpha;

		[Tooltip("Repeat every frame.")]
		public bool everyFrame;

		public override void Reset()
		{
			colorVariable = null;
			red = 0f;
			green = 0f;
			blue = 0f;
			alpha = 1f;
			everyFrame = false;
		}

		public override void OnEnter()
		{
			DoSetColorRGBA();
			if (!everyFrame)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			DoSetColorRGBA();
		}

		private void DoSetColorRGBA()
		{
			if (colorVariable != null)
			{
				Color value = colorVariable.Value;
				if (!red.IsNone)
				{
					value.r = red.Value;
				}
				if (!green.IsNone)
				{
					value.g = green.Value;
				}
				if (!blue.IsNone)
				{
					value.b = blue.Value;
				}
				if (!alpha.IsNone)
				{
					value.a = alpha.Value;
				}
				colorVariable.Value = value;
			}
		}
	}
}
