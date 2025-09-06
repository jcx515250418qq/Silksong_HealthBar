using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("PlayerPrefs")]
	[Tooltip("Save a variable value in PlayerPrefs. You can load the value later with {{PlayerPrefs Load Variable}}.\nNOTE: You cannot save references to Scene Objects in PlayerPrefs!")]
	public class PlayerPrefsSaveVariable : FsmStateAction
	{
		[Tooltip("Case sensitive key.")]
		public FsmString key;

		[UIHint(UIHint.Variable)]
		[Tooltip("The variable to save.")]
		public FsmVar variable;

		public override void Reset()
		{
			key = null;
			variable = null;
		}

		public override void OnEnter()
		{
			if (!FsmString.IsNullOrEmpty(key))
			{
				variable.UpdateValue();
				string value = JsonUtility.ToJson(variable);
				PlayerPrefs.SetString(key.Value, value);
				PlayerPrefs.Save();
			}
			Finish();
		}
	}
}
