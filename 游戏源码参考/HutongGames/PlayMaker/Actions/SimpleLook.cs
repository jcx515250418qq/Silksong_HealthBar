using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Transform)]
	[Tooltip("Rotates a GameObject based on a Vector2 input, typically from a PlayerInput action. Use it on a player GameObject for MouseLook type behaviour. It is common to setup the camera as a child of the 'body', so the body rotates left/right while the camera tilts up/down.Minimum and Maximum values can be used to constrain the rotation.")]
	public class SimpleLook : ComponentAction<Transform>
	{
		[RequiredField]
		[Tooltip("The GameObject to rotate.")]
		public FsmOwnerDefault gameObject;

		[Tooltip("The Camera is often the child of the GameObject 'body'. If you specify a Camera, it will tilt up down, while the body rotates left/right. If you leave this empty, all rotations will be applied to the main GameObject.")]
		public new FsmGameObject camera;

		[RequiredField]
		[Tooltip("Vector2 input, typically from a PlayerInput action.")]
		public FsmVector2 vector2Input;

		[RequiredField]
		[Tooltip("Sensitivity of movement in X direction (rotate left/right).")]
		public FsmFloat sensitivityX;

		[RequiredField]
		[Tooltip("Sensitivity of movement in Y direction (tilt up/down).")]
		public FsmFloat sensitivityY;

		[HasFloatSlider(-360f, 360f)]
		[Tooltip("Clamp rotation around X axis. Set to None for no clamping.")]
		public FsmFloat minimumX;

		[HasFloatSlider(-360f, 360f)]
		[Tooltip("Clamp rotation around X axis. Set to None for no clamping.")]
		public FsmFloat maximumX;

		[HasFloatSlider(-360f, 360f)]
		[Tooltip("Clamp rotation around Y axis. Set to None for no clamping.")]
		public FsmFloat minimumY;

		[HasFloatSlider(-360f, 360f)]
		[Tooltip("Clamp rotation around Y axis. Set to None for no clamping.")]
		public FsmFloat maximumY;

		[Tooltip("Repeat every frame.")]
		public bool everyFrame;

		private float rotationX;

		private float rotationY;

		private Transform cachedCameraTransform;

		public override void Reset()
		{
			gameObject = null;
			vector2Input = new FsmVector2
			{
				UseVariable = true
			};
			sensitivityX = 1f;
			sensitivityY = 1f;
			minimumX = new FsmFloat
			{
				UseVariable = true
			};
			maximumX = new FsmFloat
			{
				UseVariable = true
			};
			minimumY = -60f;
			maximumY = 60f;
			everyFrame = true;
		}

		public override void OnEnter()
		{
			if (!UpdateCachedTransform(base.Fsm.GetOwnerDefaultTarget(gameObject)) || vector2Input.IsNone)
			{
				Finish();
				return;
			}
			if (camera.Value != null)
			{
				cachedCameraTransform = camera.Value.transform;
			}
			Rigidbody component = cachedGameObject.GetComponent<Rigidbody>();
			if (component != null)
			{
				component.freezeRotation = true;
			}
			Quaternion localRotation = base.cachedTransform.localRotation;
			rotationX = localRotation.eulerAngles.y;
			rotationY = ((cachedCameraTransform == null) ? localRotation.eulerAngles.x : cachedCameraTransform.localRotation.eulerAngles.x);
			if (!everyFrame)
			{
				DoLookRotate();
				Finish();
			}
		}

		public override void OnUpdate()
		{
			DoLookRotate();
		}

		private void DoLookRotate()
		{
			if (!UpdateCachedTransform(base.Fsm.GetOwnerDefaultTarget(gameObject)))
			{
				Finish();
				return;
			}
			if (cachedCameraTransform == null)
			{
				base.cachedTransform.localEulerAngles = new Vector3(GetYRotation(), GetXRotation(), 0f);
				return;
			}
			base.cachedTransform.localEulerAngles = new Vector3(base.cachedTransform.localEulerAngles.x, GetXRotation(), 0f);
			cachedCameraTransform.localEulerAngles = new Vector3(GetYRotation(-1f), cachedCameraTransform.localEulerAngles.y, 0f);
		}

		private float GetXRotation()
		{
			rotationX += vector2Input.Value.x * sensitivityX.Value;
			rotationX = ClampAngle(rotationX, minimumX, maximumX) % 360f;
			return rotationX;
		}

		private float GetYRotation(float invert = 1f)
		{
			rotationY += vector2Input.Value.y * invert * sensitivityY.Value;
			rotationY = ClampAngle(rotationY, minimumY, maximumY) % 360f;
			return rotationY;
		}

		private static float ClampAngle(float angle, FsmFloat min, FsmFloat max)
		{
			if (angle < 0f)
			{
				angle = 360f + angle;
			}
			float num = (min.IsNone ? (-720f) : min.Value);
			float b = (max.IsNone ? 720f : max.Value);
			if (angle > 180f)
			{
				return Mathf.Max(angle, 360f + num);
			}
			return Mathf.Min(angle, b);
		}
	}
}
