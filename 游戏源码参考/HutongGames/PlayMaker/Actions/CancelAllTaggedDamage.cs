namespace HutongGames.PlayMaker.Actions
{
	public class CancelAllTaggedDamage : FsmStateAction
	{
		public FsmOwnerDefault Target;

		public override void Reset()
		{
			Target = null;
		}

		public override void OnEnter()
		{
			TagDamageTaker safe = Target.GetSafe<TagDamageTaker>(this);
			if (safe != null)
			{
				safe.ClearTagDamage();
			}
			Finish();
		}
	}
}
