using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Vector3)]
	[Tooltip("Interpolates between 2 Vector3 values over a specified Time.")]
	public class Vector3Interpolate : FsmStateAction
	{
		[Tooltip("The type of interpolation to use.")]
		public InterpolationType mode;

		[RequiredField]
		[Tooltip("The start vector.")]
		public FsmVector3 fromVector;

		[RequiredField]
		[Tooltip("The end vector.")]
		public FsmVector3 toVector;

		[RequiredField]
		[Tooltip("How long it should take to interpolate from start to end.")]
		public FsmFloat time;

		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the interpolated vector in a Vector3 Variable.")]
		public FsmVector3 storeResult;

		[Tooltip("Optionally send this event when finished.")]
		public FsmEvent finishEvent;

		[Tooltip("Ignore TimeScale e.g., if the game is paused.")]
		public bool realTime;

		private float startTime;

		private float currentTime;

		public override void Reset()
		{
			mode = InterpolationType.Linear;
			fromVector = new FsmVector3
			{
				UseVariable = true
			};
			toVector = new FsmVector3
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
			storeResult.Value = Vector3.Lerp(fromVector.Value, toVector.Value, num);
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
