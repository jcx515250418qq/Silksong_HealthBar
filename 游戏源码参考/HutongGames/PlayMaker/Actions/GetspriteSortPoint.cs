using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.SpriteRenderer)]
	[Tooltip("Get the position of the Sprite used for sorting the Renderer.")]
	public class GetspriteSortPoint : ComponentAction<SpriteRenderer>
	{
		[RequiredField]
		[CheckForComponent(typeof(SpriteRenderer))]
		[Tooltip("The GameObject with the SpriteRenderer component.")]
		public FsmOwnerDefault gameObject;

		[Tooltip("Get the Sprite Sorting Point value")]
		[ObjectType(typeof(SpriteSortPoint))]
		[UIHint(UIHint.Variable)]
		public FsmEnum spriteSortPoint;

		public override void Reset()
		{
			gameObject = null;
			spriteSortPoint = null;
		}

		public override void OnEnter()
		{
			if (UpdateCache(base.Fsm.GetOwnerDefaultTarget(gameObject)))
			{
				spriteSortPoint.Value = cachedComponent.spriteSortPoint;
				Finish();
			}
		}
	}
}
