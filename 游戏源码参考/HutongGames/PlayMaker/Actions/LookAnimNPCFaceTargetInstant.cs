namespace HutongGames.PlayMaker.Actions
{
	public class LookAnimNPCFaceTargetInstant : FSMUtility.GetComponentFsmStateAction<LookAnimNPC>
	{
		public override void Reset()
		{
			base.Reset();
		}

		protected override void DoAction(LookAnimNPC lookAnim)
		{
			lookAnim.FaceTargetInstant();
		}
	}
}
