namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Scene)]
	[Tooltip("Get a scene name.")]
	public class GetSceneName : GetSceneActionBase
	{
		[ActionSection("Result")]
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The scene name")]
		public FsmString name;

		public override void Reset()
		{
			base.Reset();
			name = null;
		}

		public override void OnEnter()
		{
			base.OnEnter();
			DoGetSceneName();
			Finish();
		}

		private void DoGetSceneName()
		{
			if (_sceneFound)
			{
				if (!name.IsNone)
				{
					name.Value = _scene.name;
				}
				base.Fsm.Event(sceneFoundEvent);
			}
		}
	}
}
