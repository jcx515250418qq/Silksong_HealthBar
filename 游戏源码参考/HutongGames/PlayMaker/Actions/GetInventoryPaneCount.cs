using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	public class GetInventoryPaneCount : FsmStateAction
	{
		[Tooltip("Will find the pane list in the parent of this target.")]
		public FsmOwnerDefault paneListChild;

		[UIHint(UIHint.Variable)]
		public FsmInt UnlockedPaneCount;

		public FsmEvent SinglePaneEvent;

		public FsmEvent ManyPaneEvent;

		public override void Reset()
		{
			paneListChild = null;
			UnlockedPaneCount = null;
		}

		public override void OnEnter()
		{
			GameObject safe = paneListChild.GetSafe(this);
			if (safe != null)
			{
				InventoryPaneList componentInParent = safe.GetComponentInParent<InventoryPaneList>();
				if (componentInParent != null)
				{
					int unlockedPaneCount = componentInParent.UnlockedPaneCount;
					UnlockedPaneCount.Value = unlockedPaneCount;
					base.Fsm.Event((unlockedPaneCount <= 1) ? SinglePaneEvent : ManyPaneEvent);
				}
			}
			Finish();
		}
	}
}
