using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	public class ActivateNPCFlyAroundTalking : FsmStateAction
	{
		[CheckForComponent(typeof(NPCFlyAround))]
		public FsmOwnerDefault Target;

		public FsmBool SetActive;

		public override void Reset()
		{
			Target = null;
		}

		public override void OnEnter()
		{
			GameObject safe = Target.GetSafe(this);
			if (!safe)
			{
				Finish();
				return;
			}
			NPCFlyAround component = safe.GetComponent<NPCFlyAround>();
			if (SetActive.Value)
			{
				component.EnableTalkingFlyAround();
			}
			else
			{
				component.DisableTalkingFlyAround();
			}
			Finish();
		}
	}
}
