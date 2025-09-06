using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	public class GetIsInteractableActive : FsmStateAction
	{
		[RequiredField]
		[CheckForComponent(typeof(InteractableBase))]
		public FsmOwnerDefault Target;

		[UIHint(UIHint.Variable)]
		public FsmBool StoreIsActive;

		public override void Reset()
		{
			Target = null;
			StoreIsActive = null;
		}

		public override void OnEnter()
		{
			StoreIsActive.Value = false;
			GameObject safe = Target.GetSafe(this);
			if ((bool)safe)
			{
				InteractableBase component = safe.GetComponent<InteractableBase>();
				if ((bool)component)
				{
					StoreIsActive.Value = !component.IsDisabled;
				}
			}
			Finish();
		}
	}
}
