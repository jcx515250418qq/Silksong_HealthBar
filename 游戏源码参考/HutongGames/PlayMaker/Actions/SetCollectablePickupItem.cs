using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	public class SetCollectablePickupItem : FsmStateAction
	{
		public FsmOwnerDefault Target;

		[ObjectType(typeof(SavedItem))]
		public FsmObject Item;

		public override void Reset()
		{
			Target = null;
			Item = null;
		}

		public override void OnEnter()
		{
			GameObject safe = Target.GetSafe(this);
			if ((bool)safe)
			{
				CollectableItemPickup component = safe.GetComponent<CollectableItemPickup>();
				if ((bool)component)
				{
					component.SetItem(Item.Value as SavedItem);
				}
			}
			Finish();
		}
	}
}
