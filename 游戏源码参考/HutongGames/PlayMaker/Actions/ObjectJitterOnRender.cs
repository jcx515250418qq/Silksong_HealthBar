using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	public class ObjectJitterOnRender : FsmStateAction
	{
		[RequiredField]
		public FsmOwnerDefault Target;

		public FsmFloat X;

		public FsmFloat Y;

		public FsmFloat Z;

		public FsmFloat LimitFps;

		private GameObject go;

		private Vector3 initialPos;

		private Vector3 preCullPos;

		private Vector3 targetPos;

		private double nextUpdateTime;

		public override void Reset()
		{
			Target = null;
			X = new FsmFloat
			{
				UseVariable = true
			};
			Y = new FsmFloat
			{
				UseVariable = true
			};
			Z = new FsmFloat
			{
				UseVariable = true
			};
			LimitFps = null;
		}

		public override void OnEnter()
		{
			go = base.Fsm.GetOwnerDefaultTarget(Target);
			if (go == null)
			{
				Finish();
				return;
			}
			initialPos = go.transform.localPosition;
			OnUpdate();
			CameraRenderHooks.CameraPreCull += OnCameraPreCull;
			CameraRenderHooks.CameraPostRender += OnCameraPostRender;
		}

		public override void OnExit()
		{
			CameraRenderHooks.CameraPreCull -= OnCameraPreCull;
			CameraRenderHooks.CameraPostRender -= OnCameraPostRender;
		}

		public override void OnUpdate()
		{
			if (LimitFps.Value > 0f)
			{
				double timeAsDouble = Time.timeAsDouble;
				if (timeAsDouble < nextUpdateTime)
				{
					return;
				}
				nextUpdateTime = timeAsDouble + (double)(1f / LimitFps.Value);
			}
			targetPos = initialPos + new Vector3(Random.Range(0f - X.Value, X.Value), Random.Range(0f - Y.Value, Y.Value), Random.Range(0f - Z.Value, Z.Value));
		}

		private void OnCameraPreCull(CameraRenderHooks.CameraSource cameraType)
		{
			if (cameraType == CameraRenderHooks.CameraSource.MainCamera && !(Time.timeScale <= Mathf.Epsilon))
			{
				Transform transform = go.transform;
				preCullPos = transform.localPosition;
				transform.localPosition = targetPos;
			}
		}

		private void OnCameraPostRender(CameraRenderHooks.CameraSource cameraType)
		{
			if (cameraType == CameraRenderHooks.CameraSource.MainCamera && !(Time.timeScale <= Mathf.Epsilon))
			{
				go.transform.localPosition = preCullPos;
				initialPos = preCullPos;
			}
		}
	}
}
