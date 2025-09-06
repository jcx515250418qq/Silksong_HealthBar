namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Scene)]
	[Tooltip("Get a scene Root GameObjects.")]
	public class GetSceneRootGameObjects : GetSceneActionBase
	{
		[ActionSection("Result")]
		[Tooltip("The scene Root GameObjects")]
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[ArrayEditor(VariableType.GameObject, "", 0, 0, 65536)]
		public FsmArray rootGameObjects;

		[Tooltip("Repeat every Frame")]
		public bool everyFrame;

		public override void Reset()
		{
			base.Reset();
			rootGameObjects = null;
			everyFrame = false;
		}

		public override void OnEnter()
		{
			base.OnEnter();
			DoGetSceneRootGameObjects();
			if (!everyFrame)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			DoGetSceneRootGameObjects();
		}

		private void DoGetSceneRootGameObjects()
		{
			if (_sceneFound)
			{
				if (!rootGameObjects.IsNone)
				{
					FsmArray fsmArray = rootGameObjects;
					object[] values = _scene.GetRootGameObjects();
					fsmArray.Values = values;
				}
				base.Fsm.Event(sceneFoundEvent);
			}
		}
	}
}
