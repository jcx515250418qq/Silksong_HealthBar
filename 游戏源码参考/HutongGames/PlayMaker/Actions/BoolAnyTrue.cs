namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Logic)]
	[Tooltip("Tests if any of the given Bool Variables are True.")]
	public class BoolAnyTrue : FsmStateAction
	{
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The Bool variables to check.")]
		public FsmBool[] boolVariables;

		[Tooltip("Event to send if any of the Bool variables are True.")]
		public FsmEvent sendEvent;

		[UIHint(UIHint.Variable)]
		[Tooltip("Store the result in a Bool variable.")]
		public FsmBool storeResult;

		[Tooltip("Repeat every frame while the state is active. Useful if you're waiting for any to be true.")]
		public bool everyFrame;

		public override void Reset()
		{
			boolVariables = null;
			sendEvent = null;
			storeResult = null;
			everyFrame = false;
		}

		public override void OnEnter()
		{
			DoAnyTrue();
			if (!everyFrame)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			DoAnyTrue();
		}

		private void DoAnyTrue()
		{
			if (boolVariables.Length == 0)
			{
				return;
			}
			bool value = false;
			FsmBool[] array = boolVariables;
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].Value)
				{
					base.Fsm.Event(sendEvent);
					value = true;
					break;
				}
			}
			storeResult.Value = value;
		}
	}
}
