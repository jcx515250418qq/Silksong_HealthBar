using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("GameObject")]
	[Tooltip("Set sprite renderer to active or inactive based on the given current color. Can only be one sprite renderer on object. ")]
	public class SetSpriteRendererByColor : FsmStateAction
	{
		[RequiredField]
		public FsmOwnerDefault gameObject;

		[UIHint(UIHint.Variable)]
		public FsmColor Color;

		public FsmBool EveryFrame;

		private SpriteRenderer spriteRenderer;

		public override void Reset()
		{
			gameObject = null;
			Color = new FsmColor
			{
				UseVariable = true
			};
			EveryFrame = new FsmBool
			{
				UseVariable = false,
				Value = true
			};
		}

		public override void OnEnter()
		{
			if (gameObject != null)
			{
				GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(gameObject);
				if (ownerDefaultTarget != null)
				{
					spriteRenderer = ownerDefaultTarget.GetComponent<SpriteRenderer>();
				}
			}
			if (spriteRenderer != null)
			{
				Apply();
			}
			if (spriteRenderer == null || !EveryFrame.Value)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			Apply();
		}

		private void Apply()
		{
			if (spriteRenderer != null)
			{
				bool flag = Color.Value.a > Mathf.Epsilon;
				if (spriteRenderer.enabled != flag)
				{
					spriteRenderer.enabled = flag;
				}
			}
		}
	}
}
