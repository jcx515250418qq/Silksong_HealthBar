using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.AnimateVariables)]
	[Tooltip("Animates the value of a Vector3 Variable FROM-TO with assistance of Deformation Curves.")]
	public class CurveVector3 : CurveFsmAction
	{
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The Vector3 variable to animate.")]
		public FsmVector3 vectorVariable;

		[RequiredField]
		[Tooltip("Animate from this value.")]
		public FsmVector3 fromValue;

		[RequiredField]
		[Tooltip("Animate to this value.")]
		public FsmVector3 toValue;

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
		[Tooltip("Curve that controls the Z value.")]
		public FsmAnimationCurve curveZ;

		[Tooltip("Calculation lets you set a type of curve deformation that will be applied to otherwise linear move between fromValue.z and toValue.z.")]
		public Calculation calculationZ;

		private Vector3 vct;

		private bool finishInNextStep;

		public override void Reset()
		{
			base.Reset();
			vectorVariable = new FsmVector3
			{
				UseVariable = true
			};
			toValue = new FsmVector3
			{
				UseVariable = true
			};
			fromValue = new FsmVector3
			{
				UseVariable = true
			};
		}

		public override void OnEnter()
		{
			base.OnEnter();
			finishInNextStep = false;
			resultFloats = new float[3];
			fromFloats = new float[3];
			fromFloats[0] = (fromValue.IsNone ? 0f : fromValue.Value.x);
			fromFloats[1] = (fromValue.IsNone ? 0f : fromValue.Value.y);
			fromFloats[2] = (fromValue.IsNone ? 0f : fromValue.Value.z);
			toFloats = new float[3];
			toFloats[0] = (toValue.IsNone ? 0f : toValue.Value.x);
			toFloats[1] = (toValue.IsNone ? 0f : toValue.Value.y);
			toFloats[2] = (toValue.IsNone ? 0f : toValue.Value.z);
			curves = new AnimationCurve[3];
			curves[0] = curveX.curve;
			curves[1] = curveY.curve;
			curves[2] = curveZ.curve;
			calculations = new Calculation[3];
			calculations[0] = calculationX;
			calculations[1] = calculationY;
			calculations[2] = calculationZ;
			Init();
		}

		public override void OnExit()
		{
		}

		public override void OnUpdate()
		{
			base.OnUpdate();
			if (!vectorVariable.IsNone && isRunning)
			{
				vct = new Vector3(resultFloats[0], resultFloats[1], resultFloats[2]);
				vectorVariable.Value = vct;
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
				if (!vectorVariable.IsNone)
				{
					vct = new Vector3(resultFloats[0], resultFloats[1], resultFloats[2]);
					vectorVariable.Value = vct;
				}
				finishInNextStep = true;
			}
		}
	}
}
