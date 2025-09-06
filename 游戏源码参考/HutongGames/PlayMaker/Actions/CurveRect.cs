using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("AnimateVariables")]
	[Tooltip("Animates the value of a Rect Variable FROM-TO with assistance of Deformation Curves.")]
	public class CurveRect : CurveFsmAction
	{
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The Rect Variable to animate.")]
		public FsmRect rectVariable;

		[RequiredField]
		[Tooltip("The Rect to animate from.")]
		public FsmRect fromValue;

		[RequiredField]
		[Tooltip("The Rect to animate to.")]
		public FsmRect toValue;

		[RequiredField]
		[Tooltip("Curve that controls the X value.")]
		public FsmAnimationCurve curveX;

		[Tooltip("Calculation lets you set a type of curve deformation that will be applied to otherwise linear move between fromValue.x and toValue.x.")]
		public Calculation calculationX;

		[RequiredField]
		[Tooltip("Curve that controls the Y value.")]
		public FsmAnimationCurve curveY;

		[Tooltip("Calculation lets you set a type of curve deformation that will be applied to otherwise linear move between fromValue.y and toValue.y.")]
		public Calculation calculationY;

		[RequiredField]
		[Tooltip("Curve that controls the Width.")]
		public FsmAnimationCurve curveW;

		[Tooltip("Calculation lets you set a type of curve deformation that will be applied to otherwise linear move between fromValue.width and toValue.width.")]
		public Calculation calculationW;

		[RequiredField]
		[Tooltip("Curve that controls the Height.")]
		public FsmAnimationCurve curveH;

		[Tooltip("Calculation lets you set a type of curve deformation that will be applied to otherwise linear move between fromValue.height and toValue.height.")]
		public Calculation calculationH;

		private Rect rct;

		private bool finishInNextStep;

		public override void Reset()
		{
			base.Reset();
			rectVariable = new FsmRect
			{
				UseVariable = true
			};
			toValue = new FsmRect
			{
				UseVariable = true
			};
			fromValue = new FsmRect
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
			fromFloats[0] = (fromValue.IsNone ? 0f : fromValue.Value.x);
			fromFloats[1] = (fromValue.IsNone ? 0f : fromValue.Value.y);
			fromFloats[2] = (fromValue.IsNone ? 0f : fromValue.Value.width);
			fromFloats[3] = (fromValue.IsNone ? 0f : fromValue.Value.height);
			toFloats = new float[4];
			toFloats[0] = (toValue.IsNone ? 0f : toValue.Value.x);
			toFloats[1] = (toValue.IsNone ? 0f : toValue.Value.y);
			toFloats[2] = (toValue.IsNone ? 0f : toValue.Value.width);
			toFloats[3] = (toValue.IsNone ? 0f : toValue.Value.height);
			curves = new AnimationCurve[4];
			curves[0] = curveX.curve;
			curves[1] = curveY.curve;
			curves[2] = curveW.curve;
			curves[3] = curveH.curve;
			calculations = new Calculation[4];
			calculations[0] = calculationX;
			calculations[1] = calculationY;
			calculations[2] = calculationW;
			calculations[2] = calculationH;
			Init();
		}

		public override void OnExit()
		{
		}

		public override void OnUpdate()
		{
			base.OnUpdate();
			if (!rectVariable.IsNone && isRunning)
			{
				rct = new Rect(resultFloats[0], resultFloats[1], resultFloats[2], resultFloats[3]);
				rectVariable.Value = rct;
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
				if (!rectVariable.IsNone)
				{
					rct = new Rect(resultFloats[0], resultFloats[1], resultFloats[2], resultFloats[3]);
					rectVariable.Value = rct;
				}
				finishInNextStep = true;
			}
		}
	}
}
