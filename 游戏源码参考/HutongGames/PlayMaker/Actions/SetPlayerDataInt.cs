using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("PlayerData")]
	[Tooltip("Sends a Message to PlayerData to send and receive data.")]
	public class SetPlayerDataInt : FsmStateAction
	{
		[RequiredField]
		public FsmString intName;

		[RequiredField]
		public FsmInt value;

		public override void Reset()
		{
			intName = null;
			value = null;
		}

		public override void OnEnter()
		{
			GameManager instance = GameManager.instance;
			if (instance == null)
			{
				Debug.Log("GameManager could not be found");
				return;
			}
			instance.SetPlayerDataInt(intName.Value, value.Value);
			Finish();
		}
	}
}
