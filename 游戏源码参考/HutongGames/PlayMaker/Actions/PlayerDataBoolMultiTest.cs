using System;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("PlayerData")]
	[Tooltip("Sends a Message to PlayerData to send and receive data.")]
	public class PlayerDataBoolMultiTest : FsmStateAction
	{
		[Serializable]
		public class BoolTest
		{
			public FsmString boolName;

			[Tooltip("Will use this value instead if bool name is empty.")]
			[UIHint(UIHint.Variable)]
			public FsmBool inputBool;

			[Tooltip("Expected state player data bool needs to be in to pass.")]
			public FsmBool expectedValue;

			[Tooltip("Value of player data bool (Independent of expected value).")]
			[UIHint(UIHint.Variable)]
			public FsmBool storeValue;
		}

		[RequiredField]
		public BoolTest[] boolTests;

		[UIHint(UIHint.Variable)]
		public FsmBool storeValue;

		public FsmEvent passedEvent;

		public FsmEvent failedEvent;

		public override void Reset()
		{
			boolTests = null;
			storeValue = null;
			passedEvent = null;
			failedEvent = null;
		}

		public override void OnEnter()
		{
			GameManager instance = GameManager.instance;
			if (instance == null)
			{
				return;
			}
			bool flag = true;
			BoolTest[] array = boolTests;
			foreach (BoolTest boolTest in array)
			{
				string value = boolTest.boolName.Value;
				bool flag2 = string.IsNullOrEmpty(value);
				if (boolTest.inputBool.IsNone && flag2)
				{
					flag = false;
					continue;
				}
				bool flag3 = boolTest.inputBool.Value;
				if (!flag2)
				{
					flag3 = instance.GetPlayerDataBool(value);
					boolTest.storeValue.Value = flag3;
				}
				if (boolTest.expectedValue.Value != flag3)
				{
					flag = false;
				}
			}
			storeValue.Value = flag;
			if (flag)
			{
				base.Fsm.Event(passedEvent);
			}
			else
			{
				base.Fsm.Event(failedEvent);
			}
			Finish();
		}
	}
}
