using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	public class CollectableItemGetDataV3 : CollectableItemAction
	{
		[UIHint(UIHint.Variable)]
		public FsmInt CollectedAmount;

		[UIHint(UIHint.Variable)]
		public FsmBool IsHoldingAny;

		public FsmInt NeededAmount;

		public FsmEvent IsCollected;

		public FsmEvent NotCollected;

		[UIHint(UIHint.Variable)]
		public FsmString ItemName;

		[UIHint(UIHint.Variable)]
		[ObjectType(typeof(Sprite))]
		public FsmObject Sprite;

		[ObjectType(typeof(CollectableItem.ReadSource))]
		public FsmEnum ReadSource;

		public override void Reset()
		{
			base.Reset();
			CollectedAmount = null;
			IsHoldingAny = null;
			NeededAmount = null;
			IsCollected = null;
			NotCollected = null;
			ItemName = null;
			Sprite = null;
			ReadSource = null;
		}

		protected override void DoAction(CollectableItem item)
		{
			CollectableItem.ReadSource readSource = (CollectableItem.ReadSource)(object)ReadSource.Value;
			ItemName.Value = item.GetDisplayName(readSource);
			Sprite.Value = item.GetIcon(readSource);
			int collectedAmount = item.CollectedAmount;
			CollectedAmount.Value = collectedAmount;
			IsHoldingAny.Value = collectedAmount > 0;
			int num = ((!NeededAmount.IsNone) ? NeededAmount.Value : 0);
			base.Fsm.Event((collectedAmount >= num) ? IsCollected : NotCollected);
		}
	}
}
