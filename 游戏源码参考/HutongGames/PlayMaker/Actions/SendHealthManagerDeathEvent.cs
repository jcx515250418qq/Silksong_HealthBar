using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Hollow Knight")]
	public class SendHealthManagerDeathEvent : FsmStateAction
	{
		[UIHint(UIHint.Variable)]
		public FsmOwnerDefault target;

		public override void Reset()
		{
			target = new FsmOwnerDefault();
		}

		public override void OnEnter()
		{
			GameObject gameObject = ((target.OwnerOption == OwnerDefaultOption.UseOwner) ? base.Owner : target.GameObject.Value);
			if (gameObject != null)
			{
				HealthManager component = gameObject.GetComponent<HealthManager>();
				if (component != null)
				{
					component.SendDeathEvent();
				}
			}
			Finish();
		}
	}
}
