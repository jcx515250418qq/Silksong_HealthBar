using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Sprite Renderer")]
	public class GetSpriteRendererSprite : FsmStateAction
	{
		[RequiredField]
		public FsmOwnerDefault gameObject;

		[ObjectType(typeof(Sprite))]
		public FsmObject sprite;

		public override void Reset()
		{
			gameObject = null;
			sprite = null;
		}

		public override void OnEnter()
		{
			if (gameObject != null)
			{
				GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(gameObject);
				sprite.Value = ownerDefaultTarget.GetComponent<SpriteRenderer>().sprite;
			}
			Finish();
		}
	}
}
