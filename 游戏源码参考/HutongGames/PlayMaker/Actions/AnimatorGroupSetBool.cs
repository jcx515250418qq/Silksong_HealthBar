namespace HutongGames.PlayMaker.Actions
{
	public class AnimatorGroupSetBool : FSMUtility.GetComponentFsmStateAction<AnimatorGroup>
	{
		[RequiredField]
		public FsmString BoolName;

		[RequiredField]
		public FsmBool SetValue;

		public override void Reset()
		{
			base.Reset();
			BoolName = null;
			SetValue = null;
		}

		protected override void DoAction(AnimatorGroup component)
		{
			if (!BoolName.IsNone)
			{
				component.SetBool(BoolName.Value, SetValue.Value);
			}
		}
	}
}
