namespace HutongGames.PlayMaker.Actions
{
	public class AddTempStatusVignette : FsmStateAction
	{
		[ObjectType(typeof(StatusVignette.TempStatusTypes))]
		public FsmEnum Status;

		public override void Reset()
		{
			Status = null;
		}

		public override void OnEnter()
		{
			StatusVignette.AddTempStatus((StatusVignette.TempStatusTypes)(object)Status.Value);
			Finish();
		}
	}
}
