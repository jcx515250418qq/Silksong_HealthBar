namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Scene)]
	[Tooltip("Get a scene path.")]
	public class GetScenePath : GetSceneActionBase
	{
		[ActionSection("Result")]
		[Tooltip("The scene path")]
		[RequiredField]
		[UIHint(UIHint.Variable)]
		public FsmString path;

		public override void Reset()
		{
			base.Reset();
			path = null;
		}

		public override void OnEnter()
		{
			base.OnEnter();
			DoGetScenePath();
			Finish();
		}

		private void DoGetScenePath()
		{
			if (_sceneFound)
			{
				if (!path.IsNone)
				{
					path.Value = _scene.path;
				}
				base.Fsm.Event(sceneFoundEvent);
			}
		}
	}
}
