using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	public class GetHealthManagerPhysicalPusher : FsmStateAction
	{
		public FsmOwnerDefault target;

		[UIHint(UIHint.Variable)]
		public FsmGameObject storeGameObject;

		public override void Reset()
		{
			target = null;
			storeGameObject = null;
		}

		public override void OnEnter()
		{
			GameObject safe = target.GetSafe(this);
			if ((bool)safe)
			{
				HealthManager component = safe.GetComponent<HealthManager>();
				if ((bool)component)
				{
					storeGameObject.Value = component.GetPhysicalPusher();
				}
			}
			Finish();
		}
	}
}
