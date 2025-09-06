namespace HutongGames.PlayMaker.Actions
{
	[Tooltip("Cancels all lag hits on target")]
	public class CancelAllLagHits : FsmStateAction
	{
		[RequiredField]
		[CheckForComponent(typeof(HealthManager))]
		public FsmOwnerDefault Target;

		public override void Reset()
		{
			Target = null;
		}

		public override void OnEnter()
		{
			HealthManager safe = Target.GetSafe<HealthManager>(this);
			if (safe != null)
			{
				safe.CancelAllLagHits();
			}
			Finish();
		}
	}
}
