using System;
using System.Collections;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	public class CharacterAmbientLightLerp : FsmStateAction
	{
		[RequiredField]
		[CheckForComponent(typeof(tk2dSprite))]
		public FsmOwnerDefault Target;

		[RequiredField]
		public FsmBool EnableAmbientColor;

		private Coroutine fadeRoutine;

		private static readonly int _ambientLerpProp = Shader.PropertyToID("_AmbientLerp");

		private const string REQUIRED_KEYWORD = "CAN_LERP_AMBIENT";

		public override void Reset()
		{
			Target = null;
			EnableAmbientColor = null;
		}

		public override void Awake()
		{
			GameObject safe = Target.GetSafe(this);
			if ((bool)safe)
			{
				tk2dSprite component = safe.GetComponent<tk2dSprite>();
				if ((bool)component)
				{
					component.EnableKeyword("CAN_LERP_AMBIENT");
				}
			}
		}

		public override void OnEnter()
		{
			tk2dSprite component = Target.GetSafe(this).GetComponent<tk2dSprite>();
			component.EnableKeyword("CAN_LERP_AMBIENT");
			if (fadeRoutine != null)
			{
				StopCoroutine(fadeRoutine);
			}
			fadeRoutine = StartCoroutine(FadeRoutine(component, EnableAmbientColor.Value));
			Finish();
		}

		private IEnumerator FadeRoutine(tk2dSprite sprite, bool enableAmbient)
		{
			MeshRenderer renderer = sprite.GetComponent<MeshRenderer>();
			MaterialPropertyBlock block = new MaterialPropertyBlock();
			renderer.GetPropertyBlock(block);
			float fromVal = block.GetFloat(_ambientLerpProp);
			float toVal = (enableAmbient ? 1f : 0f);
			if (Math.Abs(fromVal - toVal) > 0.01f)
			{
				for (float elapsed = 0f; elapsed <= 0.1f; elapsed += Time.deltaTime)
				{
					float value = Mathf.Lerp(fromVal, toVal, elapsed / 0.1f);
					renderer.GetPropertyBlock(block);
					block.SetFloat(_ambientLerpProp, value);
					renderer.SetPropertyBlock(block);
					yield return null;
				}
			}
			renderer.GetPropertyBlock(block);
			block.SetFloat(_ambientLerpProp, toVal);
			renderer.SetPropertyBlock(block);
			fadeRoutine = null;
		}
	}
}
