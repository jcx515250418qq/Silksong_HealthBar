namespace HutongGames.PlayMaker.Actions
{
	public class AddStatusVignetteStatus : FsmStateAction
	{
		[ObjectType(typeof(StatusVignette.StatusTypes))]
		public FsmEnum Status;

		[UIHint(UIHint.Variable)]
		public FsmBool StoreSet;

		public FsmBool InState;

		public override void Reset()
		{
			Status = null;
			StoreSet = null;
			InState = null;
		}

		public override void OnEnter()
		{
			if (StoreSet.IsNone || !StoreSet.Value)
			{
				StatusVignette.AddStatus((StatusVignette.StatusTypes)(object)Status.Value);
				StoreSet.Value = true;
			}
			if (!InState.Value)
			{
				Finish();
			}
		}

		public override void OnExit()
		{
			if (InState.Value)
			{
				StatusVignette.RemoveStatus((StatusVignette.StatusTypes)(object)Status.Value);
				StoreSet.Value = false;
			}
		}
	}
}
