using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[Tooltip("Checks the alpha value of the screen fader and triggers events based on comparison.")]
	public class CheckScreenFaderState : FsmStateAction
	{
		public enum CompareMethod
		{
			Equal = 0,
			LessThan = 1,
			GreaterThan = 2
		}

		[Tooltip("The target alpha value to compare against.")]
		public FsmFloat targetAlpha;

		[Tooltip("Comparison method: Equal, LessThan, GreaterThan.")]
		public CompareMethod comparisonMethod;

		[Tooltip("Event to send if the comparison is true.")]
		public FsmEvent trueEvent;

		[Tooltip("Event to send if the comparison is false.")]
		public FsmEvent falseEvent;

		[Tooltip("Whether to check every frame.")]
		public FsmBool everyFrame;

		public override void Reset()
		{
			targetAlpha = 0f;
			comparisonMethod = CompareMethod.Equal;
			trueEvent = null;
			falseEvent = null;
			everyFrame = false;
		}

		public override void OnEnter()
		{
			CheckAlpha();
			if (!everyFrame.Value)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			if (everyFrame.Value)
			{
				CheckAlpha();
			}
		}

		private void CheckAlpha()
		{
			float alpha = ScreenFaderState.Alpha;
			bool flag = Compare(alpha, targetAlpha.Value, comparisonMethod);
			base.Fsm.Event(flag ? trueEvent : falseEvent);
		}

		private bool Compare(float value1, float value2, CompareMethod method)
		{
			return method switch
			{
				CompareMethod.Equal => Mathf.Approximately(value1, value2), 
				CompareMethod.LessThan => value1 < value2, 
				CompareMethod.GreaterThan => value1 > value2, 
				_ => false, 
			};
		}
	}
}
