using UnityEngine;

namespace InControl
{
	public abstract class TouchControl : MonoBehaviour
	{
		public enum ButtonTarget
		{
			None = 0,
			DPadDown = 12,
			DPadLeft = 13,
			DPadRight = 14,
			DPadUp = 11,
			LeftTrigger = 15,
			RightTrigger = 16,
			LeftBumper = 17,
			RightBumper = 18,
			Action1 = 19,
			Action2 = 20,
			Action3 = 21,
			Action4 = 22,
			Action5 = 23,
			Action6 = 24,
			Action7 = 25,
			Action8 = 26,
			Action9 = 27,
			Action10 = 28,
			Action11 = 29,
			Action12 = 30,
			Menu = 106,
			Button0 = 500,
			Button1 = 501,
			Button2 = 502,
			Button3 = 503,
			Button4 = 504,
			Button5 = 505,
			Button6 = 506,
			Button7 = 507,
			Button8 = 508,
			Button9 = 509,
			Button10 = 510,
			Button11 = 511,
			Button12 = 512,
			Button13 = 513,
			Button14 = 514,
			Button15 = 515,
			Button16 = 516,
			Button17 = 517,
			Button18 = 518,
			Button19 = 519
		}

		public enum AnalogTarget
		{
			None = 0,
			LeftStick = 1,
			RightStick = 2,
			Both = 3
		}

		public enum SnapAngles
		{
			None = 0,
			Four = 4,
			Eight = 8,
			Sixteen = 0x10
		}

		public abstract void CreateControl();

		public abstract void DestroyControl();

		public abstract void ConfigureControl();

		public abstract void SubmitControlState(ulong updateTick, float deltaTime);

		public abstract void CommitControlState(ulong updateTick, float deltaTime);

		public abstract void TouchBegan(Touch touch);

		public abstract void TouchMoved(Touch touch);

		public abstract void TouchEnded(Touch touch);

		public abstract void DrawGizmos();

		private void OnEnable()
		{
			TouchManager.OnSetup += Setup;
		}

		private void OnDisable()
		{
			DestroyControl();
			Resources.UnloadUnusedAssets();
		}

		private void Setup()
		{
			if (base.enabled)
			{
				CreateControl();
				ConfigureControl();
			}
		}

		protected Vector3 OffsetToWorldPosition(TouchControlAnchor anchor, Vector2 offset, TouchUnitType offsetUnitType, bool lockAspectRatio)
		{
			Vector3 vector = ((offsetUnitType == TouchUnitType.Pixels) ? ((Vector3)(TouchUtility.RoundVector(offset) * TouchManager.PixelToWorld)) : ((!lockAspectRatio) ? Vector3.Scale(offset, TouchManager.ViewSize) : ((Vector3)offset * TouchManager.PercentToWorld)));
			return TouchManager.ViewToWorldPoint(TouchUtility.AnchorToViewPoint(anchor)) + vector;
		}

		protected void SubmitButtonState(ButtonTarget target, bool state, ulong updateTick, float deltaTime)
		{
			if (TouchManager.Device != null && target != 0)
			{
				InputControl control = TouchManager.Device.GetControl((InputControlType)target);
				if (control != null && control != InputControl.Null)
				{
					control.UpdateWithState(state, updateTick, deltaTime);
				}
			}
		}

		protected void SubmitButtonValue(ButtonTarget target, float value, ulong updateTick, float deltaTime)
		{
			if (TouchManager.Device != null && target != 0)
			{
				InputControl control = TouchManager.Device.GetControl((InputControlType)target);
				if (control != null && control != InputControl.Null)
				{
					control.UpdateWithValue(value, updateTick, deltaTime);
				}
			}
		}

		protected void CommitButton(ButtonTarget target)
		{
			if (TouchManager.Device != null && target != 0)
			{
				InputControl control = TouchManager.Device.GetControl((InputControlType)target);
				if (control != null && control != InputControl.Null)
				{
					control.Commit();
				}
			}
		}

