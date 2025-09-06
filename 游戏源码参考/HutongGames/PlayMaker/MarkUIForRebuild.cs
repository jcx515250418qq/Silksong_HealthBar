using UnityEngine;
using UnityEngine.UI;

namespace HutongGames.PlayMaker
{
	[ActionCategory("UI")]
	[Tooltip("Marks a UI GameObject for rebuild, either for graphic updates or layout updates.")]
	public sealed class MarkUIForRebuild : FsmStateAction
	{
		[RequiredField]
		[Tooltip("The GameObject to mark for rebuild.")]
		[CheckForComponent(typeof(RectTransform))]
		public FsmOwnerDefault gameObject;

		[Tooltip("Mark for graphic rebuild (e.g., visual updates like color or material changes).")]
		public FsmBool markGraphicRebuild;

		[Tooltip("Mark for layout rebuild (e.g., recalculating position, size, and layout).")]
		public FsmBool markLayoutRebuild;

		public override void Reset()
		{
			gameObject = null;
			markGraphicRebuild = false;
			markLayoutRebuild = true;
		}

		public override void OnEnter()
		{
			DoMarkForRebuild();
			Finish();
		}

		private void DoMarkForRebuild()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(gameObject);
			if (ownerDefaultTarget == null)
			{
				return;
			}
			RectTransform component = ownerDefaultTarget.GetComponent<RectTransform>();
			if (component == null)
			{
				return;
			}
			if (markGraphicRebuild.Value)
			{
				Graphic component2 = ownerDefaultTarget.GetComponent<Graphic>();
				if (component2 != null)
				{
					CanvasUpdateRegistry.RegisterCanvasElementForGraphicRebuild(component2);
				}
				else
				{
					Debug.LogWarning("No Graphic component found on the specified GameObject for graphic rebuild.");
				}
			}
			if (markLayoutRebuild.Value)
			{
				LayoutRebuilder.MarkLayoutForRebuild(component);
			}
		}
	}
}
