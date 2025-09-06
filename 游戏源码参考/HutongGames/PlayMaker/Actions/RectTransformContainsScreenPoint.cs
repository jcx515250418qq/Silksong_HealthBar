using UnityEngine;
using UnityEngine.EventSystems;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("RectTransform")]
	[Tooltip("Check if a RectTransform contains the screen point as seen from the given camera.")]
	public class RectTransformContainsScreenPoint : FsmStateAction
	{
		[RequiredField]
		[CheckForComponent(typeof(RectTransform))]
		[Tooltip("The GameObject target.")]
		public FsmOwnerDefault gameObject;

		[Tooltip("The screenPoint as a Vector2. Leave to none if you want to use the Vector3 alternative")]
		public FsmVector2 screenPointVector2;

		[Tooltip("The screenPoint as a Vector3. Leave to none if you want to use the Vector2 alternative")]
		public FsmVector3 orScreenPointVector3;

		[Tooltip("Define if screenPoint are expressed as normalized screen coordinates (0-1). Otherwise coordinates are in pixels.")]
		public bool normalizedScreenPoint;

		[Tooltip("The Camera. For a RectTransform in a Canvas set to Screen Space - Overlay mode, the cam parameter should be set to null explicitly (default).\nLeave to none and the camera will be the one from EventSystem.current.camera")]
		[CheckForComponent(typeof(Camera))]
		public FsmGameObject camera;

		[Tooltip("Repeat every frame")]
		public bool everyFrame;

		[ActionSection("Result")]
		[Tooltip("Store the result.")]
		[UIHint(UIHint.Variable)]
		public FsmBool isContained;

		[Tooltip("Event sent if screenPoint is contained in RectTransform.")]
		public FsmEvent isContainedEvent;

		[Tooltip("Event sent if screenPoint is NOT contained in RectTransform.")]
		public FsmEvent isNotContainedEvent;

		private RectTransform _rt;

		private Camera _camera;

		public override void Reset()
		{
			gameObject = null;
			screenPointVector2 = null;
			orScreenPointVector3 = new FsmVector3
			{
				UseVariable = true
			};
			normalizedScreenPoint = false;
			camera = null;
			everyFrame = false;
			isContained = null;
			isContainedEvent = null;
			isNotContainedEvent = null;
		}

		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(gameObject);
			if (ownerDefaultTarget != null)
			{
				_rt = ownerDefaultTarget.GetComponent<RectTransform>();
			}
			if (!camera.IsNone)
			{
				_camera = camera.Value.GetComponent<Camera>();
			}
			else
			{
				_camera = EventSystem.current.GetComponent<Camera>();
			}
			DoCheck();
			if (!everyFrame)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			DoCheck();
		}

		private void DoCheck()
		{
			if (_rt == null)
			{
				return;
			}
			Vector2 value = screenPointVector2.Value;
			if (!orScreenPointVector3.IsNone)
			{
				value.x = orScreenPointVector3.Value.x;
				value.y = orScreenPointVector3.Value.y;
			}
			if (normalizedScreenPoint)
			{
				value.x *= Screen.width;
				value.y *= Screen.height;
			}
			bool flag = RectTransformUtility.RectangleContainsScreenPoint(_rt, value, _camera);
			if (!isContained.IsNone)
			{
				isContained.Value = flag;
			}
			if (flag)
			{
				if (isContainedEvent != null)
				{
					base.Fsm.Event(isContainedEvent);
				}
			}
			else if (isNotContainedEvent != null)
			{
				base.Fsm.Event(isNotContainedEvent);
			}
		}
	}
}
