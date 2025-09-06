using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Hollow Knight")]
	public class HasTakenDamage : FsmStateAction
	{
		[UIHint(UIHint.Variable)]
		public FsmOwnerDefault target;

		public FsmBool takenDamageBool;

		public FsmEventTarget eventTarget;

		public FsmEvent takenDamageEvent;

		public override void Reset()
		{
			target = new FsmOwnerDefault();
			takenDamageBool = new FsmBool();
			takenDamageEvent = null;
		}

		public override void OnEnter()
		{
			GameObject safe = target.GetSafe(this);
			if (safe != null)
			{
				HealthManager component = safe.GetComponent<HealthManager>();
				if (component != null)
				{
					bool flag = component.HasTakenDamage();
					if (!takenDamageBool.IsNone)
					{
						takenDamageBool.Value = flag;
					}
					if (flag)
					{
						base.Fsm.Event(eventTarget, takenDamageEvent);
					}
				}
			}
			Finish();
		}
	}
}
