using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("PlayerData")]
	[Tooltip("Sends a Message to PlayerData to send and receive data.")]
	public class SetPlayerDataBool : FsmStateAction
	{
		[RequiredField]
		public FsmString boolName;

		[RequiredField]
		public FsmBool value;

		public override void Reset()
		{
			boolName = null;
			value = null;
		}

		public override void OnEnter()
		{
			GameManager instance = GameManager.instance;
			if (instance == null)
			{
				Debug.Log("GameManager could not be found");
				Finish();
				return;
			}
			if (!string.IsNullOrEmpty(boolName.Value))
			{
				instance.SetPlayerDataBool(boolName.Value, value.Value);
			}
			Finish();
		}
	}
}
