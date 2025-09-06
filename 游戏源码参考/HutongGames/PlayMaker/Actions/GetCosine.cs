using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Trigonometry)]
	[Tooltip("Get the Cosine.")]
	public class GetCosine : FsmStateAction
	{
		[RequiredField]
		[Tooltip("The angle. Note: Check Deg To Rad if the angle is expressed in degrees.")]
		public FsmFloat angle;

		[Tooltip("Check if the angle is expressed in degrees.")]
		public FsmBool DegToRad;

		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The angle cosine.")]
		public FsmFloat result;

		[Tooltip("Repeat every frame.")]
		public bool everyFrame;

		public override void Reset()
		{
			angle = null;
			DegToRad = true;
			everyFrame = false;
			result = null;
		}

		public override void OnEnter()
		{
			DoCosine();
			if (!everyFrame)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			DoCosine();
		}

		private void DoCosine()
		{
			float num = angle.Value;
			if (DegToRad.Value)
			{
				num *= MathF.PI / 180f;
			}
			result.Value = Mathf.Cos(num);
		}
	}
}
