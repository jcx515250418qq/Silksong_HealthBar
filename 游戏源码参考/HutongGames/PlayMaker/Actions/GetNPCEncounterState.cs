using System;
using GlobalEnums;

namespace HutongGames.PlayMaker.Actions
{
	public class GetNPCEncounterState : FsmStateAction
	{
		[Serializable]
		public class Response
		{
			public NPCEncounterState state;

			public FsmEvent trueEvent;

			public FsmEvent falseEvent;

			[UIHint(UIHint.Variable)]
			public FsmBool storeValue;
		}

		[RequiredField]
		[CheckForComponent(typeof(NPCEncounterStateController))]
		public FsmOwnerDefault Target;

		[UIHint(UIHint.Variable)]
		[ObjectType(typeof(NPCEncounterState))]
		public new FsmEnum State;

		[Tooltip("Define which event to send for each possible state.")]
		public Response[] responses;

		public override void Reset()
		{
			Target = null;
			State = null;
			responses = new Response[0];
		}

		public override void OnEnter()
		{
			NPCEncounterStateController safe = Target.GetSafe<NPCEncounterStateController>(this);
			if (safe != null)
			{
				NPCEncounterState currentState = safe.GetCurrentState();
				FsmEnum state = State;
				if (state != null && !state.IsNone)
				{
					State.Value = currentState;
				}
				Response[] array = responses;
				foreach (Response response in array)
				{
					bool flag = response.state.Equals(currentState);
					FsmBool storeValue = response.storeValue;
					if (storeValue != null && !storeValue.IsNone)
					{
						response.storeValue.Value = flag;
					}
					if (flag)
					{
						base.Fsm.Event(response.trueEvent);
					}
					else
					{
						base.Fsm.Event(response.falseEvent);
					}
				}
			}
			Finish();
		}
	}
}
