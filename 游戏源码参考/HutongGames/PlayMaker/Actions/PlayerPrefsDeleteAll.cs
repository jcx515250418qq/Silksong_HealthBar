using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("PlayerPrefs")]
	[Tooltip("Removes all keys and values from the preferences. Use with caution.")]
	public class PlayerPrefsDeleteAll : FsmStateAction
	{
		public override void Reset()
		{
		}

		public override void OnEnter()
		{
			PlayerPrefs.DeleteAll();
			Finish();
		}
	}
}
