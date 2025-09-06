using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	public class RendererFloatRamp : FsmStateAction
	{
		public FsmOwnerDefault Target;

		public FsmString PropertyName;

		public FsmFloat From;

		public FsmFloat To;

		public FsmFloat Duration;

		public FsmAnimationCurve Curve;

		public FsmBool UseChildren;

		private int propId;

		private float elapsed;

		private Renderer[] renderers;

		private MaterialPropertyBlock[] propBlocks;

		public override void Reset()
		{
			Target = null;
			PropertyName = null;
			From = null;
			To = null;
			Duration = null;
			Curve = new FsmAnimationCurve
			{
				curve = AnimationCurve.Linear(0f, 0f, 1f, 1f)
			};
			UseChildren = null;
		}

		public override void Awake()
		{
			GameObject safe = Target.GetSafe(this);
			if (UseChildren.Value)
			{
				if ((bool)safe)
				{
					renderers = safe.GetComponentsInChildren<Renderer>(includeInactive: true);
					propBlocks = new MaterialPropertyBlock[renderers.Length];
					for (int i = 0; i < renderers.Length; i++)
					{
						MaterialPropertyBlock materialPropertyBlock = new MaterialPropertyBlock();
						propBlocks[i] = materialPropertyBlock;
					}
					return;
				}
			}
			else
			{
				Renderer component = safe.GetComponent<Renderer>();
				if ((bool)component)
				{
					renderers = new Renderer[1] { component };
					MaterialPropertyBlock materialPropertyBlock2 = new MaterialPropertyBlock();
					propBlocks = new MaterialPropertyBlock[1] { materialPropertyBlock2 };
					return;
				}
			}
			renderers = Array.Empty<Renderer>();
			propBlocks = Array.Empty<MaterialPropertyBlock>();
		}

		public override void OnEnter()
		{
			elapsed = 0f;
			propId = Shader.PropertyToID(PropertyName.Value);
			UpdateValues(0f);
		}

		public override void OnUpdate()
		{
			elapsed += Time.deltaTime;
			float num = elapsed / Duration.Value;
			bool num2 = num >= 1f;
			if (num2)
			{
				num = 1f;
			}
			UpdateValues(Curve.curve.Evaluate(num));
			if (num2)
			{
				Finish();
			}
		}

		private void UpdateValues(float t)
		{
			float value = Mathf.Lerp(From.Value, To.Value, t);
			for (int i = 0; i < renderers.Length; i++)
			{
				MaterialPropertyBlock materialPropertyBlock = propBlocks[i];
				Renderer obj = renderers[i];
				obj.GetPropertyBlock(materialPropertyBlock);
				materialPropertyBlock.SetFloat(propId, value);
				obj.SetPropertyBlock(materialPropertyBlock);
			}
		}
	}
}
