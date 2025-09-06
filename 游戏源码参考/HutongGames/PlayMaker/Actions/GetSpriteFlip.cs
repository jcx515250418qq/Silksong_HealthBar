using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.SpriteRenderer)]
	[Tooltip("Gets the Flips values of a of a SpriteRenderer component.")]
	public class GetSpriteFlip : ComponentAction<SpriteRenderer>
	{
		[RequiredField]
		[CheckForComponent(typeof(SpriteRenderer))]
		[Tooltip("The GameObject with the SpriteRenderer component.")]
		public FsmOwnerDefault gameObject;

		[Tooltip("The X flip value")]
		[UIHint(UIHint.Variable)]
		public FsmBool x;

		[Tooltip("The Y flip value")]
		[UIHint(UIHint.Variable)]
		public FsmBool y;

		[Tooltip("Repeat every frame.")]
		public bool everyFrame;

		public override void Reset()
		{
			gameObject = null;
			x = null;
			y = null;
		}

		public override void OnEnter()
		{
			GetFlip();
			if (!everyFrame)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			GetFlip();
		}

		private void GetFlip()
		{
			if (UpdateCache(base.Fsm.GetOwnerDefaultTarget(gameObject)))
			{
				if (!x.IsNone)
				{
					x.Value = cachedComponent.flipX;
				}
				if (!y.IsNone)
				{
					y.Value = cachedComponent.flipY;
				}
			}
		}
	}
}
