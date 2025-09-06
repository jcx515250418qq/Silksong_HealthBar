namespace HutongGames.PlayMaker.Actions
{
	public class ObjectIsNull : FSMUtility.CheckFsmStateAction
	{
		public FsmObject Target;

		public override bool IsTrue => Target.Value == null;

		public override void Reset()
		{
			base.Reset();
			Target = null;
		}
	}
}
