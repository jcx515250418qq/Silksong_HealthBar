using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("PlayerData")]
	[Tooltip("Sends a Message to PlayerData to send and receive data.")]
	public class GetPlayerDataFloat : FsmStateAction
	{
		[RequiredField]
		[Tooltip("GameManager reference, set this to the global variable GameManager.")]
		public FsmOwnerDefault gameObject;

		[RequiredField]
		public FsmString floatName;

		[RequiredField]
		[UIHint(UIHint.Variable)]
		public FsmFloat storeValue;

		public override void Reset()
		{
			gameObject = null;
			floatName = null;
			storeValue = null;
		}

		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(gameObject);
			if (!(ownerDefaultTarget == null))
			{
				GameManager component = ownerDefaultTarget.GetComponent<GameManager>();
				if (!(component == null))
				{
					storeValue.Value = component.GetPlayerDataFloat(floatName.Value);
					Finish();
				}
			}
		}
	}
}
