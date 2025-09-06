using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	public class ActivateInteractible : FsmStateAction
	{
		[RequiredField]
		public FsmOwnerDefault Target;

		[RequiredField]
		public FsmBool Activate;

		[HideIf("IsActivate")]
		public FsmBool AllowQueueing;

		public FsmBool UseChildren;

		public bool IsActivate()
		{
			return Activate.Value;
		}

		public override void Reset()
		{
			Target = null;
			Activate = null;
			AllowQueueing = null;
			UseChildren = null;
		}

		public override void OnEnter()
		{
			GameObject safe = Target.GetSafe(this);
			if ((bool)safe)
			{
				if (!UseChildren.Value)
				{
					InteractableBase component = safe.GetComponent<InteractableBase>();
					if ((bool)component)
					{
						DoInteractableAction(component);
					}
				}
				else
				{
					InteractableBase[] componentsInChildren = safe.GetComponentsInChildren<InteractableBase>();
					foreach (InteractableBase interactable in componentsInChildren)
					{
						DoInteractableAction(interactable);
					}
				}
			}
			Finish();
		}

		private void DoInteractableAction(InteractableBase interactable)
		{
			if (Activate.Value)
			{
				interactable.Activate();
			}
			else
			{
				interactable.Deactivate(AllowQueueing.Value);
			}
		}
	}
}
