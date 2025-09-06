using System.Text;

namespace HutongGames.PlayMaker.Actions
{
	public class GetHeroCStateEvent : FsmStateAction
	{
		public FsmString VariableName;

		[UIHint(UIHint.Variable)]
		public FsmBool StoreValue;

		public FsmEvent TrueEvent;

		public FsmEvent FalseEvent;

		public bool EveryFrame;

		public override void Reset()
		{
			VariableName = null;
			StoreValue = null;
			EveryFrame = false;
			TrueEvent = null;
			FalseEvent = null;
		}

		public override void OnEnter()
		{
			DoAction();
			if (!EveryFrame)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			DoAction();
		}

		private void DoAction()
		{
			if (!VariableName.IsNone)
			{
				bool cState = HeroController.instance.GetCState(VariableName.Value);
				if (!StoreValue.IsNone)
				{
					StoreValue.Value = cState;
				}
				if (cState)
				{
					base.Fsm.Event(TrueEvent);
				}
				else
				{
					base.Fsm.Event(FalseEvent);
				}
			}
			else
			{
				Finish();
			}
		}

		public override string ErrorCheck()
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (string.IsNullOrEmpty(VariableName.Value))
			{
				stringBuilder.AppendLine("State name must be specified!");
			}
			else if (!HeroController.CStateExists(VariableName.Value))
			{
				stringBuilder.AppendLine("State could not be found in HeroControllerStates");
			}
			return stringBuilder.ToString();
		}
	}
}
