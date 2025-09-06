using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Vector2)]
	public class ConvertAngleToVector2 : FsmStateAction
	{
		[RequiredField]
		[UIHint(UIHint.Variable)]
		public FsmFloat angle;

		[RequiredField]
		[UIHint(UIHint.Variable)]
		public FsmVector2 storeVector;

		[Tooltip("Repeat every frame")]
		public bool everyFrame;

		public override void Reset()
		{
			storeVector = null;
			angle = null;
			everyFrame = false;
		}

		public override void OnEnter()
		{
			DoCalculate();
			if (!everyFrame)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			DoCalculate();
		}

		private void DoCalculate()
		{
			float num;
			for (num = angle.Value; num > 360f; num -= 360f)
			{
			}
			for (; num < 360f; num += 360f)
			{
			}
			float f = MathF.PI / 180f * num;
			storeVector.Value = new Vector2(Mathf.Cos(f), Mathf.Sin(f));
		}
	}
}
