using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	public class EndInteractEvents : FsmStateAction
	{
		[RequiredField]
		[CheckForComponent(typeof(InteractEvents))]
		public FsmOwnerDefault Target;

		public override void Reset()
		{
			Target = null;
		}

		public override void OnEnter()
		{
			GameObject safe = Target.GetSafe(this);
			if ((bool)safe)
			{
				InteractEvents component = safe.GetComponent<InteractEvents>();
				if ((bool)component)
				{
					component.EndInteraction();
				}
			}
			Finish();
		}
	}
}
