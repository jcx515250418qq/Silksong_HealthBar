using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("RectTransform")]
	[Tooltip("RectTransforms position from world space into screen space. Leave the camera to none for default behavior.")]
	public class RectTransformWorldToScreenPoint : BaseUpdateAction
	{
		[RequiredField]
		[CheckForComponent(typeof(RectTransform))]
		[Tooltip("The GameObject target.")]
		public FsmOwnerDefault gameObject;

		[CheckForComponent(typeof(Camera))]
		[Tooltip("The camera to perform the calculation. Leave as None for default behavior.")]
		public FsmOwnerDefault camera;

		[UIHint(UIHint.Variable)]
		[Tooltip("Store the screen position in a Vector3 Variable. Z will equal zero.")]
		public FsmVector3 screenPoint;

		[UIHint(UIHint.Variable)]
		[Tooltip("Store the screen X position in a Float Variable.")]
		public FsmFloat screenX;

		[UIHint(UIHint.Variable)]
		[Tooltip("Store the screen Y position in a Float Variable.")]
		public FsmFloat screenY;

		[Tooltip("Normalize screen coordinates (0-1). Otherwise coordinates are in pixels.")]
		public FsmBool normalize;

		private RectTransform _rt;

		private Camera _cam;

		public override void Reset()
		{
			base.Reset();
			gameObject = null;
			camera = new FsmOwnerDefault();
			camera.OwnerOption = OwnerDefaultOption.SpecifyGameObject;
			camera.GameObject = new FsmGameObject
			{
				UseVariable = true
			};
			screenPoint = null;
			screenX = null;
			screenY = null;
			everyFrame = false;
		}

		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(gameObject);
			if (ownerDefaultTarget != null)
			{
				_rt = ownerDefaultTarget.GetComponent<RectTransform>();
			}
			if (base.Fsm.GetOwnerDefaultTarget(camera) != null)
			{
				_cam = ownerDefaultTarget.GetComponent<Camera>();
			}
			DoWorldToScreenPoint();
			if (!everyFrame)
			{
				Finish();
			}
		}

		public override void OnActionUpdate()
		{
			DoWorldToScreenPoint();
		}

		private void DoWorldToScreenPoint()
		{
			Vector2 vector = RectTransformUtility.WorldToScreenPoint(_cam, _rt.position);
			if (normalize.Value)
			{
				vector.x /= Screen.width;
				vector.y /= Screen.height;
			}
			screenPoint.Value = vector;
			screenX.Value = vector.x;
			screenY.Value = vector.y;
		}
	}
}
