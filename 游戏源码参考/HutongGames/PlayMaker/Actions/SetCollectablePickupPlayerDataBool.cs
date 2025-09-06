using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	public class SetCollectablePickupPlayerDataBool : FsmStateAction
	{
		public FsmOwnerDefault Target;

		public FsmString PlayerDataBoolName;

		public override void Reset()
		{
			Target = null;
			PlayerDataBoolName = null;
		}

		public override void OnEnter()
		{
			GameObject safe = Target.GetSafe(this);
			if ((bool)safe)
			{
				CollectableItemPickup component = safe.GetComponent<CollectableItemPickup>();
				if ((bool)component)
				{
					component.SetPlayerDataBool(PlayerDataBoolName.Value);
				}
			}
			Finish();
		}
	}
}
