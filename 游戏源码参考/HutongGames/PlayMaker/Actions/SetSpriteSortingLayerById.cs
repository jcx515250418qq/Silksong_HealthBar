using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.SpriteRenderer)]
	[Tooltip("Set the Sorting Layer of a SpriteRenderer component by Id (by id is faster than by name)")]
	public class SetSpriteSortingLayerById : ComponentAction<SpriteRenderer>
	{
		[RequiredField]
		[CheckForComponent(typeof(SpriteRenderer))]
		[Tooltip("The GameObject with the SpriteRenderer component.")]
		public FsmOwnerDefault gameObject;

		[RequiredField]
		[Tooltip("The sorting Layer Name")]
		public FsmInt sortingLayerId;

		[Tooltip("If true, set the sorting layer to all children")]
		public FsmBool setAllSpritesInChildren;

		public override void Reset()
		{
			gameObject = null;
			sortingLayerId = null;
			setAllSpritesInChildren = false;
		}

		public override void OnEnter()
		{
			if (!UpdateCache(base.Fsm.GetOwnerDefaultTarget(gameObject)))
			{
				return;
			}
			if (setAllSpritesInChildren.Value)
			{
				SpriteRenderer[] componentsInChildren = cachedComponent.GetComponentsInChildren<SpriteRenderer>();
				for (int i = 0; i < componentsInChildren.Length; i++)
				{
					componentsInChildren[i].sortingLayerID = sortingLayerId.Value;
				}
			}
			else
			{
				cachedComponent.sortingLayerID = sortingLayerId.Value;
			}
			Finish();
		}
	}
}
