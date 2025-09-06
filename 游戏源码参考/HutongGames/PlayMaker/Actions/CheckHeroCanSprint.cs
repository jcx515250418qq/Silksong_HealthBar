using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	public class CheckHeroCanSprint : FsmStateAction
	{
		public FsmBool storeValue;

		public FsmBool everyFrame;

		public FsmEvent isTrue;

		public FsmEvent isFalse;

		private bool hasHero;

		private HeroController heroController;

		public override void Reset()
		{
			storeValue = null;
		}

		public override void OnEnter()
		{
			hasHero = heroController;
			if (!hasHero)
			{
				heroController = HeroController.instance;
				hasHero = heroController;
			}
			if (!hasHero || !everyFrame.Value)
			{
				SendSprintEvent();
				Finish();
			}
			if (!hasHero)
			{
				Debug.LogError("Failed to find hero controller.");
			}
		}

		public override void OnUpdate()
		{
			SendSprintEvent();
		}

		private void SendSprintEvent()
		{
			if (hasHero)
			{
				if (heroController.CanSprint())
				{
					storeValue.Value = true;
					base.Fsm.Event(isTrue);
				}
				else
				{
					storeValue.Value = false;
					base.Fsm.Event(isFalse);
				}
			}
		}
	}
}
