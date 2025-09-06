namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Logic)]
	[Tooltip("Compares 2 Game Objects and sends Events based on the result.")]
	public class GameObjectCompare : FsmStateAction
	{
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Title("Game Object")]
		[Tooltip("A Game Object variable to compare.")]
		public FsmOwnerDefault gameObjectVariable;

		[Tooltip("Compare the variable with this Game Object")]
		public FsmGameObject compareTo;

		[Tooltip("Send this event if Game Objects are equal")]
		public FsmEvent equalEvent;

		[Tooltip("Send this event if Game Objects are not equal")]
		public FsmEvent notEqualEvent;

		[UIHint(UIHint.Variable)]
		[Tooltip("Store the result of the check in a Bool Variable. (True if equal, false if not equal).")]
		public FsmBool storeResult;

		[Tooltip("Repeat every frame. Useful if you're waiting for a true or false result.")]
		public bool everyFrame;

		public override void Reset()
		{
			gameObjectVariable = null;
			compareTo = null;
			equalEvent = null;
			notEqualEvent = null;
			storeResult = null;
			everyFrame = false;
		}

		public override void OnEnter()
		{
			DoGameObjectCompare();
			if (!everyFrame)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			DoGameObjectCompare();
		}

		private void DoGameObjectCompare()
		{
			bool flag = base.Fsm.GetOwnerDefaultTarget(gameObjectVariable) == compareTo.Value;
			storeResult.Value = flag;
			if (flag && equalEvent != null)
			{
				base.Fsm.Event(equalEvent);
			}
			else if (!flag && notEqualEvent != null)
			{
				base.Fsm.Event(notEqualEvent);
			}
		}
	}
}
