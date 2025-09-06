using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.AnimateVariables)]
	[Tooltip("Easing Animation - Color")]
	public class EaseSpriteColor : EaseFsmAction
	{
		[UIHint(UIHint.Variable)]
		[RequiredField]
		public FsmGameObject target;

		[RequiredField]
		public FsmColor fromValue;

		[RequiredField]
		public FsmColor toValue;

		public bool disableSpriteRendererIfTransparent;

		private SpriteRenderer spriteRenderer;

		private bool finishInNextStep;

		public override void Reset()
		{
			base.Reset();
			target = null;
			fromValue = null;
			toValue = null;
			finishInNextStep = false;
		}

		public override void OnEnter()
		{
			if (!(target.Value != null))
			{
				return;
			}
			spriteRenderer = target.Value.GetComponent<SpriteRenderer>();
			Color color = fromValue.Value;
			if (spriteRenderer != null)
			{
				if (fromValue.IsNone)
				{
					Color color3 = (fromValue.Value = spriteRenderer.color);
					color = color3;
				}
				else
				{
					spriteRenderer.color = color;
				}
				spriteRenderer.enabled = true;
			}
			base.OnEnter();
			fromFloats = new float[4];
			fromFloats[0] = color.r;
			fromFloats[1] = color.g;
			fromFloats[2] = color.b;
			fromFloats[3] = color.a;
			toFloats = new float[4];
			Color value = toValue.Value;
			toFloats[0] = value.r;
			toFloats[1] = value.g;
			toFloats[2] = value.b;
			toFloats[3] = value.a;
			resultFloats = new float[4];
			finishInNextStep = false;
		}

		public override void OnExit()
		{
			base.OnExit();
		}

		public override void OnUpdate()
		{
			base.OnUpdate();
			if (isRunning && spriteRenderer != null)
			{
				spriteRenderer.color = new Color(resultFloats[0], resultFloats[1], resultFloats[2], resultFloats[3]);
			}
			if (finishInNextStep)
			{
				if (disableSpriteRendererIfTransparent && spriteRenderer.color.a == 0f)
				{
					spriteRenderer.enabled = false;
				}
				Finish();
				if (finishEvent != null)
				{
					base.Fsm.Event(finishEvent);
				}
			}
			if (!finishAction || finishInNextStep)
			{
				return;
			}
			if (spriteRenderer != null)
			{
				if (reverse.IsNone || !reverse.Value)
				{
					spriteRenderer.color = toValue.Value;
				}
				else
				{
					spriteRenderer.color = fromValue.Value;
				}
			}
			finishInNextStep = true;
		}
	}
}
