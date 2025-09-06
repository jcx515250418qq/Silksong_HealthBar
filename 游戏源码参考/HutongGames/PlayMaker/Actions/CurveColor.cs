using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.AnimateVariables)]
	[Tooltip("Animates the value of a Color Variable FROM-TO with assistance of Deformation Curves.")]
	public class CurveColor : CurveFsmAction
	{
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The Color variable to animate.")]
		public FsmColor colorVariable;

		[RequiredField]
		[Tooltip("Animate from this color.")]
		public FsmColor fromValue;

		[RequiredField]
		[Tooltip("Animate to this color.")]
		public FsmColor toValue;

		[RequiredField]
		[Tooltip("The curve used to animate the red value.")]
		public FsmAnimationCurve curveR;

		[Tooltip("Calculation lets you set a type of curve deformation that will be applied to otherwise linear move between fromValue.Red and toValue.Rec.")]
		public Calculation calculationR;

		[RequiredField]
		[Tooltip("The curve used to animate the green value.")]
		public FsmAnimationCurve curveG;

		[Tooltip("Calculation lets you set a type of curve deformation that will be applied to otherwise linear move between fromValue.Green and toValue.Green.")]
		public Calculation calculationG;

		[RequiredField]
		[Tooltip("The curve used to animate the blue value.")]
		public FsmAnimationCurve curveB;

		[Tooltip("Calculation lets you set a type of curve deformation that will be applied to otherwise linear move between fromValue.Blue and toValue.Blue.")]
		public Calculation calculationB;

		[RequiredField]
		[Tooltip("The curve used to animate the alpha value.")]
		public FsmAnimationCurve curveA;

		[Tooltip("Calculation lets you set a type of curve deformation that will be applied to otherwise linear move between fromValue.Alpha and toValue.Alpha.")]
		public Calculation calculationA;

		private Color clr;

		private bool finishInNextStep;

		public override void Reset()
		{
			base.Reset();
			colorVariable = new FsmColor
			{
				UseVariable = true
			};
			toValue = new FsmColor
			{
				UseVariable = true
			};
			fromValue = new FsmColor
			{
				UseVariable = true
			};
		}

		public override void OnEnter()
		{
			base.OnEnter();
			finishInNextStep = false;
			resultFloats = new float[4];
			fromFloats = new float[4];
			fromFloats[0] = (fromValue.IsNone ? 0f : fromValue.Value.r);
			fromFloats[1] = (fromValue.IsNone ? 0f : fromValue.Value.g);
			fromFloats[2] = (fromValue.IsNone ? 0f : fromValue.Value.b);
			fromFloats[3] = (fromValue.IsNone ? 0f : fromValue.Value.a);
			toFloats = new float[4];
			toFloats[0] = (toValue.IsNone ? 0f : toValue.Value.r);
			toFloats[1] = (toValue.IsNone ? 0f : toValue.Value.g);
			toFloats[2] = (toValue.IsNone ? 0f : toValue.Value.b);
			toFloats[3] = (toValue.IsNone ? 0f : toValue.Value.a);
			curves = new AnimationCurve[4];
			curves[0] = curveR.curve;
			curves[1] = curveG.curve;
			curves[2] = curveB.curve;
			curves[3] = curveA.curve;
			calculations = new Calculation[4];
			calculations[0] = calculationR;
			calculations[1] = calculationG;
			calculations[2] = calculationB;
			calculations[3] = calculationA;
			Init();
		}

		public override void OnExit()
		{
		}

		public override void OnUpdate()
		{
			base.OnUpdate();
			if (!colorVariable.IsNone && isRunning)
			{
				clr = new Color(resultFloats[0], resultFloats[1], resultFloats[2], resultFloats[3]);
				colorVariable.Value = clr;
			}
			if (finishInNextStep && !looping)
			{
				Finish();
				if (finishEvent != null)
				{
					base.Fsm.Event(finishEvent);
				}
			}
			if (finishAction && !finishInNextStep)
			{
				if (!colorVariable.IsNone)
				{
					clr = new Color(resultFloats[0], resultFloats[1], resultFloats[2], resultFloats[3]);
					colorVariable.Value = clr;
				}
				finishInNextStep = true;
			}
		}
	}
}
