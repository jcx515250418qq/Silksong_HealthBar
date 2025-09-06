using TeamCherry.SharedUtils;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("PlayerData")]
	[Tooltip("Sends a Message to PlayerData to send and receive data.")]
	public class SetPlayerDataFloat : FsmStateAction
	{
		[RequiredField]
		[Tooltip("GameManager reference, set this to the global variable GameManager.")]
		public FsmOwnerDefault gameObject;

		[RequiredField]
		public FsmString floatName;

		[RequiredField]
		public FsmFloat value;

		public override void Reset()
		{
			gameObject = null;
			floatName = null;
			value = null;
		}

		public override void OnEnter()
		{
			if (VariableExtensions.VariableExists<float, PlayerData>(floatName.Value))
			{
				PlayerData.instance.SetVariable(floatName.Value, value.Value);
			}
			else
			{
				Debug.Log($"PlayerData float {floatName.Value} does not exist. (FSM: {base.Fsm.Name}, State: {base.State.Name})", base.Owner);
			}
			Finish();
		}
	}
}
