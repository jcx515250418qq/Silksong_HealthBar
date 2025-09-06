namespace HutongGames.PlayMaker.Actions
{
	public class SetPoisonTintOverride : FsmStateAction
	{
		[CheckForComponent(typeof(PoisonTintBase))]
		[RequiredField]
		public FsmOwnerDefault Target;

		[RequiredField]
		public FsmBool IsPoison;

		public override void Reset()
		{
			Target = null;
			IsPoison = null;
		}

		public override void OnEnter()
		{
			Target.GetSafe(this).GetComponent<PoisonTintBase>().SetPoisoned(IsPoison.Value);
			Finish();
		}
	}
}
