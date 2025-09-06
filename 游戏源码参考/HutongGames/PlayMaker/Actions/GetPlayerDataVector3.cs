using TeamCherry.SharedUtils;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("PlayerData")]
	[Tooltip("Sends a Message to PlayerData to send and receive data.")]
	public class GetPlayerDataVector3 : FsmStateAction
	{
		[RequiredField]
		public FsmString vector3Name;

		[RequiredField]
		[UIHint(UIHint.Variable)]
		public FsmVector3 storeValue;

		public override void Reset()
		{
			vector3Name = null;
			storeValue = null;
		}

		public override void OnEnter()
		{
			if (VariableExtensions.VariableExists<Vector3, PlayerData>(vector3Name.Value))
			{
				storeValue.Value = PlayerData.instance.GetVariable<Vector3>(vector3Name.Value);
			}
			else
			{
				Debug.Log($"PlayerData vector3 {vector3Name.Value} does not exist. (FSM: {base.Fsm.Name}, State: {base.State.Name})", base.Owner);
			}
			Finish();
		}
	}
}
