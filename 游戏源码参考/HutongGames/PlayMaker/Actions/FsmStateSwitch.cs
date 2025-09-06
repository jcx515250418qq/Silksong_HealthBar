using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Logic)]
	[ActionTarget(typeof(PlayMakerFSM), "gameObject,fsmName", false)]
	[Tooltip("Sends Events based on the current State of an FSM.")]
	public class FsmStateSwitch : FsmStateAction
	{
		[RequiredField]
		[Tooltip("The GameObject that owns the FSM.")]
		public FsmGameObject gameObject;

		[UIHint(UIHint.FsmName)]
		[Tooltip("Optional name of Fsm on GameObject. Useful if there is more than one FSM on the GameObject.")]
		public FsmString fsmName;

		[CompoundArray("State Switches", "Compare State", "Send Event")]
		[Tooltip("Compare the current State to this State.")]
		public FsmString[] compareTo;

		[Tooltip("Send this event if the same.")]
		public FsmEvent[] sendEvent;

		[Tooltip("Repeat every frame. Useful if you're waiting for a particular result.")]
		public bool everyFrame;

		private GameObject previousGo;

		private PlayMakerFSM fsm;

		public override void Reset()
		{
			gameObject = null;
			fsmName = null;
			compareTo = new FsmString[1];
			sendEvent = new FsmEvent[1];
			everyFrame = false;
		}

		public override void OnEnter()
		{
			DoFsmStateSwitch();
			if (!everyFrame)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			DoFsmStateSwitch();
		}

		private void DoFsmStateSwitch()
		{
			GameObject value = gameObject.Value;
			if (value == null)
			{
				return;
			}
			if (value != previousGo)
			{
				fsm = ActionHelpers.GetGameObjectFsm(value, fsmName.Value);
				previousGo = value;
			}
			if (fsm == null)
			{
				return;
			}
			string activeStateName = fsm.ActiveStateName;
			for (int i = 0; i < compareTo.Length; i++)
			{
				if (activeStateName == compareTo[i].Value)
				{
					base.Fsm.Event(sendEvent[i]);
					break;
				}
			}
		}
	}
}
