using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Hollow Knight")]
	public class SetSendKilledToObject : FsmStateAction
	{
		[UIHint(UIHint.Variable)]
		public FsmOwnerDefault target;

		public FsmGameObject killedObject;

		public override void Reset()
		{
			target = new FsmOwnerDefault();
			killedObject = new FsmGameObject();
		}

		public override void OnEnter()
		{
			GameObject safe = target.GetSafe(this);
			if (safe != null)
			{
				HealthManager component = safe.GetComponent<HealthManager>();
				if (component != null && !killedObject.IsNone)
				{
					component.SetSendKilledToObject(killedObject.Value);
				}
			}
			Finish();
		}
	}
}
