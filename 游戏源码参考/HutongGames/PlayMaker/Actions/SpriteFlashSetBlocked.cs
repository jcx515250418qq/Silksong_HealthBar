namespace HutongGames.PlayMaker.Actions
{
	public class SpriteFlashSetBlocked : FsmStateAction
	{
		[RequiredField]
		[CheckForComponent(typeof(SpriteFlash))]
		public FsmOwnerDefault Target;

		public FsmBool SetBlocked;

		public override void Reset()
		{
			Target = null;
			SetBlocked = null;
		}

		public override void OnEnter()
		{
			SpriteFlash safe = Target.GetSafe<SpriteFlash>(this);
			if (safe != null)
			{
				safe.IsBlocked = SetBlocked.Value;
			}
			Finish();
		}
	}
}
