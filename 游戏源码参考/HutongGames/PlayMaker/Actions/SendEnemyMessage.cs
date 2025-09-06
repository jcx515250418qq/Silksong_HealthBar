using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Hollow Knight")]
	[Tooltip("Hook for enemy messages for scripts. Translates messages into appropriate method calls.")]
	public class SendEnemyMessage : FsmStateAction
	{
		public FsmGameObject Target;

		public FsmString EventString;

		public override void Reset()
		{
			Target = new FsmGameObject
			{
				UseVariable = true
			};
			EventString = new FsmString
			{
				UseVariable = true
			};
		}

		public override void OnEnter()
		{
			GameObject value = Target.Value;
			string value2 = EventString.Value;
			if (value != null && !string.IsNullOrEmpty(value2))
			{
				if (!(value2 == "GO LEFT"))
				{
					if (value2 == "GO RIGHT")
					{
						SendWalkerGoInDirection(value, 1);
					}
				}
				else
				{
					SendWalkerGoInDirection(value, -1);
				}
			}
			Finish();
		}

		private static void SendWalkerGoInDirection(GameObject target, int facing)
		{
			Walker component = target.GetComponent<Walker>();
			if (component != null)
			{
				component.RecieveGoMessage(facing);
			}
			WalkerV2 component2 = target.GetComponent<WalkerV2>();
			if ((bool)component2)
			{
				component2.ForceDirection(facing);
			}
		}
	}
}
