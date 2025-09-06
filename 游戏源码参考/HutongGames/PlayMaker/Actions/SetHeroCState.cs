using System.Text;

namespace HutongGames.PlayMaker.Actions
{
	public class SetHeroCState : FsmStateAction
	{
		public FsmString VariableName;

		public FsmBool Value;

		public bool EveryFrame;

		public bool SetOppositeOnExit;

		public override void Reset()
		{
			VariableName = null;
			Value = null;
			EveryFrame = false;
			SetOppositeOnExit = false;
		}

		public override void OnEnter()
		{
			DoAction();
			if (!EveryFrame)
			{
				Finish();
			}
		}

		public override void OnExit()
		{
			if (SetOppositeOnExit)
			{
				HeroController.instance.SetCState(VariableName.Value, !Value.Value);
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
				HeroController.instance.SetCState(VariableName.Value, Value.Value);
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
