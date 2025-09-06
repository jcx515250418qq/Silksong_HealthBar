using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.AnimateVariables)]
	[Tooltip("Animates the value of a Float Variable FROM-TO with assistance of Deformation Curve.")]
	public class CurveFloat : CurveFsmAction
	{
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The float variable to animate.")]
		public FsmFloat floatVariable;

		[RequiredField]
		[Tooltip("Animate from this value.")]
		public FsmFloat fromValue;

		[RequiredField]
		[Tooltip("Animate to this value.")]
		public FsmFloat toValue;

		[RequiredField]
		[Tooltip("The curve to use when animating.")]
		public FsmAnimationCurve animCurve;

		[Tooltip("Calculation lets you set a type of curve deformation that will be applied to otherwise linear move between fromValue and toValue.")]
		public Calculation calculation;

		private bool finishInNextStep;

		public override void Reset()
		{
			base.Reset();
			floatVariable = new FsmFloat
			{
				UseVariable = true
			};
			toValue = new FsmFloat
			{
				UseVariable = true
			};
			fromValue = new FsmFloat
			{
				UseVariable = true
			};
		}

		public override void OnEnter()
		{
			base.OnEnter();
			finishInNextStep = false;
			resultFloats = new float[1];
			fromFloats = new float[1];
			fromFloats[0] = (fromValue.IsNone ? 0f : fromValue.Value);
			toFloats = new float[1];
			toFloats[0] = (toValue.IsNone ? 0f : toValue.Value);
			calculations = new Calculation[1];
			calculations[0] = calculation;
			curves = new AnimationCurve[1];
			curves[0] = animCurve.curve;
			Init();
		}

		public override void OnExit()
		{
		}

		public override void OnUpdate()
		{
			base.OnUpdate();
			if (!floatVariable.IsNone && isRunning)
			{
				floatVariable.Value = resultFloats[0];
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
				if (!floatVariable.IsNone)
				{
					floatVariable.Value = resultFloats[0];
				}
				finishInNextStep = true;
			}
		}
	}
}
