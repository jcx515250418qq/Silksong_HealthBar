using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.SpriteRenderer)]
	[Tooltip("Gets the source image sprite of a SpriteRenderer component.")]
	public class GetSprite : ComponentAction<SpriteRenderer>
	{
		[RequiredField]
		[CheckForComponent(typeof(SpriteRenderer))]
		[Tooltip("The GameObject with the SpriteRenderer component.")]
		public FsmOwnerDefault gameObject;

		[RequiredField]
		[Tooltip("The source sprite of the SpriteRenderer component.")]
		[UIHint(UIHint.Variable)]
		[ObjectType(typeof(Sprite))]
		public FsmObject sprite;

		public override void Reset()
		{
			gameObject = null;
			sprite = null;
		}

		public override void OnEnter()
		{
			ExecuteAction();
			Finish();
		}

		private void ExecuteAction()
		{
			if (UpdateCache(base.Fsm.GetOwnerDefaultTarget(gameObject)))
			{
				sprite.Value = cachedComponent.sprite;
			}
		}
	}
}
