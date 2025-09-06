using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Vector2)]
	[Tooltip("Interpolates between 2 Vector2 values over a specified Time.")]
	public class Vector2Interpolate : FsmStateAction
	{
		[Tooltip("The interpolation type")]
		public InterpolationType mode;

		[RequiredField]
		[Tooltip("The vector to interpolate from")]
		public FsmVector2 fromVector;

		[RequiredField]
		[Tooltip("The vector to interpolate to")]
		public FsmVector2 toVector;

		[RequiredField]
		[Tooltip("the interpolate time")]
		public FsmFloat time;

		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("the interpolated result")]
		public FsmVector2 storeResult;

		[Tooltip("This event is fired when the interpolation is done.")]
		public FsmEvent finishEvent;

		[Tooltip("Ignore TimeScale")]
		public bool realTime;

		private float startTime;

		private float currentTime;

		public override void Reset()
		{
			mode = InterpolationType.Linear;
			fromVector = new FsmVector2
			{
				UseVariable = true
			};
			toVector = new FsmVector2
			{
				UseVariable = true
			};
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
				storeResult.Value = fromVector.Value;
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
			InterpolationType interpolationType = mode;
			if (interpolationType != 0 && interpolationType == InterpolationType.EaseInOut)
			{
				num = Mathf.SmoothStep(0f, 1f, num);
			}
			storeResult.Value = Vector2.Lerp(fromVector.Value, toVector.Value, num);
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
