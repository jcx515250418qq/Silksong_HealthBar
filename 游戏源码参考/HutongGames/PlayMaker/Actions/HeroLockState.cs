using GlobalEnums;

namespace HutongGames.PlayMaker.Actions
{
	public sealed class HeroLockState : FsmStateAction
	{
		public enum Mode
		{
			Add = 0,
			Remove = 1
		}

		public HeroLockStates heroLockStates;

		public Mode mode;

		public FsmBool setOppositeOnExit;

		private HeroController hc;

		public override void Reset()
		{
			heroLockStates = HeroLockStates.None;
		}

		public override void OnEnter()
		{
			hc = HeroController.instance;
			if (hc != null)
			{
				Add();
			}
			Finish();
		}

		public override void OnExit()
		{
			if (setOppositeOnExit.Value && hc != null)
			{
				Remove();
			}
		}

		private void Add()
		{
			switch (mode)
			{
			case Mode.Add:
				hc.AddLockStates(heroLockStates);
				break;
			case Mode.Remove:
				hc.RemoveLockStates(heroLockStates);
				break;
			}
		}

		private void Remove()
		{
			switch (mode)
			{
			case Mode.Add:
				hc.RemoveLockStates(heroLockStates);
				break;
			case Mode.Remove:
				hc.AddLockStates(heroLockStates);
				break;
			}
		}
	}
}
