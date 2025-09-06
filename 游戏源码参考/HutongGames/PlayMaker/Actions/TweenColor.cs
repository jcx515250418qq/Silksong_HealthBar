using System;
using HutongGames.PlayMaker.TweenEnums;
using UnityEngine;
using UnityEngine.UI;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Tween)]
	[Tooltip("Tween the color of a GameObject or a Color variable. The GameObject needs a Material, Sprite, Image, Text, or Light component.")]
	public class TweenColor : TweenPropertyBase<FsmColor>
	{
		public enum Target
		{
			GameObject = 0,
			Variable = 1
		}

		public enum TargetType
		{
			None = 0,
			Material = 1,
			Sprite = 2,
			Image = 3,
			Text = 4,
			Light = 5
		}

		private const string SupportedComponents = "MeshRenderer, Sprite, Image, Text, Light.";

		private const string OffsetTooltip = "How to apply the Offset Color. Similar to Photoshop Blend modes. \nNote: use the color alpha to fade the blend.";

		[Tooltip("What to tween.")]
		public Target target = Target.Variable;

		[Tooltip("A GameObject with a Material, Sprite, Image, Text, or Light component.")]
		public FsmOwnerDefault gameObject;

		[Tooltip("The variable to tween.")]
		[UIHint(UIHint.Variable)]
		public FsmColor variable;

		[Tooltip("How to apply the Offset Color. Similar to Photoshop Blend modes. \nNote: use the color alpha to fade the blend.")]
		public ColorBlendMode fromOffsetBlendMode;

		[Tooltip("How to apply the Offset Color. Similar to Photoshop Blend modes. \nNote: use the color alpha to fade the blend.")]
		public ColorBlendMode toOffsetBlendMode;

		private GameObject cachedGameObject;

		private Component cachedComponent;

		private TargetType targetType;

		private Material material;

		private SpriteRenderer spriteRenderer;

		private Text text;

		private Image image;

		private Light light;

		public TargetType type => targetType;

		public override void Reset()
		{
			base.Reset();
			fromOffsetBlendMode = ColorBlendMode.Normal;
			toOffsetBlendMode = ColorBlendMode.Normal;
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
			cachedComponent = go.GetComponent<MeshRenderer>();
			if (cachedComponent != null)
			{
				return;
			}
			cachedComponent = go.GetComponent<Image>();
			if (cachedComponent != null)
			{
				return;
			}
			cachedComponent = go.GetComponent<Text>();
			if (!(cachedComponent != null))
			{
				cachedComponent = go.GetComponent<Light>();
				if (!(cachedComponent != null))
				{
					cachedComponent = go.GetComponent<SpriteRenderer>();
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
			Init();
		}

		private void Init()
		{
			targetType = TargetType.None;
			MeshRenderer meshRenderer = cachedComponent as MeshRenderer;
			if (meshRenderer != null)
			{
				targetType = TargetType.Material;
				material = meshRenderer.material;
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
			}
		}

		public override void OnEnter()
		{
			if (target == Target.GameObject)
			{
				CheckCache();
			}
			base.OnEnter();
			InitOffsets();
			DoTween();
		}

		protected override void InitTargets()
		{
			switch (fromOption)
			{
			case TargetValueOptions.CurrentValue:
				base.StartValue = GetTargetColor();
				break;
			case TargetValueOptions.Value:
				base.StartValue = fromValue.Value;
				break;
			case TargetValueOptions.Offset:
				base.StartValue = GetOffsetValue(variable.RawValue, fromValue.RawValue);
				break;
			default:
				throw new ArgumentOutOfRangeException();
			}
			switch (toOption)
			{
			case TargetValueOptions.CurrentValue:
				base.EndValue = GetTargetColor();
				break;
			case TargetValueOptions.Value:
				base.EndValue = toValue.Value;
				break;
			case TargetValueOptions.Offset:
				base.EndValue = GetOffsetValue(variable.RawValue, toValue.RawValue);
				break;
			default:
				throw new ArgumentOutOfRangeException();
			}
		}

		private Color GetTargetColor()
		{
			if (target == Target.Variable)
			{
				return variable.Value;
			}
			return targetType switch
			{
				TargetType.None => Color.white, 
				TargetType.Material => material.color, 
				TargetType.Sprite => spriteRenderer.color, 
				TargetType.Image => image.color, 
				TargetType.Text => text.color, 
				TargetType.Light => light.color, 
				_ => throw new ArgumentOutOfRangeException(), 
			};
		}

		private void SetTargetColor(Color color)
		{
			if (target == Target.Variable)
			{
				variable.Value = color;
				return;
			}
			switch (targetType)
			{
			case TargetType.Material:
				material.color = color;
				break;
			case TargetType.Sprite:
				spriteRenderer.color = color;
				break;
			case TargetType.Image:
				image.color = color;
				break;
			case TargetType.Text:
				text.color = color;
				break;
			case TargetType.Light:
				light.color = color;
				break;
			default:
				throw new ArgumentOutOfRangeException();
			case TargetType.None:
				break;
			}
		}

		private void InitOffsets()
		{
			if (fromOption == TargetValueOptions.Offset)
			{
				base.StartValue = ActionHelpers.BlendColor(fromOffsetBlendMode, GetTargetColor(), fromValue.Value);
			}
			if (toOption == TargetValueOptions.Offset)
			{
				base.EndValue = ActionHelpers.BlendColor(toOffsetBlendMode, GetTargetColor(), toValue.Value);
			}
		}

		protected override object GetOffsetValue(object value, object offset)
		{
			return value;
		}

		protected override void DoTween()
		{
			float t = base.easingFunction(0f, 1f, normalizedTime);
			SetTargetColor(Color.Lerp((Color)base.StartValue, (Color)base.EndValue, t));
		}
	}
}
