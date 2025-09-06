using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.SpriteRenderer)]
	[Tooltip("Get the Sorting Layer name and/or Id of a of a SpriteRenderer component.")]
	public class GetSpriteSortingLayer : ComponentAction<SpriteRenderer>
	{
		[RequiredField]
		[CheckForComponent(typeof(SpriteRenderer))]
		[Tooltip("The GameObject with the SpriteRenderer component.")]
		public FsmOwnerDefault gameObject;

		[UIHint(UIHint.Variable)]
		[Tooltip("The sorting layer name")]
		public FsmString sortingLayerName;

		[UIHint(UIHint.Variable)]
		[Tooltip("The sorting layer id")]
		public FsmInt sortingLayerId;

		public override void Reset()
		{
			gameObject = null;
			sortingLayerName = null;
			sortingLayerId = null;
		}

		public override void OnEnter()
		{
			if (UpdateCache(base.Fsm.GetOwnerDefaultTarget(gameObject)))
			{
				if (!sortingLayerName.IsNone)
				{
					sortingLayerName.Value = cachedComponent.sortingLayerName;
				}
				if (!sortingLayerId.IsNone)
				{
					sortingLayerId.Value = cachedComponent.sortingLayerID;
				}
				Finish();
			}
		}
	}
}
