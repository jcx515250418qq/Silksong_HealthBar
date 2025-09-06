using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("PlayerData")]
	[Tooltip("Sends a Message to PlayerData to send and receive data.")]
	public class SetPlayerDataString : FsmStateAction
	{
		[RequiredField]
		[Tooltip("GameManager reference, set this to the global variable GameManager.")]
		public FsmOwnerDefault gameObject;

		[RequiredField]
		public FsmString stringName;

		[RequiredField]
		public FsmString value;

		public override void Reset()
		{
			gameObject = null;
			stringName = null;
			value = null;
		}

		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(gameObject);
			if (!(ownerDefaultTarget == null))
			{
				GameManager component = ownerDefaultTarget.GetComponent<GameManager>();
				if (component == null)
				{
					Debug.Log("SetPlayerDataInt: could not find a GameManager on this object, please refere to the GameManager global variable");
					return;
				}
				component.SetPlayerDataString(stringName.Value, value.Value);
				Finish();
			}
		}
	}
}
