using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	public class LerpByXPosition : FsmStateAction
	{
		[Serializable]
		public class FsmFloatMinMax
		{
			public FsmFloat MinValue;

			public FsmFloat MaxValue;

			[UIHint(UIHint.Variable)]
			public FsmFloat StoreValue;
		}

		public FsmGameObject StartPoint;

		[HideIf("IsStartPointDefined")]
		public FsmFloat StartPosition;

		public FsmGameObject EndPoint;

		[HideIf("IsEndPointDefined")]
		public FsmFloat EndPosition;

		public FsmVector2 TargetPosition;

		public FsmFloatMinMax[] Values;

		public bool EveryFrame;

		public bool IsStartPointDefined()
		{
			return !StartPoint.IsNone;
		}

		public bool IsEndPointDefined()
		{
			return !EndPoint.IsNone;
		}

		public override void Reset()
		{
			StartPoint = null;
			StartPosition = null;
			EndPoint = null;
			EndPosition = null;
			TargetPosition = null;
			EveryFrame = false;
		}

		public override void OnEnter()
		{
			DoAction();
			if (!EveryFrame)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			DoAction();
		}

		private void DoAction()
		{
			float num = (StartPoint.Value ? StartPoint.Value.transform.position.x : StartPosition.Value);
			float num2 = (EndPoint.Value ? EndPoint.Value.transform.position.x : EndPosition.Value);
			float num3 = Mathf.Clamp(TargetPosition.Value.x, (num < num2) ? num : num2, (num > num2) ? num : num2);
			float num4 = num2 - num;
			float num5 = (num2 - num3) / num4;
			float t = 1f - num5;
			FsmFloatMinMax[] values = Values;
			foreach (FsmFloatMinMax fsmFloatMinMax in values)
			{
				fsmFloatMinMax.StoreValue.Value = Mathf.Lerp(fsmFloatMinMax.MinValue.Value, fsmFloatMinMax.MaxValue.Value, t);
			}
		}
	}
}
