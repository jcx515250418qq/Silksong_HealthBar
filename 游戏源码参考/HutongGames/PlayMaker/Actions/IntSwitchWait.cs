using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Logic)]
	[Tooltip("Sends an Event based on the value of an Integer Variable.")]
	public class IntSwitchWait : FsmStateAction
	{
		[RequiredField]
		[Tooltip("Time to wait in seconds.")]
		public FsmFloat time;

		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The integer variable to test.")]
		public FsmInt intVariable;

		[CompoundArray("Int Switches", "Compare Int", "Send Event")]
		[Tooltip("The integer variable to test.")]
		public FsmInt[] compareTo;

		[Tooltip("Event to send if true.")]
		public FsmEvent[] sendEvent;

		[Tooltip("Repeat every frame.")]
		public bool everyFrame;

		private float timer;

		public override void Reset()
		{
			time = null;
			intVariable = null;
			compareTo = new FsmInt[1];
			sendEvent = new FsmEvent[1];
			everyFrame = false;
		}

		public override void OnEnter()
		{
			timer = 0f;
		}

		public override void OnUpdate()
		{
			timer += Time.deltaTime;
			if (timer >= time.Value)
			{
				DoIntSwitch();
				Finish();
			}
		}

		private void DoIntSwitch()
		{
			if (intVariable.IsNone)
			{
				return;
			}
			for (int i = 0; i < compareTo.Length; i++)
			{
				if (intVariable.Value == compareTo[i].Value)
				{
					base.Fsm.Event(sendEvent[i]);
					break;
				}
			}
		}
	}
}
