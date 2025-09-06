using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	public class SendEventEnableFsm : FsmStateAction
	{
		public FsmOwnerDefault Target;

		public FsmBool SendToChildren;

		public FsmString EventName;

		public override void Reset()
		{
			Target = null;
			SendToChildren = null;
			EventName = null;
		}

		public override void OnEnter()
		{
			GameObject safe = Target.GetSafe(this);
			if ((bool)safe)
			{
				if (SendToChildren.Value)
				{
					PlayMakerFSM[] componentsInChildren = safe.GetComponentsInChildren<PlayMakerFSM>();
					foreach (PlayMakerFSM playMakerFSM in componentsInChildren)
					{
						if (!playMakerFSM.enabled)
						{
							playMakerFSM.enabled = true;
						}
						playMakerFSM.SendEvent(EventName.Value);
					}
				}
				else
				{
					PlayMakerFSM component = safe.GetComponent<PlayMakerFSM>();
					if ((bool)component)
					{
						if (!component.enabled)
						{
							component.enabled = true;
						}
						component.SendEvent(EventName.Value);
					}
				}
			}
			Finish();
		}
	}
}
