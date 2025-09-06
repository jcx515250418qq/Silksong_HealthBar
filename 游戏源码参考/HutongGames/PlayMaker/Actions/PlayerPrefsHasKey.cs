using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("PlayerPrefs")]
	[Tooltip("Returns true if PlayerPref key exists in the preferences.")]
	public class PlayerPrefsHasKey : FsmStateAction
	{
		[RequiredField]
		[Tooltip("The name of the PlayerPref to test for.")]
		public FsmString key;

		[UIHint(UIHint.Variable)]
		[Title("Store Result")]
		[Tooltip("Store the result in a bool variable.")]
		public FsmBool variable;

		[Tooltip("Event to send if the key exists.")]
		public FsmEvent trueEvent;

		[Tooltip("Event to send if the key does not exist.")]
		public FsmEvent falseEvent;

		public override void Reset()
		{
			key = "";
		}

		public override void OnEnter()
		{
			Finish();
			if (!key.IsNone && !key.Value.Equals(""))
			{
				variable.Value = PlayerPrefs.HasKey(key.Value);
			}
			base.Fsm.Event(variable.Value ? trueEvent : falseEvent);
		}
	}
}
