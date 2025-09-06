using System.Text;

namespace HutongGames.PlayMaker.Actions
{
	public class GetHeroCState : FsmStateAction
	{
		public FsmString VariableName;

		[UIHint(UIHint.Variable)]
		public FsmBool StoreValue;

		public bool EveryFrame;

		public override void Reset()
		{
			VariableName = null;
			StoreValue = null;
			EveryFrame = false;
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
			if (!VariableName.IsNone && !StoreValue.IsNone)
			{
				StoreValue.Value = HeroController.instance.GetCState(VariableName.Value);
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
