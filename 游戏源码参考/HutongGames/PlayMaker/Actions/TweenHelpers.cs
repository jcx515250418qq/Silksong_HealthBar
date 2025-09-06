using System;
using HutongGames.PlayMaker.TweenEnums;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	public static class TweenHelpers
	{
		public static Quaternion GetTargetRotation(RotationOptions option, Transform owner, Transform target, Vector3 rotation)
		{
			if (owner == null)
			{
				return Quaternion.identity;
			}
			switch (option)
			{
			case RotationOptions.CurrentRotation:
				return owner.rotation;
			case RotationOptions.WorldRotation:
				return Quaternion.Euler(rotation);
			case RotationOptions.LocalRotation:
				if (owner.parent == null)
				{
					return Quaternion.Euler(rotation);
				}
				return owner.parent.rotation * Quaternion.Euler(rotation);
			case RotationOptions.WorldOffsetRotation:
				return Quaternion.Euler(rotation) * owner.rotation;
			case RotationOptions.LocalOffsetRotation:
				return owner.rotation * Quaternion.Euler(rotation);
			case RotationOptions.MatchGameObjectRotation:
				if (target == null)
				{
					return owner.rotation;
				}
				return target.rotation * Quaternion.Euler(rotation);
			default:
				throw new ArgumentOutOfRangeException();
			}
		}

		public static bool GetTargetRotation(RotationOptions option, Transform owner, FsmVector3 rotation, FsmGameObject target, out Quaternion targetRotation)
		{
			targetRotation = Quaternion.identity;
			if (owner == null || !CanEditTargetRotation(option, rotation, target))
			{
				return false;
			}
			targetRotation = GetTargetRotation(option, owner, (target.Value != null) ? target.Value.transform : null, rotation.Value);
			return true;
		}

		private static bool CanEditTargetRotation(RotationOptions option, NamedVariable rotation, FsmGameObject target)
		{
			if (target == null)
			{
				return false;
			}
			switch (option)
			{
			case RotationOptions.CurrentRotation:
				return false;
			case RotationOptions.WorldRotation:
			case RotationOptions.LocalRotation:
			case RotationOptions.WorldOffsetRotation:
			case RotationOptions.LocalOffsetRotation:
				return !rotation.IsNone;
			case RotationOptions.MatchGameObjectRotation:
				return target.Value != null;
			default:
				throw new ArgumentOutOfRangeException();
			}
		}

		public static Vector3 GetTargetScale(ScaleOptions option, Transform owner, Transform target, Vector3 scale)
		{
			if (owner == null)
			{
				return Vector3.one;
			}
			switch (option)
			{
			case ScaleOptions.CurrentScale:
				return owner.localScale;
			case ScaleOptions.LocalScale:
				return scale;
			case ScaleOptions.MultiplyScale:
			{
				Vector3 localScale2 = owner.localScale;
				return new Vector3(localScale2.x * scale.x, localScale2.y * scale.y, localScale2.z * scale.z);
			}
			case ScaleOptions.AddToScale:
			{
				Vector3 localScale = owner.localScale;
				return new Vector3(localScale.x + scale.x, localScale.y + scale.y, localScale.z + scale.z);
			}
			case ScaleOptions.MatchGameObject:
				if (target == null)
				{
					return owner.localScale;
				}
				return target.localScale;
			default:
				return owner.localScale;
			}
		}

		public static bool GetTargetPosition(PositionOptions option, Transform owner, FsmVector3 position, FsmGameObject target, out Vector3 targetPosition)
		{
			targetPosition = Vector3.zero;
			if (owner == null || !IsValidTargetPosition(option, position, target))
			{
				return false;
			}
			targetPosition = GetTargetPosition(option, owner, (target != null && target.Value != null) ? target.Value.transform : null, position?.Value ?? Vector3.zero);
			return true;
		}

		private static bool IsValidTargetPosition(PositionOptions option, NamedVariable position, FsmGameObject target)
		{
			switch (option)
			{
			case PositionOptions.CurrentPosition:
				return true;
			case PositionOptions.WorldPosition:
			case PositionOptions.LocalPosition:
			case PositionOptions.WorldOffset:
			case PositionOptions.LocalOffset:
				return !position.IsNone;
			case PositionOptions.TargetGameObject:
				return target.Value != null;
			default:
				throw new ArgumentOutOfRangeException();
			}
		}

		public static bool CanEditTargetPosition(PositionOptions option, NamedVariable position, FsmGameObject target)
		{
			switch (option)
			{
			case PositionOptions.CurrentPosition:
				return false;
			case PositionOptions.WorldPosition:
			case PositionOptions.LocalPosition:
			case PositionOptions.WorldOffset:
			case PositionOptions.LocalOffset:
				return !position.IsNone;
			case PositionOptions.TargetGameObject:
				return target.Value != null;
			default:
				throw new ArgumentOutOfRangeException();
			}
		}

		public static Vector3 GetTargetPosition(PositionOptions option, Transform owner, Transform target, Vector3 position)
		{
			if (owner == null)
			{
				return Vector3.zero;
			}
			switch (option)
			{
			case PositionOptions.CurrentPosition:
				return owner.position;
			case PositionOptions.WorldPosition:
				return position;
			case PositionOptions.LocalPosition:
				if (owner.parent == null)
				{
					return position;
				}
				return owner.parent.TransformPoint(position);
			case PositionOptions.WorldOffset:
				return owner.position + position;
			case PositionOptions.LocalOffset:
				return owner.TransformPoint(position);
			case PositionOptions.TargetGameObject:
				if (target == null)
				{
					return owner.position;
				}
				if (position != Vector3.zero)
				{
					return target.TransformPoint(position);
				}
				return target.position;
			default:
				throw new ArgumentOutOfRangeException();
			}
		}

		public static Vector3 GetUiTargetPosition(UiPositionOptions option, RectTransform owner, Transform target, Vector3 position)
		{
			if (owner == null)
			{
				return Vector3.zero;
			}
			switch (option)
			{
			case UiPositionOptions.CurrentPosition:
				return owner.anchoredPosition3D;
			case UiPositionOptions.Position:
				return position;
			case UiPositionOptions.Offset:
				return owner.anchoredPosition3D + position;
			case UiPositionOptions.OffscreenTop:
			{
				Vector3 anchoredPosition3D = owner.anchoredPosition3D;
				Rect worldRect = GetWorldRect(owner);
				anchoredPosition3D.y += (float)Screen.height - worldRect.yMin;
				return anchoredPosition3D;
			}
			case UiPositionOptions.OffscreenBottom:
			{
				Vector3 anchoredPosition3D = owner.anchoredPosition3D;
				Rect worldRect = GetWorldRect(owner);
				anchoredPosition3D.y -= worldRect.yMax;
				return anchoredPosition3D;
			}
			case UiPositionOptions.OffscreenLeft:
			{
				Vector3 anchoredPosition3D = owner.anchoredPosition3D;
				Rect worldRect = GetWorldRect(owner);
				anchoredPosition3D.x -= worldRect.xMax;
				return anchoredPosition3D;
			}
			case UiPositionOptions.OffscreenRight:
			{
				Vector3 anchoredPosition3D = owner.anchoredPosition3D;
				Rect worldRect = GetWorldRect(owner);
				anchoredPosition3D.x += (float)Screen.width - worldRect.xMin;
				return anchoredPosition3D;
			}
			case UiPositionOptions.TargetGameObject:
				if (target == null)
				{
					return owner.anchoredPosition3D;
				}
				if (position != Vector3.zero)
				{
					return target.TransformPoint(position);
				}
				return target.position;
			default:
				throw new ArgumentOutOfRangeException("option", option, null);
			}
		}

		public static Rect GetWorldRect(RectTransform rectTransform)
		{
			Vector3[] array = new Vector3[4];
			rectTransform.GetWorldCorners(array);
			float num = Mathf.Max(array[0].y, array[1].y, array[2].y, array[3].y);
			float num2 = Mathf.Min(array[0].y, array[1].y, array[2].y, array[3].y);
			float num3 = Mathf.Max(array[0].x, array[1].x, array[2].x, array[3].x);
			float num4 = Mathf.Min(array[0].x, array[1].x, array[2].x, array[3].x);
			return new Rect(num4, num2, num3 - num4, num - num2);
		}
	}
}
