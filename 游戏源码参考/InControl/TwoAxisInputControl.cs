using UnityEngine;

namespace InControl
{
	public class TwoAxisInputControl : IInputControl
	{
		public static readonly TwoAxisInputControl Null = new TwoAxisInputControl();

		public DeadZoneFunc DeadZoneFunc = DeadZone.Circular;

		private float sensitivity = 1f;

		private float lowerDeadZone;

		private float upperDeadZone = 1f;

		private float stateThreshold;

		public bool Raw;

		private bool thisState;

		private bool lastState;

		private Vector2 thisValue;

		private Vector2 lastValue;

		private bool clearInputState;

		public float X { get; protected set; }

		public float Y { get; protected set; }

		public OneAxisInputControl Left { get; protected set; }

		public OneAxisInputControl Right { get; protected set; }

		public OneAxisInputControl Up { get; protected set; }

		public OneAxisInputControl Down { get; protected set; }

		public ulong UpdateTick { get; protected set; }

		public float Sensitivity
		{
			get
			{
				return sensitivity;
			}
			set
			{
				sensitivity = Mathf.Clamp01(value);
				Left.Sensitivity = sensitivity;
				Right.Sensitivity = sensitivity;
				Up.Sensitivity = sensitivity;
				Down.Sensitivity = sensitivity;
			}
		}

		public float StateThreshold
		{
			get
			{
				return stateThreshold;
			}
			set
			{
				stateThreshold = Mathf.Clamp01(value);
				Left.StateThreshold = stateThreshold;
				Right.StateThreshold = stateThreshold;
				Up.StateThreshold = stateThreshold;
				Down.StateThreshold = stateThreshold;
			}
		}

		public float LowerDeadZone
		{
			get
			{
				return lowerDeadZone;
			}
			set
			{
				lowerDeadZone = Mathf.Clamp01(value);
				Left.LowerDeadZone = lowerDeadZone;
				Right.LowerDeadZone = lowerDeadZone;
				Up.LowerDeadZone = lowerDeadZone;
				Down.LowerDeadZone = lowerDeadZone;
			}
		}

		public float UpperDeadZone
		{
			get
			{
				return upperDeadZone;
			}
			set
			{
				upperDeadZone = Mathf.Clamp01(value);
				Left.UpperDeadZone = upperDeadZone;
				Right.UpperDeadZone = upperDeadZone;
				Up.UpperDeadZone = upperDeadZone;
				Down.UpperDeadZone = upperDeadZone;
			}
		}

		public bool State => thisState;

		public bool LastState => lastState;

		public Vector2 Value => thisValue;

		public Vector2 LastValue => lastValue;

		public Vector2 Vector => thisValue;

		public bool HasChanged { get; protected set; }

		public bool IsPressed => thisState;

		public bool WasPressed
		{
			get
			{
				if (thisState)
				{
					return !lastState;
				}
				return false;
			}
		}

		public bool WasReleased
		{
			get
			{
				if (!thisState)
				{
					return lastState;
				}
				return false;
			}
		}

		public float Angle => Utility.VectorToAngle(thisValue);

		public TwoAxisInputControl()
		{
			Left = new OneAxisInputControl();
			Right = new OneAxisInputControl();
			Up = new OneAxisInputControl();
			Down = new OneAxisInputControl();
		}

		public void ClearInputState()
		{
			Left.ClearInputState();
			Right.ClearInputState();
			Up.ClearInputState();
			Down.ClearInputState();
			lastState = false;
			lastValue = Vector2.zero;
			thisState = false;
			thisValue = Vector2.zero;
			X = 0f;
			Y = 0f;
			clearInputState = true;
		}

		public void Filter(TwoAxisInputControl twoAxisInputControl, float deltaTime)
		{
			UpdateWithAxes(twoAxisInputControl.X, twoAxisInputControl.Y, InputManager.CurrentTick, deltaTime);
		}

		internal void UpdateWithAxes(float x, float y, ulong updateTick, float deltaTime)
		{
			lastState = thisState;
			lastValue = thisValue;
			thisValue = (Raw ? new Vector2(x, y) : DeadZoneFunc(x, y, LowerDeadZone, UpperDeadZone));
			X = thisValue.x;
			Y = thisValue.y;
			Left.CommitWithValue(Mathf.Max(0f, 0f - X), updateTick, deltaTime);
			Right.CommitWithValue(Mathf.Max(0f, X), updateTick, deltaTime);
			if (InputManager.InvertYAxis)
			{
				Up.CommitWithValue(Mathf.Max(0f, 0f - Y), updateTick, deltaTime);
				Down.CommitWithValue(Mathf.Max(0f, Y), updateTick, deltaTime);
			}
			else
			{
				Up.CommitWithValue(Mathf.Max(0f, Y), updateTick, deltaTime);
				Down.CommitWithValue(Mathf.Max(0f, 0f - Y), updateTick, deltaTime);
			}
			thisState = Up.State || Down.State || Left.State || Right.State;
			if (clearInputState)
			{
				lastState = thisState;
				lastValue = thisValue;
				clearInputState = false;
				HasChanged = false;
			}
			else if (thisValue != lastValue)
			{
				UpdateTick = updateTick;
				HasChanged = true;
			}
			else
			{
				HasChanged = false;
			}
		}

		public static implicit operator bool(TwoAxisInputControl instance)
		{
			return instance.thisState;
		}

		public static implicit operator Vector2(TwoAxisInputControl instance)
		{
			return instance.thisValue;
		}

		public static implicit operator Vector3(TwoAxisInputControl instance)
		{
			return instance.thisValue;
		}
	}
}
