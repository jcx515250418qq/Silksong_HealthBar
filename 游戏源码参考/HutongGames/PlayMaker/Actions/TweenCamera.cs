using System;
using HutongGames.Extensions;
using HutongGames.PlayMaker.TweenEnums;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Tween)]
	[ActionTarget(typeof(Camera), "", false)]
	[Tooltip("Tween common Camera properties.")]
	public class TweenCamera : TweenComponentBase<Camera>
	{
		public enum CameraProperty
		{
			Aspect = 0,
			BackgroundColor = 1,
			FieldOfView = 2,
			OrthoSize = 3,
			PixelRect = 4,
			ViewportRect = 5
		}

		[Tooltip("Camera property to tween.")]
		public CameraProperty property;

		[Tooltip("Tween To/From values set below.")]
		public TweenDirection tweenDirection;

		[Tooltip("Context sensitive parameter. Depends on Property.")]
		public FsmColor targetColor;

		[Tooltip("Context sensitive parameter. Depends on Property.")]
		public FsmFloat targetFloat;

		[Tooltip("Context sensitive parameter. Depends on Property.")]
		public FsmRect targetRect;

		private Camera camera;

		private Color fromColor;

		private Color toColor;

		private float fromFloat;

		private float toFloat;

		private Rect fromRect;

		private Rect toRect;

		public override void Reset()
		{
			base.Reset();
			property = CameraProperty.FieldOfView;
			tweenDirection = TweenDirection.To;
			targetColor = null;
			targetFloat = null;
		}

		public override void OnEnter()
		{
			base.OnEnter();
			if (base.Finished)
			{
				return;
			}
			camera = cachedComponent;
			if (tweenDirection == TweenDirection.From)
			{
				switch (property)
				{
				case CameraProperty.Aspect:
					fromFloat = targetFloat.Value;
					toFloat = camera.aspect;
					break;
				case CameraProperty.BackgroundColor:
					fromColor = targetColor.Value;
					toColor = camera.backgroundColor;
					break;
				case CameraProperty.FieldOfView:
					fromFloat = targetFloat.Value;
					toFloat = camera.fieldOfView;
					break;
				case CameraProperty.OrthoSize:
					fromFloat = targetFloat.Value;
					toFloat = camera.orthographicSize;
					break;
				case CameraProperty.PixelRect:
					fromRect = targetRect.Value;
					toRect = camera.pixelRect;
					break;
				case CameraProperty.ViewportRect:
					fromRect = targetRect.Value;
					toRect = camera.rect;
					break;
				default:
					throw new ArgumentOutOfRangeException();
				}
			}
			else
			{
				switch (property)
				{
				case CameraProperty.Aspect:
					fromFloat = camera.aspect;
					toFloat = targetFloat.Value;
					break;
				case CameraProperty.BackgroundColor:
					fromColor = camera.backgroundColor;
					toColor = targetColor.Value;
					break;
				case CameraProperty.FieldOfView:
					fromFloat = camera.fieldOfView;
					toFloat = targetFloat.Value;
					break;
				case CameraProperty.OrthoSize:
					fromFloat = camera.orthographicSize;
					toFloat = targetFloat.Value;
					break;
				case CameraProperty.PixelRect:
					fromRect = camera.pixelRect;
					toRect = targetRect.Value;
					break;
				case CameraProperty.ViewportRect:
					fromRect = camera.rect;
					toRect = targetRect.Value;
					break;
				default:
					throw new ArgumentOutOfRangeException();
				}
			}
		}

		protected override void DoTween()
		{
			float t = base.easingFunction(0f, 1f, normalizedTime);
			switch (property)
			{
			case CameraProperty.Aspect:
				camera.aspect = Mathf.Lerp(fromFloat, toFloat, t);
				break;
			case CameraProperty.BackgroundColor:
				camera.backgroundColor = Color.Lerp(fromColor, toColor, t);
				break;
			case CameraProperty.FieldOfView:
				camera.fieldOfView = Mathf.Lerp(fromFloat, toFloat, t);
				break;
			case CameraProperty.OrthoSize:
				camera.orthographicSize = Mathf.Lerp(fromFloat, toFloat, t);
				break;
			case CameraProperty.PixelRect:
				camera.pixelRect = camera.pixelRect.Lerp(fromRect, toRect, t);
				break;
			case CameraProperty.ViewportRect:
				camera.rect = camera.rect.Lerp(fromRect, toRect, t);
				break;
			default:
				throw new ArgumentOutOfRangeException();
			}
		}
	}
}
