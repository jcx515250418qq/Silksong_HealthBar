namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Scene)]
	[Tooltip("Allow scenes to be activated. Use this after {{LoadSceneAsynch}} if you did not set the scene to activate after loading")]
	public class AllowSceneActivation : FsmStateAction
	{
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The name of the new scene. It cannot be empty, null, or same as existing scenes.")]
		public FsmInt aSynchOperationHashCode;

		[Tooltip("Allow the scene to be activated")]
		public FsmBool allowSceneActivation;

		[ActionSection("Result")]
		[Tooltip("The loading's progress.")]
		[UIHint(UIHint.Variable)]
		public FsmFloat progress;

		[Tooltip("True when loading is done")]
		[UIHint(UIHint.Variable)]
		public FsmBool isDone;

		[Tooltip("Event sent when scene loading is done")]
		public FsmEvent doneEvent;

		[Tooltip("Event sent when action could not be performed. Check Log for more information")]
		public FsmEvent failureEvent;

		public override void Reset()
		{
			aSynchOperationHashCode = null;
			allowSceneActivation = true;
			progress = null;
			isDone = null;
			doneEvent = null;
			failureEvent = null;
		}

		public override void OnEnter()
		{
			DoAllowSceneActivation();
		}

		public override void OnUpdate()
		{
			if (!progress.IsNone)
			{
				progress.Value = LoadSceneAsynch.aSyncOperationLUT[aSynchOperationHashCode.Value].progress;
			}
			if (!isDone.IsNone)
			{
				isDone.Value = LoadSceneAsynch.aSyncOperationLUT[aSynchOperationHashCode.Value].isDone;
			}
			if (LoadSceneAsynch.aSyncOperationLUT[aSynchOperationHashCode.Value].isDone)
			{
				LoadSceneAsynch.aSyncOperationLUT.Remove(aSynchOperationHashCode.Value);
				base.Fsm.Event(doneEvent);
				Finish();
			}
		}

		private void DoAllowSceneActivation()
		{
			if (aSynchOperationHashCode.IsNone || allowSceneActivation.IsNone || LoadSceneAsynch.aSyncOperationLUT == null || !LoadSceneAsynch.aSyncOperationLUT.ContainsKey(aSynchOperationHashCode.Value))
			{
				base.Fsm.Event(failureEvent);
				Finish();
			}
			else
			{
				LoadSceneAsynch.aSyncOperationLUT[aSynchOperationHashCode.Value].allowSceneActivation = allowSceneActivation.Value;
			}
		}
	}
}
