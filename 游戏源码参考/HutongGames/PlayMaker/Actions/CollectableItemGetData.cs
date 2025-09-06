using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	public class CollectableItemGetData : CollectableItemAction
	{
		[UIHint(UIHint.Variable)]
		public FsmInt CollectedAmount;

		public FsmInt NeededAmount;

		public FsmEvent IsCollected;

		public FsmEvent NotCollected;

		[UIHint(UIHint.Variable)]
		public FsmString ItemName;

		[UIHint(UIHint.Variable)]
		[ObjectType(typeof(Sprite))]
		public FsmObject Sprite;

		public override void Reset()
		{
			base.Reset();
			CollectedAmount = null;
			NeededAmount = null;
			IsCollected = null;
			NotCollected = null;
			ItemName = null;
			Sprite = null;
		}

		protected override void DoAction(CollectableItem item)
		{
			ItemName.Value = item.GetDisplayName(CollectableItem.ReadSource.GetPopup);
			Sprite.Value = item.GetIcon(CollectableItem.ReadSource.GetPopup);
			int collectedAmount = item.CollectedAmount;
			CollectedAmount.Value = collectedAmount;
			int num = ((!NeededAmount.IsNone) ? NeededAmount.Value : 0);
			base.Fsm.Event((collectedAmount >= num) ? IsCollected : NotCollected);
		}
	}
}
