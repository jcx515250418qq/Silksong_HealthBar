namespace HutongGames.PlayMaker.Actions
{
	public class WaitIfPoolCreation : FsmStateAction
	{
		public FsmEvent isCreating;

		public FsmEvent isNotCreating;

		public override void Reset()
		{
			isCreating = null;
			isNotCreating = null;
		}

		public override void OnEnter()
		{
			if (!ObjectPool.IsCreatingPool)
			{
				base.Fsm.Event(isNotCreating);
				Finish();
			}
			else
			{
				base.Fsm.Event(isCreating);
			}
		}

		public override void OnUpdate()
		{
			if (!ObjectPool.IsCreatingPool)
			{
				base.Fsm.Event(isNotCreating);
				Finish();
			}
		}
	}
}
