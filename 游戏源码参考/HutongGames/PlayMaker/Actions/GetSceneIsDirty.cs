namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Scene)]
	[Tooltip("Get a scene isDirty flag. True if the scene is modified.")]
	public class GetSceneIsDirty : GetSceneActionBase
	{
		[ActionSection("Result")]
		[UIHint(UIHint.Variable)]
		[Tooltip("true if the scene is modified.")]
		public FsmBool isDirty;

		[Tooltip("Event sent if the scene is modified.")]
		public FsmEvent isDirtyEvent;

		[Tooltip("Event sent if the scene is unmodified.")]
		public FsmEvent isNotDirtyEvent;

		[Tooltip("Repeat every frame")]
		public bool everyFrame;

		public override void Reset()
		{
			base.Reset();
			isDirty = null;
			everyFrame = false;
		}

		public override void OnEnter()
		{
			base.OnEnter();
			DoGetSceneIsDirty();
			if (!everyFrame)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			DoGetSceneIsDirty();
		}

		private void DoGetSceneIsDirty()
		{
			if (_sceneFound)
			{
				if (!isDirty.IsNone)
				{
					isDirty.Value = _scene.isDirty;
				}
				base.Fsm.Event(sceneFoundEvent);
			}
		}
	}
}
