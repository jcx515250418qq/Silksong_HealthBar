namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.GameObject)]
	[Tooltip("Gets a Game Object's Layer and stores it in an Int Variable.")]
	public class GetLayer : FsmStateAction
	{
		[RequiredField]
		[Tooltip("The Game Object to examine.")]
		public FsmGameObject gameObject;

		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the Layer in an Int Variable.")]
		public FsmInt storeResult;

		[Tooltip("Repeat every frame.")]
		public bool everyFrame;

		public override void Reset()
		{
			gameObject = null;
			storeResult = null;
			everyFrame = false;
		}

		public override void OnEnter()
		{
			DoGetLayer();
			if (!everyFrame)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			DoGetLayer();
		}

		private void DoGetLayer()
		{
			if (!(gameObject.Value == null))
			{
				storeResult.Value = gameObject.Value.layer;
			}
		}
	}
}
