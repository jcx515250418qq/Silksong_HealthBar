using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.SpriteRenderer)]
	[Tooltip("Get the mode under which the sprite will interact with the masking system.")]
	public class GetSpriteMaskInteraction : ComponentAction<SpriteRenderer>
	{
		[RequiredField]
		[CheckForComponent(typeof(SpriteRenderer))]
		[Tooltip("The GameObject with the SpriteRenderer component.")]
		public FsmOwnerDefault gameObject;

		[Tooltip("Get the Mask Interactions of the SpriteRenderer component.")]
		[ObjectType(typeof(SpriteMaskInteraction))]
		[UIHint(UIHint.Variable)]
		public FsmEnum spriteMaskInteraction;

		public override void Reset()
		{
			gameObject = null;
			spriteMaskInteraction = null;
		}

		public override void OnEnter()
		{
			if (UpdateCache(base.Fsm.GetOwnerDefaultTarget(gameObject)))
			{
				spriteMaskInteraction.Value = cachedComponent.maskInteraction;
				Finish();
			}
		}
	}
}
