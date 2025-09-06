namespace HutongGames.PlayMaker.Actions
{
	public class SetCamLock : FsmStateAction
	{
		public FsmFloat xLockMin;

		public FsmFloat xLockMax;

		public FsmFloat yLockMin;

		public FsmFloat yLockMax;

		public bool everyFrame;

		private CameraTarget camTarget;

		public override void Reset()
		{
			xLockMin = new FsmFloat
			{
				UseVariable = true
			};
			xLockMax = new FsmFloat
			{
				UseVariable = true
			};
			yLockMin = new FsmFloat
			{
				UseVariable = true
			};
			yLockMax = new FsmFloat
			{
				UseVariable = true
			};
			everyFrame = false;
		}

		public override void OnEnter()
		{
			camTarget = GameCameras.instance.cameraTarget.GetComponent<CameraTarget>();
			DoSetCamLock();
			if (!everyFrame)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			DoSetCamLock();
		}

		public void DoSetCamLock()
		{
			if (!xLockMin.IsNone)
			{
				camTarget.xLockMin = xLockMin.Value;
			}
			if (!xLockMax.IsNone)
			{
				camTarget.xLockMax = xLockMax.Value;
			}
			if (!yLockMin.IsNone)
			{
				camTarget.yLockMin = yLockMin.Value;
			}
			if (!yLockMax.IsNone)
			{
				camTarget.yLockMax = yLockMax.Value;
			}
		}
	}
}
