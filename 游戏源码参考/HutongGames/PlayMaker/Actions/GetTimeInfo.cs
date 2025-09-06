using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Time)]
	[Tooltip("Gets various useful Time measurements.")]
	public class GetTimeInfo : FsmStateAction
	{
		public enum TimeInfo
		{
			DeltaTime = 0,
			TimeScale = 1,
			SmoothDeltaTime = 2,
			TimeInCurrentState = 3,
			TimeSinceStartup = 4,
			TimeSinceLevelLoad = 5,
			RealTimeSinceStartup = 6,
			RealTimeInCurrentState = 7
		}

		[Tooltip("Info to get.")]
		public TimeInfo getInfo;

		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the time info in a float variable.")]
		public FsmFloat storeValue;

		[Tooltip("Repeat every frame.")]
		public bool everyFrame;

		public override void Reset()
		{
			getInfo = TimeInfo.TimeSinceLevelLoad;
			storeValue = null;
			everyFrame = false;
		}

		public override void OnEnter()
		{
			DoGetTimeInfo();
			if (!everyFrame)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			DoGetTimeInfo();
		}

		private void DoGetTimeInfo()
		{
			switch (getInfo)
			{
			case TimeInfo.DeltaTime:
				storeValue.Value = Time.deltaTime;
				break;
			case TimeInfo.TimeScale:
				storeValue.Value = Time.timeScale;
				break;
			case TimeInfo.SmoothDeltaTime:
				storeValue.Value = Time.smoothDeltaTime;
				break;
			case TimeInfo.TimeInCurrentState:
				storeValue.Value = base.State.StateTime;
				break;
			case TimeInfo.TimeSinceStartup:
				storeValue.Value = Time.time;
				break;
			case TimeInfo.TimeSinceLevelLoad:
				storeValue.Value = Time.timeSinceLevelLoad;
				break;
			case TimeInfo.RealTimeSinceStartup:
				storeValue.Value = FsmTime.RealtimeSinceStartup;
				break;
			case TimeInfo.RealTimeInCurrentState:
				storeValue.Value = FsmTime.RealtimeSinceStartup - base.State.RealStartTime;
				break;
			default:
				storeValue.Value = 0f;
				break;
			}
		}
	}
}
