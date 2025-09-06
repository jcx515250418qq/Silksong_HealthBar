using System;
using HutongGames.PlayMaker.TweenEnums;
using UnityEngine;
using UnityEngine.UI;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Tween)]
	[Tooltip("Fades a GameObject with a Material, Sprite, Image, Text, Light, AudioSource, or CanvasGroup component.\n\nNote: The Material Shader must support transparency. For example, in URP set the Surface Type to Transparent.\n\nTip: When using the Standard shader, set Rendering Mode to Fade for best fading effect.")]
	public class TweenFade : TweenActionBase
	{
		public enum TargetType
		{
			None = 0,
			Material = 1,
			Sprite = 2,
			Image = 3,
			Text = 4,
			Light = 5,
			AudioSource = 6,
			CanvasGroup = 7
		}

		private const string SupportedComponents = "MeshRenderer, Sprite, Image, Text, Light, AudioSource, or CanvasGroup component.";

		[Tooltip("A GameObject with a MeshRenderer, Sprite, Image, Text, Light, AudioSource, or CanvasGroup component.")]
		public FsmOwnerDefault gameObject;

		[Tooltip("Fade To or From value.")]
		public TweenDirection tweenDirection;

		[Tooltip("Value to fade to. E.g., alpha if fading an image, volume if fading audio...")]
		public FsmFloat value;

		private GameObject cachedGameObject;

		private Component cachedComponent;

		private TargetType targetType;

		private Renderer renderer;

		private SpriteRenderer spriteRenderer;

		private Text text;

		private Image image;

		private Light light;

		private CanvasGroup canvasGroup;

		private AudioSource audioSource;

		private float startValue;

		private float endValue;

		public TargetType type => targetType;

		public override void Reset()
		{
			base.Reset();
			tweenDirection = TweenDirection.To;
			value = null;
			gameObject = null;
			cachedGameObject = null;
			cachedComponent = null;
		}

		private void UpdateCache(GameObject go)
		{
			cachedGameObject = go;
			if (go == null)
			{
				cachedComponent = null;
				return;
			}
			FindComponent(typeof(MeshRenderer), typeof(Image), typeof(Text), typeof(Light), typeof(SpriteRenderer), typeof(AudioSource), typeof(CanvasGroup));
		}

		private void FindComponent(params Type[] components)
		{
			foreach (Type type in components)
			{
				cachedComponent = cachedGameObject.GetComponent(type);
				if (cachedComponent != null)
				{
					break;
				}
			}
		}

		private void CheckCache()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(gameObject);
			if (cachedGameObject != ownerDefaultTarget)
			{
				UpdateCache(ownerDefaultTarget);
			}
			InitTarget();
		}

		private void InitTarget()
		{
			targetType = TargetType.None;
			renderer = cachedComponent as MeshRenderer;
			if (renderer != null)
			{
				targetType = TargetType.Material;
				return;
			}
			image = cachedComponent as Image;
			if (image != null)
			{
				targetType = TargetType.Image;
				return;
			}
			spriteRenderer = cachedComponent as SpriteRenderer;
			if (spriteRenderer != null)
			{
				targetType = TargetType.Sprite;
				return;
			}
			text = cachedComponent as Text;
			if (text != null)
			{
				targetType = TargetType.Text;
				return;
			}
			light = cachedComponent as Light;
			if (light != null)
			{
				targetType = TargetType.Light;
				return;
			}
			audioSource = cachedComponent as AudioSource;
			if (audioSource != null)
			{
				targetType = TargetType.AudioSource;
				return;
			}
			canvasGroup = cachedComponent as CanvasGroup;
			if (canvasGroup != null)
			{
				targetType = TargetType.CanvasGroup;
			}
		}

		public override void OnEnter()
		{
			CheckCache();
			if (targetType == TargetType.None)
			{
				LogWarning("GameObject needs a MeshRenderer, Sprite, Image, Text, Light, AudioSource, or CanvasGroup component.");
				base.Enabled = false;
				return;
			}
			if (tweenDirection == TweenDirection.From)
			{
				startValue = value.Value;
				endValue = GetTargetFade();
			}
			else
			{
				startValue = GetTargetFade();
				endValue = value.Value;
			}
			base.OnEnter();
			DoTween();
		}

		private float GetTargetFade()
		{
			return targetType switch
			{
				TargetType.None => 1f, 
				TargetType.Material => renderer.material.color.a, 
				TargetType.Sprite => spriteRenderer.color.a, 
				TargetType.Image => image.color.a, 
				TargetType.Text => text.color.a, 
				TargetType.Light => light.intensity, 
				TargetType.CanvasGroup => canvasGroup.alpha, 
				TargetType.AudioSource => audioSource.volume, 
				_ => throw new ArgumentOutOfRangeException(), 
			};
		}

		private void SetTargetFade(float fade)
		{
			switch (targetType)
			{
			case TargetType.Material:
			{
				Color color = renderer.material.color;
				color.a = fade;
				renderer.material.color = color;
				break;
			}
			case TargetType.Sprite:
			{
				Color color = spriteRenderer.color;
				color.a = fade;
				spriteRenderer.color = color;
				break;
			}
			case TargetType.Image:
			{
				Color color = image.color;
				color.a = fade;
				image.color = color;
				break;
			}
			case TargetType.Text:
			{
				Color color = text.color;
				color.a = fade;
				text.color = color;
				break;
			}
			case TargetType.Light:
				light.intensity = fade;
				break;
			case TargetType.AudioSource:
				audioSource.volume = fade;
				break;
			case TargetType.CanvasGroup:
				canvasGroup.alpha = fade;
				break;
			default:
				throw new ArgumentOutOfRangeException();
			case TargetType.None:
				break;
			}
		}

		protected override void DoTween()
		{
			float t = base.easingFunction(0f, 1f, normalizedTime);
			SetTargetFade(Mathf.Lerp(startValue, endValue, t));
		}
	}
}
