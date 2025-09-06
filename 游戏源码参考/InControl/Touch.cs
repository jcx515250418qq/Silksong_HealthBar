using System;
using UnityEngine;

namespace InControl
{
	public class Touch
	{
		public const int FingerID_None = -1;

		public const int FingerID_Mouse = -2;

		public int fingerId;

		public int mouseButton;

		public TouchPhase phase;

		public int tapCount;

		public Vector2 position;

		public Vector2 startPosition;

		public Vector2 deltaPosition;

		public Vector2 lastPosition;

		public float deltaTime;

		public ulong updateTick;

		public TouchType type;

		public float altitudeAngle;

		public float azimuthAngle;

		public float maximumPossiblePressure;

		public float pressure;

		public float radius;

		public float radiusVariance;

		[Obsolete("normalizedPressure is deprecated, please use NormalizedPressure instead.")]
		public float normalizedPressure => Mathf.Clamp(pressure / maximumPossiblePressure, 0.001f, 1f);

		public float NormalizedPressure => Mathf.Clamp(pressure / maximumPossiblePressure, 0.001f, 1f);

		public bool IsMouse => type == TouchType.Mouse;

		internal Touch()
		{
			fingerId = -1;
			phase = TouchPhase.Ended;
		}

		internal void Reset()
		{
			fingerId = -1;
			mouseButton = 0;
			phase = TouchPhase.Ended;
			tapCount = 0;
			position = Vector2.zero;
			startPosition = Vector2.zero;
			deltaPosition = Vector2.zero;
			lastPosition = Vector2.zero;
			deltaTime = 0f;
			updateTick = 0uL;
			type = TouchType.Direct;
			altitudeAngle = 0f;
			azimuthAngle = 0f;
			maximumPossiblePressure = 1f;
			pressure = 0f;
			radius = 0f;
			radiusVariance = 0f;
		}

		internal void SetWithTouchData(UnityEngine.Touch touch, ulong updateTick, float deltaTime)
		{
			phase = touch.phase;
			tapCount = touch.tapCount;
			mouseButton = 0;
			altitudeAngle = touch.altitudeAngle;
			azimuthAngle = touch.azimuthAngle;
			maximumPossiblePressure = touch.maximumPossiblePressure;
			pressure = touch.pressure;
			radius = touch.radius;
			radiusVariance = touch.radiusVariance;
			Vector2 vector = touch.position;
			vector.x = Mathf.Clamp(vector.x, 0f, Screen.width);
			vector.y = Mathf.Clamp(vector.y, 0f, Screen.height);
			if (phase == TouchPhase.Began)
			{
				startPosition = vector;
				deltaPosition = Vector2.zero;
				lastPosition = vector;
				position = vector;
			}
			else
			{
				if (phase == TouchPhase.Stationary)
				{
					phase = TouchPhase.Moved;
				}
				deltaPosition = vector - lastPosition;
				lastPosition = position;
				position = vector;
			}
			this.deltaTime = deltaTime;
			this.updateTick = updateTick;
		}

		internal bool SetWithMouseData(int button, ulong updateTick, float deltaTime)
		{
			if (Input.touchCount > 0)
			{
				return false;
			}
			if (button < 0 || button > 2)
			{
				return false;
			}
			Vector2 vector = InputManager.MouseProvider.GetPosition();
			Vector2 vector2 = new Vector2(Mathf.Round(vector.x), Mathf.Round(vector.y));
			Mouse control = (Mouse)(1 + button);
			if (InputManager.MouseProvider.GetButtonWasPressed(control))
			{
				phase = TouchPhase.Began;
				pressure = 1f;
				maximumPossiblePressure = 1f;
				tapCount = 1;
				type = TouchType.Mouse;
				mouseButton = button;
				startPosition = vector2;
				deltaPosition = Vector2.zero;
				lastPosition = vector2;
				position = vector2;
				this.deltaTime = deltaTime;
				this.updateTick = updateTick;
				return true;
			}
			if (InputManager.MouseProvider.GetButtonWasReleased(control))
			{
				phase = TouchPhase.Ended;
				pressure = 0f;
				maximumPossiblePressure = 1f;
				tapCount = 1;
				type = TouchType.Mouse;
				mouseButton = button;
				deltaPosition = vector2 - lastPosition;
				lastPosition = position;
				position = vector2;
				this.deltaTime = deltaTime;
				this.updateTick = updateTick;
				return true;
			}
			if (InputManager.MouseProvider.GetButtonIsPressed(control))
			{
				phase = TouchPhase.Moved;
				pressure = 1f;
				maximumPossiblePressure = 1f;
				tapCount = 1;
				type = TouchType.Mouse;
				mouseButton = button;
				deltaPosition = vector2 - lastPosition;
				lastPosition = position;
				position = vector2;
				this.deltaTime = deltaTime;
				this.updateTick = updateTick;
				return true;
			}
			return false;
		}
	}
}
