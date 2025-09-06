namespace HutongGames.PlayMaker.Actions
{
	public class AnimatorGroupPlay : FSMUtility.GetComponentFsmStateAction<AnimatorGroup>
	{
		[RequiredField]
		public FsmString StateName;

		public override void Reset()
		{
			base.Reset();
			StateName = null;
		}

		protected override void DoAction(AnimatorGroup component)
		{
			if (!StateName.IsNone)
			{
				component.Play(StateName.Value);
			}
		}
	}
}
