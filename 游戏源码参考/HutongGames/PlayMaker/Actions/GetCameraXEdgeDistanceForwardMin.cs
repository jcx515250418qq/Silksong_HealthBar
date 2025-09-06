using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	public class GetCameraXEdgeDistanceForwardMin : FsmStateAction
	{
		public FsmOwnerDefault From;

		public FsmFloat Min;

		[UIHint(UIHint.Variable)]
		public FsmFloat StoreResult;

		public override void Reset()
		{
			From = null;
			Min = new FsmFloat
			{
				UseVariable = true
			};
			StoreResult = null;
		}

		public override void OnEnter()
		{
			GameObject safe = From.GetSafe(this);
			HeroController component = safe.GetComponent<HeroController>();
			bool flag = ((!component) ? (Mathf.Sign(safe.transform.lossyScale.x) > 0f) : component.cState.facingRight);
			Vector2 vector = GameCameras.instance.mainCamera.transform.position;
			float num = 8.3f * ForceCameraAspect.CurrentViewportAspect;
			float num2 = vector.x - num;
			float num3 = vector.x + num;
			float x = safe.transform.position.x;
			float num4 = ((!flag) ? (x - num2) : (num3 - x));
			if (!Min.IsNone && num4 < Min.Value)
			{
				num4 = Min.Value;
			}
			StoreResult.Value = num4;
			Finish();
		}
	}
}
