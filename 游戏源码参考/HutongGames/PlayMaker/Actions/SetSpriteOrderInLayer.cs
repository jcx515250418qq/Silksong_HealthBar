using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.SpriteRenderer)]
	[Tooltip("Set the Order in Layer of a SpriteRenderer component.")]
	public class SetSpriteOrderInLayer : ComponentAction<SpriteRenderer>
	{
		[RequiredField]
		[CheckForComponent(typeof(SpriteRenderer))]
		[Tooltip("The GameObject with the SpriteRenderer component.")]
		public FsmOwnerDefault gameObject;

		[RequiredField]
		[Tooltip("The Order In Layer Value")]
		public FsmInt orderInLayer;

		public override void Reset()
		{
			gameObject = null;
			orderInLayer = null;
		}

		public override void OnEnter()
		{
			if (UpdateCache(base.Fsm.GetOwnerDefaultTarget(gameObject)))
			{
				cachedComponent.sortingOrder = orderInLayer.Value;
				Finish();
			}
		}
	}
}
