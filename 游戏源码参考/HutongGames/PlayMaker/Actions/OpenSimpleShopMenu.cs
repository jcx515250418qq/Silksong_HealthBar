using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	public class OpenSimpleShopMenu : FsmStateAction
	{
		[CheckForComponent(typeof(SimpleShopMenuOwner))]
		public FsmOwnerDefault Target;

		public FsmEvent NoStockEvent;

		public FsmEvent CancelledEvent;

		public FsmEvent PurchasedEvent;

		private SimpleShopMenuOwner shopOwner;

		public override void Reset()
		{
			Target = null;
			NoStockEvent = null;
			CancelledEvent = null;
			PurchasedEvent = null;
		}

		public override void OnEnter()
		{
			GameObject safe = Target.GetSafe(this);
			if (!safe)
			{
				return;
			}
			shopOwner = safe.GetComponent<SimpleShopMenuOwner>();
			if ((bool)shopOwner)
			{
				shopOwner.ClosedNoPurchase += OnClosedNoPurchase;
				shopOwner.ClosedPurchase += OnClosedPurchase;
				if (!shopOwner.OpenMenu())
				{
					OnNoStock();
				}
			}
		}

		public override void OnExit()
		{
			if ((bool)shopOwner)
			{
				shopOwner.ClosedNoPurchase -= OnClosedNoPurchase;
				shopOwner.ClosedPurchase -= OnClosedPurchase;
				shopOwner = null;
			}
		}

		private void OnNoStock()
		{
			base.Fsm.Event(NoStockEvent);
			Finish();
		}

		private void OnClosedNoPurchase()
		{
			base.Fsm.Event(CancelledEvent);
			Finish();
		}

		private void OnClosedPurchase()
		{
			base.Fsm.Event(PurchasedEvent);
			Finish();
		}
	}
}
