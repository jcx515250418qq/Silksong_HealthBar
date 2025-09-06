using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("PlayerPrefs")]
	[Tooltip("Removes key and its corresponding value from the preferences.")]
	public class PlayerPrefsDeleteKey : FsmStateAction
	{
		[Tooltip("The name of the PlayerPref.")]
		public FsmString key;

		public override void Reset()
		{
			key = "";
		}

		public override void OnEnter()
		{
			if (!key.IsNone && !key.Value.Equals(""))
			{
				PlayerPrefs.DeleteKey(key.Value);
			}
			Finish();
		}
	}
}
