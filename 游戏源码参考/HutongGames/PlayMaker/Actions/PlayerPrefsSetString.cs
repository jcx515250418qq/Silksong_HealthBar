using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("PlayerPrefs")]
	[Tooltip("Sets the value of the preference identified by key. Lets you save a string that you can load later with {{PlayerPrefs Get String}}.")]
	public class PlayerPrefsSetString : FsmStateAction
	{
		[CompoundArray("Count", "Key", "Value")]
		[Tooltip("Case sensitive key.")]
		public FsmString[] keys;

		[Tooltip("The value to save.")]
		public FsmString[] values;

		public override void Reset()
		{
			keys = new FsmString[1];
			values = new FsmString[1];
		}

		public override void OnEnter()
		{
			for (int i = 0; i < keys.Length; i++)
			{
				if (!keys[i].IsNone || !keys[i].Value.Equals(""))
				{
					PlayerPrefs.SetString(keys[i].Value, values[i].IsNone ? "" : values[i].Value);
				}
			}
			Finish();
		}
	}
}