		protected void SubmitAnalogValue(AnalogTarget target, Vector2 value, float lowerDeadZone, float upperDeadZone, ulong updateTick, float deltaTime)
		{
			if (TouchManager.Device != null && target != 0)
			{
				Vector2 value2 = DeadZone.Circular(value.x, value.y, lowerDeadZone, upperDeadZone);
				if (target == AnalogTarget.LeftStick || target == AnalogTarget.Both)
				{
					TouchManager.Device.UpdateLeftStickWithValue(value2, updateTick, deltaTime);
				}
				if (target == AnalogTarget.RightStick || target == AnalogTarget.Both)
				{
					TouchManager.Device.UpdateRightStickWithValue(value2, updateTick, deltaTime);
				}
			}
		}

		protected void CommitAnalog(AnalogTarget target)
		{
			if (TouchManager.Device != null)
			{
				switch (target)
				{
				case AnalogTarget.None:
					return;
				case AnalogTarget.LeftStick:
				case AnalogTarget.Both:
					TouchManager.Device.CommitLeftStick();
					break;
				}
				if (target == AnalogTarget.RightStick || target == AnalogTarget.Both)
				{
					TouchManager.Device.CommitRightStick();
				}
			}
		}

		protected void SubmitRawAnalogValue(AnalogTarget target, Vector2 rawValue, ulong updateTick, float deltaTime)
		{
			if (TouchManager.Device != null)
			{
				switch (target)
				{
				case AnalogTarget.None:
					return;
				case AnalogTarget.LeftStick:
				case AnalogTarget.Both:
					TouchManager.Device.UpdateLeftStickWithRawValue(rawValue, updateTick, deltaTime);
					break;
				}
				if (target == AnalogTarget.RightStick || target == AnalogTarget.Both)
				{
					TouchManager.Device.UpdateRightStickWithRawValue(rawValue, updateTick, deltaTime);
				}
			}
		}

		protected static Vector3 SnapTo(Vector2 vector, SnapAngles snapAngles)
		{
			if (snapAngles == SnapAngles.None)
			{
				return vector;
			}
			float snapAngle = 360f / (float)snapAngles;
			return SnapTo(vector, snapAngle);
		}

		protected static Vector3 SnapTo(Vector2 vector, float snapAngle)
		{
			float num = Vector2.Angle(vector, Vector2.up);
			if (num < snapAngle / 2f)
			{
				return Vector2.up * vector.magnitude;
			}
			if (num > 180f - snapAngle / 2f)
			{
				return -Vector2.up * vector.magnitude;
			}
			float angle = Mathf.Round(num / snapAngle) * snapAngle - num;
			Vector3 axis = Vector3.Cross(Vector2.up, vector);
			return Quaternion.AngleAxis(angle, axis) * vector;
		}

		private void OnDrawGizmosSelected()
		{
			if (base.enabled && TouchManager.ControlsShowGizmos == TouchManager.GizmoShowOption.WhenSelected && !Utility.GameObjectIsCulledOnCurrentCamera(base.gameObject))
			{
				if (!Application.isPlaying)
				{
					ConfigureControl();
				}
				DrawGizmos();
			}
		}

		private void OnDrawGizmos()
		{
			if (!base.enabled)
			{
				return;
			}
			if (TouchManager.ControlsShowGizmos == TouchManager.GizmoShowOption.UnlessPlaying)
			{
				if (Application.isPlaying)
				{
					return;
				}
			}
			else if (TouchManager.ControlsShowGizmos != TouchManager.GizmoShowOption.Always)
			{
				return;
			}
			if (!Utility.GameObjectIsCulledOnCurrentCamera(base.gameObject))
			{
				if (!Application.isPlaying)
				{
					ConfigureControl();
				}
				DrawGizmos();
			}
		}
	}
}
