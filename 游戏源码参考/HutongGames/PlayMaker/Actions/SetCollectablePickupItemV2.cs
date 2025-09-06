using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	public class SetCollectablePickupItemV2 : FsmStateAction
	{
		public FsmOwnerDefault Target;

		[ObjectType(typeof(SavedItem))]
		public FsmObject Item;

		public FsmBool KeepPersistence;

		public override void Reset()
		{
			Target = null;
			Item = null;
			KeepPersistence = null;
		}

		public override void OnEnter()
		{
			GameObject safe = Target.GetSafe(this);
			if ((bool)safe)
			{
				CollectableItemPickup component = safe.GetComponent<CollectableItemPickup>();
				if ((bool)component)
				{
					component.SetItem(Item.Value as SavedItem, KeepPersistence.Value);
				}
			}
			Finish();
		}
	}
}
