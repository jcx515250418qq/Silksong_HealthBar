using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("PlayerPrefs")]
	[Tooltip("Load variable value saved with {{PlayerPrefs Save Variable}}. The Key should be a unique identifier for the variable.\nNOTE: You cannot save references to Scene Objects in PlayerPrefs!")]
	public class PlayerPrefsLoadVariable : FsmStateAction
	{
		[Tooltip("Case sensitive key.")]
		public FsmString key;

		[UIHint(UIHint.Variable)]
		[Tooltip("The variable to load.")]
		public FsmVar variable;

		public override void Reset()
		{
			key = null;
			variable = null;
		}

		public override void OnEnter()
		{
			if (!FsmString.IsNullOrEmpty(key) && !variable.IsNone)
			{
				string @string = PlayerPrefs.GetString(key.Value, "");
				if (@string == "")
				{
					Finish();
					return;
				}
				FsmVar fsmVar = JsonUtility.FromJson<FsmVar>(@string);
				if (fsmVar.Type == variable.Type && fsmVar.ObjectType == variable.ObjectType)
				{
					fsmVar.ApplyValueTo(variable.NamedVar);
				}
				variable.NamedVar.Init();
			}
			Finish();
		}
	}
}
