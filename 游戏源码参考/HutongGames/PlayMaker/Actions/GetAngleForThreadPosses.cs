namespace HutongGames.PlayMaker.Actions
{
	public class GetAngleForThreadPosses : FsmStateAction
	{
		[UIHint(UIHint.Variable)]
		public FsmFloat StoreAngle;

		public override void Reset()
		{
			StoreAngle = null;
		}

		public override void OnEnter()
		{
			GameManager instance = GameManager.instance;
			if ((bool)instance)
			{
				CustomSceneManager sm = instance.sm;
				StoreAngle.Value = sm.AngleToSilkThread;
			}
			Finish();
		}
	}
}
