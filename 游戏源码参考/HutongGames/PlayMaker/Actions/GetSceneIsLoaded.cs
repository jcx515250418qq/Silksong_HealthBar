namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Scene)]
	[Tooltip("Get a scene isLoaded flag.")]
	public class GetSceneIsLoaded : GetSceneActionBase
	{
		[ActionSection("Result")]
		[Tooltip("true if the scene is loaded.")]
		[UIHint(UIHint.Variable)]
		public FsmBool isLoaded;

		[Tooltip("Event sent if the scene is loaded.")]
		public FsmEvent isLoadedEvent;

		[Tooltip("Event sent if the scene is not loaded.")]
		public FsmEvent isNotLoadedEvent;

		[Tooltip("Repeat every Frame")]
		public bool everyFrame;

		public override void Reset()
		{
			base.Reset();
			isLoaded = null;
			everyFrame = false;
		}

		public override void OnEnter()
		{
			base.OnEnter();
			DoGetSceneIsLoaded();
			if (!everyFrame)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			DoGetSceneIsLoaded();
		}

		private void DoGetSceneIsLoaded()
		{
			if (_sceneFound)
			{
				if (!isLoaded.IsNone)
				{
					isLoaded.Value = _scene.isLoaded;
				}
				base.Fsm.Event(sceneFoundEvent);
			}
		}
	}
}
