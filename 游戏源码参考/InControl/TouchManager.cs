using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace InControl
{
	[ExecuteInEditMode]
	public class TouchManager : SingletonMonoBehavior<TouchManager>
	{
		public enum GizmoShowOption
		{
			Never = 0,
			WhenSelected = 1,
			UnlessPlaying = 2,
			Always = 3
		}

		[Space(10f)]
		public Camera touchCamera;

		public GizmoShowOption controlsShowGizmos = GizmoShowOption.Always;

		[HideInInspector]
		public bool enableControlsOnTouch;

		[SerializeField]
		[HideInInspector]
		private bool _controlsEnabled = true;

		[HideInInspector]
		public int controlsLayer = 5;

		private InputDevice device;

		private Vector3 viewSize;

		private Vector2 screenSize;

		private Vector2 halfScreenSize;

		private float percentToWorld;

		private float halfPercentToWorld;

		private float pixelToWorld;

		private float halfPixelToWorld;

		private TouchControl[] touchControls;

		private TouchPool cachedTouches;

		private List<Touch> activeTouches;

		private ReadOnlyCollection<Touch> readOnlyActiveTouches;

		private bool isReady;

		private readonly Touch[] mouseTouches = new Touch[3];

		public bool controlsEnabled
		{
			get
			{
				return _controlsEnabled;
			}
			set
			{
				if (_controlsEnabled != value)
				{
					int num = touchControls.Length;
					for (int i = 0; i < num; i++)
					{
						touchControls[i].enabled = value;
					}
					_controlsEnabled = value;
				}
			}
		}

		public static ReadOnlyCollection<Touch> Touches => SingletonMonoBehavior<TouchManager>.Instance.readOnlyActiveTouches;

		public static int TouchCount => SingletonMonoBehavior<TouchManager>.Instance.activeTouches.Count;

		public static Camera Camera => SingletonMonoBehavior<TouchManager>.Instance.touchCamera;

		public static InputDevice Device => SingletonMonoBehavior<TouchManager>.Instance.device;

		public static Vector3 ViewSize => SingletonMonoBehavior<TouchManager>.Instance.viewSize;

		public static float PercentToWorld => SingletonMonoBehavior<TouchManager>.Instance.percentToWorld;

		public static float HalfPercentToWorld => SingletonMonoBehavior<TouchManager>.Instance.halfPercentToWorld;

		public static float PixelToWorld => SingletonMonoBehavior<TouchManager>.Instance.pixelToWorld;

		public static float HalfPixelToWorld => SingletonMonoBehavior<TouchManager>.Instance.halfPixelToWorld;

		public static Vector2 ScreenSize => SingletonMonoBehavior<TouchManager>.Instance.screenSize;

		public static Vector2 HalfScreenSize => SingletonMonoBehavior<TouchManager>.Instance.halfScreenSize;

		public static GizmoShowOption ControlsShowGizmos => SingletonMonoBehavior<TouchManager>.Instance.controlsShowGizmos;

		public static bool ControlsEnabled
		{
			get
			{
				return SingletonMonoBehavior<TouchManager>.Instance.controlsEnabled;
			}
			set
			{
				SingletonMonoBehavior<TouchManager>.Instance.controlsEnabled = value;
			}
		}

		public static event Action OnSetup;

		protected TouchManager()
		{
		}

		private void OnEnable()
		{
			if (GetComponent<InControlManager>() == null)
			{
				Logger.LogError("Touch Manager component can only be added to the InControl Manager object.");
				UnityEngine.Object.DestroyImmediate(this);
			}
			else if (!base.EnforceSingleton)
			{
				touchControls = GetComponentsInChildren<TouchControl>(includeInactive: true);
				if (Application.isPlaying)
				{
					InputManager.OnSetup += Setup;
					InputManager.OnUpdateDevices += UpdateDevice;
					InputManager.OnCommitDevices += CommitDevice;
				}
			}
		}

		private void OnDisable()
		{
			if (Application.isPlaying)
			{
				InputManager.OnSetup -= Setup;
				InputManager.OnUpdateDevices -= UpdateDevice;
				InputManager.OnCommitDevices -= CommitDevice;
			}
			Reset();
		}

		private void Setup()
		{
			UpdateScreenSize(GetCurrentScreenSize());
			CreateDevice();
			CreateTouches();
			if (TouchManager.OnSetup != null)
			{
				TouchManager.OnSetup();
				TouchManager.OnSetup = null;
			}
		}

		private void Reset()
		{
			device = null;
			for (int i = 0; i < 3; i++)
			{
				mouseTouches[i] = null;
			}
			cachedTouches = null;
			activeTouches = null;
			readOnlyActiveTouches = null;
			touchControls = null;
			TouchManager.OnSetup = null;
		}

		private IEnumerator UpdateScreenSizeAtEndOfFrame()
		{
			yield return new WaitForEndOfFrame();
			UpdateScreenSize(GetCurrentScreenSize());
			yield return null;
		}

		private void Update()
		{
			Vector2 currentScreenSize = GetCurrentScreenSize();
			if (!isReady)
			{
				StartCoroutine(UpdateScreenSizeAtEndOfFrame());
				UpdateScreenSize(currentScreenSize);
				isReady = true;
				return;
			}
			if (screenSize != currentScreenSize)
			{
				UpdateScreenSize(currentScreenSize);
			}
			if (TouchManager.OnSetup != null)
			{
				TouchManager.OnSetup();
				TouchManager.OnSetup = null;
			}
		}

		private void CreateDevice()
		{
			device = new TouchInputDevice();
			device.AddControl(InputControlType.LeftStickLeft, "LeftStickLeft");
			device.AddControl(InputControlType.LeftStickRight, "LeftStickRight");
			device.AddControl(InputControlType.LeftStickUp, "LeftStickUp");
			device.AddControl(InputControlType.LeftStickDown, "LeftStickDown");
			device.AddControl(InputControlType.RightStickLeft, "RightStickLeft");
			device.AddControl(InputControlType.RightStickRight, "RightStickRight");
			device.AddControl(InputControlType.RightStickUp, "RightStickUp");
			device.AddControl(InputControlType.RightStickDown, "RightStickDown");
			device.AddControl(InputControlType.DPadUp, "DPadUp");
			device.AddControl(InputControlType.DPadDown, "DPadDown");
			device.AddControl(InputControlType.DPadLeft, "DPadLeft");
			device.AddControl(InputControlType.DPadRight, "DPadRight");
			device.AddControl(InputControlType.LeftTrigger, "LeftTrigger");
			device.AddControl(InputControlType.RightTrigger, "RightTrigger");
			device.AddControl(InputControlType.LeftBumper, "LeftBumper");
			device.AddControl(InputControlType.RightBumper, "RightBumper");
			for (InputControlType inputControlType = InputControlType.Action1; inputControlType <= InputControlType.Action12; inputControlType++)
			{
				device.AddControl(inputControlType, inputControlType.ToString());
			}
			device.AddControl(InputControlType.Menu, "Menu");
			for (InputControlType inputControlType2 = InputControlType.Button0; inputControlType2 <= InputControlType.Button19; inputControlType2++)
			{
				device.AddControl(inputControlType2, inputControlType2.ToString());
			}
			InputManager.AttachDevice(device);
		}

		private void UpdateDevice(ulong updateTick, float deltaTime)
		{
			UpdateTouches(updateTick, deltaTime);
			SubmitControlStates(updateTick, deltaTime);
		}

		private void CommitDevice(ulong updateTick, float deltaTime)
		{
			CommitControlStates(updateTick, deltaTime);
		}

		private void SubmitControlStates(ulong updateTick, float deltaTime)
		{
			int num = touchControls.Length;
			for (int i = 0; i < num; i++)
			{
				TouchControl touchControl = touchControls[i];
				if (touchControl.enabled && touchControl.gameObject.activeInHierarchy)
				{
					touchControl.SubmitControlState(updateTick, deltaTime);
				}
			}
		}

		private void CommitControlStates(ulong updateTick, float deltaTime)
		{
			int num = touchControls.Length;
			for (int i = 0; i < num; i++)
			{
				TouchControl touchControl = touchControls[i];
				if (touchControl.enabled && touchControl.gameObject.activeInHierarchy)
				{
					touchControl.CommitControlState(updateTick, deltaTime);
				}
			}
		}

		private void UpdateScreenSize(Vector2 currentScreenSize)
		{
			touchCamera.rect = new Rect(0f, 0f, 0.99f, 1f);
			touchCamera.rect = new Rect(0f, 0f, 1f, 1f);
			screenSize = currentScreenSize;
			halfScreenSize = screenSize / 2f;
			viewSize = ConvertViewToWorldPoint(Vector2.one) * 0.02f;
			percentToWorld = Mathf.Min(viewSize.x, viewSize.y);
			halfPercentToWorld = percentToWorld / 2f;
			if (touchCamera != null)
			{
				halfPixelToWorld = touchCamera.orthographicSize / screenSize.y;
				pixelToWorld = halfPixelToWorld * 2f;
			}
			if (touchControls != null)
			{
				int num = touchControls.Length;
				for (int i = 0; i < num; i++)
				{
					touchControls[i].ConfigureControl();
				}
			}
		}

		private void CreateTouches()
		{
			cachedTouches = new TouchPool();
			for (int i = 0; i < 3; i++)
			{
				mouseTouches[i] = new Touch();
				mouseTouches[i].fingerId = -2;
			}
			activeTouches = new List<Touch>(32);
			readOnlyActiveTouches = new ReadOnlyCollection<Touch>(activeTouches);
		}

		private void UpdateTouches(ulong updateTick, float deltaTime)
		{
			activeTouches.Clear();
			cachedTouches.FreeEndedTouches();
			for (int i = 0; i < 3; i++)
			{
				if (mouseTouches[i].SetWithMouseData(i, updateTick, deltaTime))
				{
					activeTouches.Add(mouseTouches[i]);
				}
			}
			for (int j = 0; j < Input.touchCount; j++)
			{
				UnityEngine.Touch touch = Input.GetTouch(j);
				Touch touch2 = cachedTouches.FindOrCreateTouch(touch.fingerId);
				touch2.SetWithTouchData(touch, updateTick, deltaTime);
				activeTouches.Add(touch2);
			}
			int count = cachedTouches.Touches.Count;
			for (int k = 0; k < count; k++)
			{
				Touch touch3 = cachedTouches.Touches[k];
				if (touch3.phase != TouchPhase.Ended && touch3.updateTick != updateTick)
				{
					touch3.phase = TouchPhase.Ended;
					activeTouches.Add(touch3);
				}
			}
			InvokeTouchEvents();
		}

		private void SendTouchBegan(Touch touch)
		{
			int num = touchControls.Length;
			for (int i = 0; i < num; i++)
			{
				TouchControl touchControl = touchControls[i];
				if (touchControl.enabled && touchControl.gameObject.activeInHierarchy)
				{
					touchControl.TouchBegan(touch);
				}
			}
		}

		private void SendTouchMoved(Touch touch)
		{
			int num = touchControls.Length;
			for (int i = 0; i < num; i++)
			{
				TouchControl touchControl = touchControls[i];
				if (touchControl.enabled && touchControl.gameObject.activeInHierarchy)
				{
					touchControl.TouchMoved(touch);
				}
			}
		}

		private void SendTouchEnded(Touch touch)
		{
			int num = touchControls.Length;
			for (int i = 0; i < num; i++)
			{
				TouchControl touchControl = touchControls[i];
				if (touchControl.enabled && touchControl.gameObject.activeInHierarchy)
				{
					touchControl.TouchEnded(touch);
				}
			}
		}

		private void InvokeTouchEvents()
		{
			int count = activeTouches.Count;
			if (enableControlsOnTouch && count > 0 && !controlsEnabled)
			{
				Device.RequestActivation();
				controlsEnabled = true;
			}
			for (int i = 0; i < count; i++)
			{
				Touch touch = activeTouches[i];
				switch (touch.phase)
				{
				case TouchPhase.Began:
					SendTouchBegan(touch);
					break;
				case TouchPhase.Moved:
					SendTouchMoved(touch);
					break;
				case TouchPhase.Ended:
					SendTouchEnded(touch);
					break;
				case TouchPhase.Canceled:
					SendTouchEnded(touch);
					break;
				default:
					throw new ArgumentOutOfRangeException();
				case TouchPhase.Stationary:
					break;
				}
			}
		}

		private bool TouchCameraIsValid()
		{
			if (touchCamera == null)
			{
				return false;
			}
			if (Utility.IsZero(touchCamera.orthographicSize))
			{
				return false;
			}
			if (Utility.IsZero(touchCamera.rect.width) && Utility.IsZero(touchCamera.rect.height))
			{
				return false;
			}
			if (Utility.IsZero(touchCamera.pixelRect.width) && Utility.IsZero(touchCamera.pixelRect.height))
			{
				return false;
			}
			return true;
		}

		private Vector3 ConvertScreenToWorldPoint(Vector2 point)
		{
			if (TouchCameraIsValid())
			{
				return touchCamera.ScreenToWorldPoint(new Vector3(point.x, point.y, 0f - touchCamera.transform.position.z));
			}
			return Vector3.zero;
		}

		private Vector3 ConvertViewToWorldPoint(Vector2 point)
		{
			if (TouchCameraIsValid())
			{
				return touchCamera.ViewportToWorldPoint(new Vector3(point.x, point.y, 0f - touchCamera.transform.position.z));
			}
			return Vector3.zero;
		}

		private Vector3 ConvertScreenToViewPoint(Vector2 point)
		{
			if (TouchCameraIsValid())
			{
				return touchCamera.ScreenToViewportPoint(new Vector3(point.x, point.y, 0f - touchCamera.transform.position.z));
			}
			return Vector3.zero;
		}

		private Vector2 GetCurrentScreenSize()
		{
			if (TouchCameraIsValid())
			{
				return new Vector2(touchCamera.pixelWidth, touchCamera.pixelHeight);
			}
			return new Vector2(Screen.width, Screen.height);
		}

		public static Touch GetTouch(int touchIndex)
		{
			return SingletonMonoBehavior<TouchManager>.Instance.activeTouches[touchIndex];
		}

		public static Touch GetTouchByFingerId(int fingerId)
		{
			return SingletonMonoBehavior<TouchManager>.Instance.cachedTouches.FindTouch(fingerId);
		}

		public static Vector3 ScreenToWorldPoint(Vector2 point)
		{
			return SingletonMonoBehavior<TouchManager>.Instance.ConvertScreenToWorldPoint(point);
		}

		public static Vector3 ViewToWorldPoint(Vector2 point)
		{
			return SingletonMonoBehavior<TouchManager>.Instance.ConvertViewToWorldPoint(point);
		}

		public static Vector3 ScreenToViewPoint(Vector2 point)
		{
			return SingletonMonoBehavior<TouchManager>.Instance.ConvertScreenToViewPoint(point);
		}

		public static float ConvertToWorld(float value, TouchUnitType unitType)
		{
			return value * ((unitType == TouchUnitType.Pixels) ? PixelToWorld : PercentToWorld);
		}

		public static Rect PercentToWorldRect(Rect rect)
		{
			return new Rect((rect.xMin - 50f) * ViewSize.x, (rect.yMin - 50f) * ViewSize.y, rect.width * ViewSize.x, rect.height * ViewSize.y);
		}

		public static Rect PixelToWorldRect(Rect rect)
		{
			return new Rect(Mathf.Round(rect.xMin - HalfScreenSize.x) * PixelToWorld, Mathf.Round(rect.yMin - HalfScreenSize.y) * PixelToWorld, Mathf.Round(rect.width) * PixelToWorld, Mathf.Round(rect.height) * PixelToWorld);
		}

		public static Rect ConvertToWorld(Rect rect, TouchUnitType unitType)
		{
			if (unitType != TouchUnitType.Pixels)
			{
				return PercentToWorldRect(rect);
			}
			return PixelToWorldRect(rect);
		}

		public static implicit operator bool(TouchManager instance)
		{
			return instance != null;
		}
	}
}
