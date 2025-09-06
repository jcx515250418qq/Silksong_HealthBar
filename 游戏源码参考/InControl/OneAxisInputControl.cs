using System;
using UnityEngine;

namespace InControl
{
	public class OneAxisInputControl : IInputControl
	{
		private float sensitivity = 1f;

		private float lowerDeadZone;

		private float upperDeadZone = 1f;

		private float stateThreshold;

		protected bool isNullControl;

		public float FirstRepeatDelay = 0.8f;

		public float RepeatDelay = 0.1f;

		public bool Raw;

		private bool enabled = true;

		protected bool ownerEnabled = true;

		private ulong pendingTick;

		private bool pendingCommit;

		private float nextRepeatTime;

		private bool wasRepeated;

		private bool clearInputState;

		private InputControlState lastState;

		private InputControlState nextState;

		private InputControlState thisState;

		public ulong UpdateTick { get; protected set; }

		public bool State
		{
			get
			{
				if (EnabledInHierarchy)
				{
					return thisState.State;
				}
				return false;
			}
		}

		public bool LastState
		{
			get
			{
				if (EnabledInHierarchy)
				{
					return lastState.State;
				}
				return false;
			}
		}

		public float Value
		{
			get
			{
				if (!EnabledInHierarchy)
				{
					return 0f;
				}
				return thisState.Value;
			}
		}

		public float LastValue
		{
			get
			{
				if (!EnabledInHierarchy)
				{
					return 0f;
				}
				return lastState.Value;
			}
		}

		public float RawValue
		{
			get
			{
				if (!EnabledInHierarchy)
				{
					return 0f;
				}
				return thisState.RawValue;
			}
		}

		internal float NextRawValue
		{
			get
			{
				if (!EnabledInHierarchy)
				{
					return 0f;
				}
				return nextState.RawValue;
			}
		}

		internal bool HasInput
		{
			get
			{
				if (EnabledInHierarchy)
				{
					return Utility.IsNotZero(thisState.Value);
				}
				return false;
			}
		}

		public bool HasChanged
		{
			get
			{
				if (EnabledInHierarchy)
				{
					return thisState != lastState;
				}
				return false;
			}
		}

		public bool IsPressed
		{
			get
			{
				if (EnabledInHierarchy)
				{
					return thisState.State;
				}
				return false;
			}
		}

		public bool WasPressed
		{
			get
			{
				if (EnabledInHierarchy && (bool)thisState)
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
				if (EnabledInHierarchy && !thisState)
				{
					return lastState;
				}
				return false;
			}
		}

		public bool WasRepeated
		{
			get
			{
				if (EnabledInHierarchy)
				{
					return wasRepeated;
				}
				return false;
			}
		}

		public float Sensitivity
		{
			get
			{
				return sensitivity;
			}
			set
			{
				sensitivity = Mathf.Clamp01(value);
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
			}
		}

		public bool IsNullControl => isNullControl;

		public bool Enabled
		{
			get
			{
				return enabled;
			}
			set
			{
				enabled = value;
			}
		}

		public bool EnabledInHierarchy
		{
			get
			{
				if (enabled)
				{
					return ownerEnabled;
				}
				return false;
			}
		}

		private void PrepareForUpdate(ulong updateTick)
		{
			if (!isNullControl)
			{
				if (updateTick < pendingTick)
				{
					throw new InvalidOperationException("Cannot be updated with an earlier tick.");
				}
				if (pendingCommit && updateTick != pendingTick)
				{
					throw new InvalidOperationException("Cannot be updated for a new tick until pending tick is committed.");
				}
				if (updateTick > pendingTick)
				{
					lastState = thisState;
					nextState.Reset();
					pendingTick = updateTick;
					pendingCommit = true;
				}
			}
		}

		public bool UpdateWithState(bool state, ulong updateTick, float deltaTime)
		{
			if (isNullControl)
			{
				return false;
			}
			PrepareForUpdate(updateTick);
			nextState.Set(state || nextState.State);
			return state;
		}

		public bool UpdateWithValue(float value, ulong updateTick, float deltaTime)
		{
			if (isNullControl)
			{
				return false;
			}
			PrepareForUpdate(updateTick);
			if (Utility.Abs(value) > Utility.Abs(nextState.RawValue))
			{
				nextState.RawValue = value;
				if (!Raw)
				{
					value = Utility.ApplyDeadZone(value, lowerDeadZone, upperDeadZone);
				}
				nextState.Set(value, stateThreshold);
				return true;
			}
			return false;
		}

		internal bool UpdateWithRawValue(float value, ulong updateTick, float deltaTime)
		{
			if (isNullControl)
			{
				return false;
			}
			Raw = true;
			PrepareForUpdate(updateTick);
			if (Utility.Abs(value) > Utility.Abs(nextState.RawValue))
			{
				nextState.RawValue = value;
				nextState.Set(value, stateThreshold);
				return true;
			}
			return false;
		}

		internal void SetValue(float value, ulong updateTick)
		{
			if (!isNullControl)
			{
				if (updateTick > pendingTick)
				{
					lastState = thisState;
					nextState.Reset();
					pendingTick = updateTick;
					pendingCommit = true;
				}
				nextState.RawValue = value;
				nextState.Set(value, StateThreshold);
			}
		}

		public void ClearInputState()
		{
			lastState.Reset();
			thisState.Reset();
			nextState.Reset();
			wasRepeated = false;
			clearInputState = true;
		}

		public void Commit()
		{
			if (isNullControl)
			{
				return;
			}
			pendingCommit = false;
			thisState = nextState;
			if (clearInputState)
			{
				lastState = nextState;
				UpdateTick = pendingTick;
				clearInputState = false;
				return;
			}
			bool state = lastState.State;
			bool state2 = thisState.State;
			wasRepeated = false;
			if (state && !state2)
			{
				nextRepeatTime = 0f;
			}
			else if (state2)
			{
				float realtimeSinceStartup = Time.realtimeSinceStartup;
				if (!state)
				{
					nextRepeatTime = realtimeSinceStartup + FirstRepeatDelay;
				}
				else if (realtimeSinceStartup >= nextRepeatTime)
				{
					wasRepeated = true;
					nextRepeatTime = realtimeSinceStartup + RepeatDelay;
				}
			}
			if (thisState != lastState)
			{
				UpdateTick = pendingTick;
			}
		}

		public void CommitWithState(bool state, ulong updateTick, float deltaTime)
		{
			UpdateWithState(state, updateTick, deltaTime);
			Commit();
		}

		public void CommitWithValue(float value, ulong updateTick, float deltaTime)
		{
			UpdateWithValue(value, updateTick, deltaTime);
			Commit();
		}

		internal void CommitWithSides(InputControl negativeSide, InputControl positiveSide, ulong updateTick, float deltaTime)
		{
			LowerDeadZone = Mathf.Max(negativeSide.LowerDeadZone, positiveSide.LowerDeadZone);
			UpperDeadZone = Mathf.Min(negativeSide.UpperDeadZone, positiveSide.UpperDeadZone);
			Raw = negativeSide.Raw || positiveSide.Raw;
			float value = Utility.ValueFromSides(negativeSide.RawValue, positiveSide.RawValue);
			CommitWithValue(value, updateTick, deltaTime);
		}

		public static implicit operator bool(OneAxisInputControl instance)
		{
			return instance.State;
		}

		public static implicit operator float(OneAxisInputControl instance)
		{
			return instance.Value;
		}
	}
}
