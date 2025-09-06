using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	public class TryOpenQuickMap : FsmStateAction
	{
		public FsmOwnerDefault Target;

		[UIHint(UIHint.Variable)]
		public FsmString StoreDisplayName;

		public FsmEvent HasMapEvent;

		public FsmEvent NoMapEvent;

		[UIHint(UIHint.Variable)]
		public FsmBool StoreValue;

		public override void Reset()
		{
			Target = null;
			StoreDisplayName = null;
			HasMapEvent = null;
			NoMapEvent = null;
			StoreValue = null;
		}

		public override void OnEnter()
		{
			GameObject safe = Target.GetSafe(this);
			if ((bool)safe)
			{
				GameMap component = safe.GetComponent<GameMap>();
				if ((bool)component)
				{
					string displayName;
					bool flag = component.TryOpenQuickMap(out displayName);
					StoreDisplayName.Value = displayName;
					StoreValue.Value = flag;
					base.Fsm.Event(flag ? HasMapEvent : NoMapEvent);
				}
			}
			Finish();
		}
	}
}
