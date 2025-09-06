using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Input)]
	[Tooltip("Rotates a GameObject based on mouse movement. Minimum and Maximum values can be used to constrain the rotation.")]
	public class MouseLook : ComponentAction<Transform>
	{
		public enum RotationAxes
		{
			MouseXAndY = 0,
			MouseX = 1,
			MouseY = 2
		}

		[RequiredField]
		[Tooltip("The GameObject to rotate.")]
		public FsmOwnerDefault gameObject;

		[Tooltip("The axes to rotate around.")]
		public RotationAxes axes;

		[RequiredField]
		[Tooltip("Sensitivity of movement in X direction.")]
		public FsmFloat sensitivityX;

		[RequiredField]
		[Tooltip("Sensitivity of movement in Y direction.")]
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

		public override void Reset()
		{
			gameObject = null;
			axes = RotationAxes.MouseXAndY;
			sensitivityX = 15f;
			sensitivityY = 15f;
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
			if (!UpdateCachedTransform(base.Fsm.GetOwnerDefaultTarget(gameObject)))
			{
				Finish();
				return;
			}
			Rigidbody component = cachedGameObject.GetComponent<Rigidbody>();
			if (component != null)
			{
				component.freezeRotation = true;
			}
			rotationX = base.cachedTransform.localRotation.eulerAngles.y;
			rotationY = base.cachedTransform.localRotation.eulerAngles.x;
			if (!everyFrame)
			{
				DoMouseLook();
				Finish();
			}
		}

		public override void OnUpdate()
		{
			DoMouseLook();
		}

		private void DoMouseLook()
		{
			if (!UpdateCachedTransform(base.Fsm.GetOwnerDefaultTarget(gameObject)))
			{
				Finish();
				return;
			}
			switch (axes)
			{
			case RotationAxes.MouseXAndY:
				base.cachedTransform.localEulerAngles = new Vector3(GetYRotation(), GetXRotation(), 0f);
				break;
			case RotationAxes.MouseX:
				base.cachedTransform.localEulerAngles = new Vector3(base.cachedTransform.localEulerAngles.x, GetXRotation(), 0f);
				break;
			case RotationAxes.MouseY:
				base.cachedTransform.localEulerAngles = new Vector3(GetYRotation(-1f), base.cachedTransform.localEulerAngles.y, 0f);
				break;
			}
		}

		private float GetXRotation()
		{
			rotationX += Input.GetAxis("Mouse X") * sensitivityX.Value;
			rotationX = ClampAngle(rotationX, minimumX, maximumX) % 360f;
			return rotationX;
		}

		private float GetYRotation(float invert = 1f)
		{
			rotationY += Input.GetAxis("Mouse Y") * sensitivityY.Value * invert;
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
