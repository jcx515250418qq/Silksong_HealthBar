namespace HutongGames.PlayMaker.Actions
{
	public class RemoveStatusVignetteStatus : FsmStateAction
	{
		[ObjectType(typeof(StatusVignette.StatusTypes))]
		public FsmEnum Status;

		[UIHint(UIHint.Variable)]
		public FsmBool StoredSet;

		public override void Reset()
		{
			Status = null;
			StoredSet = false;
		}

		public override void OnEnter()
		{
			if (StoredSet.IsNone || StoredSet.Value)
			{
				StatusVignette.RemoveStatus((StatusVignette.StatusTypes)(object)Status.Value);
				StoredSet.Value = false;
			}
			Finish();
		}
	}
}
