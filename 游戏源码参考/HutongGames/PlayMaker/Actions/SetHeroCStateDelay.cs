using System.Text;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	public class SetHeroCStateDelay : FsmStateAction
	{
		public FsmString VariableName;

		public FsmBool Value;

		public float Delay;

		public bool SetOppositeOnExit;

		private float timer;

		public override void Reset()
		{
			VariableName = null;
			Value = null;
			Delay = 0f;
			SetOppositeOnExit = false;
			timer = 0f;
		}

		public override void OnEnter()
		{
			timer = 0f;
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
			if (timer <= Delay)
			{
				timer += Time.deltaTime;
				return;
			}
			DoAction();
			Finish();
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
