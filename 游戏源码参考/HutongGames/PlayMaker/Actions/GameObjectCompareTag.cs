namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Logic)]
	[Tooltip("Tests if a Game Object has a tag.")]
	public class GameObjectCompareTag : FsmStateAction
	{
		[RequiredField]
		[Tooltip("The GameObject to test.")]
		public FsmGameObject gameObject;

		[RequiredField]
		[UIHint(UIHint.Tag)]
		[Tooltip("The Tag to check for.")]
		public FsmString tag;

		[Tooltip("Event to send if the GameObject has the Tag.")]
		public FsmEvent trueEvent;

		[Tooltip("Event to send if the GameObject does not have the Tag.")]
		public FsmEvent falseEvent;

		[UIHint(UIHint.Variable)]
		[Tooltip("Store the result in a Bool variable.")]
		public FsmBool storeResult;

		[Tooltip("Repeat every frame.")]
		public bool everyFrame;

		public override void Reset()
		{
			gameObject = null;
			tag = "Untagged";
			trueEvent = null;
			falseEvent = null;
			storeResult = null;
			everyFrame = false;
		}

		public override void OnEnter()
		{
			DoCompareTag();
			if (!everyFrame)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			DoCompareTag();
		}

		private void DoCompareTag()
		{
			bool flag = false;
			if (gameObject.Value != null)
			{
				flag = gameObject.Value.CompareTag(tag.Value);
			}
			storeResult.Value = flag;
			base.Fsm.Event(flag ? trueEvent : falseEvent);
		}
	}
}
