using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Hollow Knight")]
	public class CustomTagIsSuckable : FsmStateAction
	{
		[UIHint(UIHint.Variable)]
		public FsmOwnerDefault target;

		public FsmEvent isSuckableEvent;

		public FsmEvent isNotSuckableEvent;

		public FsmBool storeIsSuckable;

		public override void Reset()
		{
			target = new FsmOwnerDefault();
			storeIsSuckable = new FsmBool();
		}

		public override void OnEnter()
		{
			GameObject gameObject = ((target.OwnerOption == OwnerDefaultOption.UseOwner) ? base.Owner : target.GameObject.Value);
			if (gameObject != null)
			{
				if (gameObject.GetComponent<CustomTag>() == null)
				{
					storeIsSuckable = false;
					base.Fsm.Event(isNotSuckableEvent);
				}
				else
				{
					bool flag = gameObject.GetComponent<CustomTag>().IsSuckable();
					storeIsSuckable = flag;
					base.Fsm.Event(flag ? isSuckableEvent : isNotSuckableEvent);
				}
			}
			Finish();
		}
	}
}
