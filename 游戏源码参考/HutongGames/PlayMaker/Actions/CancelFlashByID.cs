namespace HutongGames.PlayMaker.Actions
{
	public class CancelFlashByID : FsmStateAction
	{
		[RequiredField]
		[CheckForComponent(typeof(SpriteFlash))]
		public FsmOwnerDefault Target;

		public FsmInt ID;

		public override void Reset()
		{
			Target = null;
			ID = null;
		}

		public override void OnEnter()
		{
			SpriteFlash safe = Target.GetSafe<SpriteFlash>(this);
			if (safe != null)
			{
				safe.CancelFlashByID(ID.Value);
			}
			Finish();
		}
	}
}
