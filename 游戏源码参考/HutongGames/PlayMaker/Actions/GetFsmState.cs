using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.StateMachine)]
	[ActionTarget(typeof(PlayMakerFSM), "fsmComponent", false)]
	[Tooltip("Gets the name of the specified FSMs current state. Either reference the fsm component directly, or find it on a game object.")]
	public class GetFsmState : FsmStateAction
	{
		[Tooltip("Choose a PlayMakerFSM component. If you set a component here it overrides the Game Object and Fsm Name settings.")]
		public PlayMakerFSM fsmComponent;

		[Tooltip("If not specifying the component above, specify the GameObject that owns the FSM.")]
		public FsmOwnerDefault gameObject;

		[UIHint(UIHint.FsmName)]
		[Tooltip("Optional name of FSM on Game Object. If left blank it will find the first PlayMakerFSM on the GameObject.")]
		public FsmString fsmName;

		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the FSM's current State in a string variable.")]
		public FsmString storeResult;

		[Tooltip("Repeat every frame. E.g.,  useful if you're waiting for the State to change.")]
		public bool everyFrame;

		private PlayMakerFSM fsm;

		public override void Reset()
		{
			fsmComponent = null;
			gameObject = null;
			fsmName = "";
			storeResult = null;
			everyFrame = false;
		}

		public override void OnEnter()
		{
			DoGetFsmState();
			if (!everyFrame)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			DoGetFsmState();
		}

		private void DoGetFsmState()
		{
			if (fsm == null)
			{
				if (fsmComponent != null)
				{
					fsm = fsmComponent;
				}
				else
				{
					GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(gameObject);
					if (ownerDefaultTarget != null)
					{
						fsm = ActionHelpers.GetGameObjectFsm(ownerDefaultTarget, fsmName.Value);
					}
				}
				if (fsm == null)
				{
					storeResult.Value = "";
					return;
				}
			}
			storeResult.Value = fsm.ActiveStateName;
		}
	}
}
