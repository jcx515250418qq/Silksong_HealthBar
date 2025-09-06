namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Scene)]
	[Tooltip("Returns the index of a scene in the Build Settings. Always returns -1 if the scene was loaded through an AssetBundle.")]
	public class GetSceneBuildIndex : GetSceneActionBase
	{
		[ActionSection("Result")]
		[Tooltip("The scene Build Index")]
		[RequiredField]
		[UIHint(UIHint.Variable)]
		public FsmInt buildIndex;

		public override void Reset()
		{
			base.Reset();
			buildIndex = null;
		}

		public override void OnEnter()
		{
			base.OnEnter();
			DoGetSceneBuildIndex();
			Finish();
		}

		private void DoGetSceneBuildIndex()
		{
			if (_sceneFound)
			{
				if (!buildIndex.IsNone)
				{
					buildIndex.Value = _scene.buildIndex;
				}
				base.Fsm.Event(sceneFoundEvent);
			}
		}
	}
}
