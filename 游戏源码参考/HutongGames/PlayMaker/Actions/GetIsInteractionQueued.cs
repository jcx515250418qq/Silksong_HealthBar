using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	public class GetIsInteractionQueued : FsmStateAction
	{
		[RequiredField]
		[CheckForComponent(typeof(InteractableBase))]
		public FsmOwnerDefault Target;

		public FsmEvent IsQueuedEvent;

		public FsmEvent NotQueuedEvent;

		[UIHint(UIHint.Variable)]
		public FsmBool StoreIsQueued;

		public bool EveryFrame;

		private InteractableBase interactable;

		public override void Reset()
		{
			Target = null;
			StoreIsQueued = null;
			EveryFrame = false;
		}

		public override void OnEnter()
		{
			StoreIsQueued.Value = false;
			interactable = null;
			GameObject safe = Target.GetSafe(this);
			if ((bool)safe)
			{
				interactable = safe.GetComponent<InteractableBase>();
				if ((bool)interactable)
				{
					DoAction();
				}
			}
			if (!EveryFrame || !interactable)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			DoAction();
		}

		private void DoAction()
		{
			bool isQueued = interactable.IsQueued;
			StoreIsQueued.Value = isQueued;
			base.Fsm.Event(isQueued ? IsQueuedEvent : NotQueuedEvent);
		}
	}
}
