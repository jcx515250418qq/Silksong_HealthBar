using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.SpriteRenderer)]
	[Tooltip("Set the mode under which the sprite will interact with the masking system.")]
	public class SetSpriteMaskInteraction : ComponentAction<SpriteRenderer>
	{
		[RequiredField]
		[CheckForComponent(typeof(SpriteRenderer))]
		[Tooltip("The GameObject with the SpriteRenderer component.")]
		public FsmOwnerDefault gameObject;

		[Tooltip("Set the Mask Interactions of the SpriteRenderer component.")]
		[ObjectType(typeof(SpriteMaskInteraction))]
		public FsmEnum spriteMaskInteraction;

		public override void Reset()
		{
			gameObject = null;
			spriteMaskInteraction = new FsmEnum
			{
				UseVariable = true
			};
		}

		public override void OnEnter()
		{
			if (UpdateCache(base.Fsm.GetOwnerDefaultTarget(gameObject)))
			{
				cachedComponent.maskInteraction = (SpriteMaskInteraction)(object)spriteMaskInteraction.Value;
				Finish();
			}
		}
	}
}
