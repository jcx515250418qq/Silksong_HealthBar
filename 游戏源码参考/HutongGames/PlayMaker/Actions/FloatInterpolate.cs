using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Math)]
	[Tooltip("Interpolates between 2 Float values over a specified Time.")]
	public class FloatInterpolate : FsmStateAction
	{
		[Tooltip("Interpolation mode: Linear or EaseInOut.")]
		public InterpolationType mode;

		[RequiredField]
		[Tooltip("Interpolate from this value.")]
		public FsmFloat fromFloat;

		[RequiredField]
		[Tooltip("Interpolate to this value.")]
		public FsmFloat toFloat;

		[RequiredField]
		[Tooltip("Interpolate over this amount of time in seconds.")]
		public FsmFloat time;

		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the current value in a float variable.")]
		public FsmFloat storeResult;

		[Tooltip("Event to send when the interpolation is finished.")]
		public FsmEvent finishEvent;

		[Tooltip("Ignore TimeScale. Useful if the game is paused (Time scaled to 0).")]
		public bool realTime;

		private float startTime;

		private float currentTime;

		public override void Reset()
		{
			mode = InterpolationType.Linear;
			fromFloat = null;
			toFloat = null;
			time = 1f;
			storeResult = null;
			finishEvent = null;
			realTime = false;
		}

		public override void OnEnter()
		{
			startTime = FsmTime.RealtimeSinceStartup;
			currentTime = 0f;
			if (storeResult == null)
			{
				Finish();
			}
			else
			{
				storeResult.Value = fromFloat.Value;
			}
		}

		public override void OnUpdate()
		{
			if (realTime)
			{
				currentTime = FsmTime.RealtimeSinceStartup - startTime;
			}
			else
			{
				currentTime += Time.deltaTime;
			}
			float num = currentTime / time.Value;
			switch (mode)
			{
			case InterpolationType.Linear:
				storeResult.Value = Mathf.Lerp(fromFloat.Value, toFloat.Value, num);
				break;
			case InterpolationType.EaseInOut:
				storeResult.Value = Mathf.SmoothStep(fromFloat.Value, toFloat.Value, num);
				break;
			}
			if (num >= 1f)
			{
				if (finishEvent != null)
				{
					base.Fsm.Event(finishEvent);
				}
				Finish();
			}
		}
	}
}
