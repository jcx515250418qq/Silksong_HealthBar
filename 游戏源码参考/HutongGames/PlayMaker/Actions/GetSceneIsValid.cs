namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Scene)]
	[Tooltip("Get a scene isValid flag. A scene may be invalid if, for example, you tried to open a scene that does not exist. In this case, the scene returned from EditorSceneManager.OpenScene would return False for IsValid. ")]
	public class GetSceneIsValid : GetSceneActionBase
	{
		[ActionSection("Result")]
		[UIHint(UIHint.Variable)]
		[Tooltip("true if the scene is loaded.")]
		public FsmBool isValid;

		[Tooltip("Event sent if the scene is valid.")]
		public FsmEvent isValidEvent;

		[Tooltip("Event sent if the scene is not valid.")]
		public FsmEvent isNotValidEvent;

		public override void Reset()
		{
			base.Reset();
			isValid = null;
		}

		public override void OnEnter()
		{
			base.OnEnter();
			DoGetSceneIsValid();
			Finish();
		}

		private void DoGetSceneIsValid()
		{
			if (_sceneFound)
			{
				if (!isValid.IsNone)
				{
					isValid.Value = _scene.IsValid();
				}
				if (_scene.IsValid())
				{
					base.Fsm.Event(isValidEvent);
				}
				else
				{
					base.Fsm.Event(isNotValidEvent);
				}
				base.Fsm.Event(sceneFoundEvent);
			}
		}
	}
}
