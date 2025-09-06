using TeamCherry.SharedUtils;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Logic)]
	[Tooltip("Sends Events based on the value of a Boolean Variable.")]
	public class BoolTestDelayV2 : FsmStateAction
	{
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Readonly]
		[Tooltip("The Bool variable to test.")]
		public FsmBool BoolVariable;

		[Tooltip("Event to send if the Bool variable is True.")]
		public FsmEvent IsTrue;

		[Tooltip("Event to send if the Bool variable is False.")]
		public FsmEvent IsFalse;

		public FsmFloat MinDelay;

		public FsmFloat MaxDelay;

		private float timer;

		private float delay;

		private bool previousBoolValue;

		public override void Reset()
		{
			BoolVariable = null;
			IsTrue = null;
			IsFalse = null;
			MinDelay = null;
			MaxDelay = null;
		}

		public override void OnEnter()
		{
			ResetTimer();
		}

		public override void OnUpdate()
		{
			if (BoolVariable.Value != previousBoolValue)
			{
				ResetTimer();
			}
			else if (timer < delay)
			{
				timer += Time.deltaTime;
			}
			else
			{
				base.Fsm.Event(BoolVariable.Value ? IsTrue : IsFalse);
			}
		}

		private void ResetTimer()
		{
			timer = 0f;
			delay = new MinMaxFloat(MinDelay.Value, MaxDelay.Value).GetRandomValue();
			previousBoolValue = BoolVariable.Value;
		}
	}
}
