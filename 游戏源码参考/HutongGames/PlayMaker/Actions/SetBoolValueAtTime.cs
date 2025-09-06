using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Math)]
	[Tooltip("Sets the value of a Bool Variable after a certain point in time has passed.")]
	public class SetBoolValueAtTime : FsmStateAction
	{
		[RequiredField]
		[UIHint(UIHint.Variable)]
		public FsmBool BoolVariable;

		[RequiredField]
		public FsmBool BoolValue;

		public FsmFloat Time;

		public bool SetOppositeOnStateEntry;

		private float timer;

		public override void Reset()
		{
			BoolVariable = null;
			BoolValue = null;
			Time = null;
		}

		public override void OnEnter()
		{
			timer = 0f;
			if (SetOppositeOnStateEntry)
			{
				BoolVariable.Value = !BoolValue.Value;
			}
			if (Time.Value <= 0f)
			{
				Set();
			}
		}

		public override void OnUpdate()
		{
			if (timer >= Time.Value)
			{
				Set();
			}
			else
			{
				timer += UnityEngine.Time.deltaTime;
			}
		}

		private void Set()
		{
			BoolVariable.Value = BoolValue.Value;
			Finish();
		}
	}
}
