using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("GameObject")]
	[Tooltip("Set Audio Source to active or inactive. Can only be one Audio Source on object. ")]
	public class SetAudioSource : FsmStateAction
	{
		[RequiredField]
		public FsmOwnerDefault gameObject;

		public FsmBool active;

		public override void Reset()
		{
			gameObject = null;
			active = false;
		}

		public override void OnEnter()
		{
			if (gameObject != null)
			{
				GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(gameObject);
				if (ownerDefaultTarget != null)
				{
					AudioSource component = ownerDefaultTarget.GetComponent<AudioSource>();
					if (component != null)
					{
						component.enabled = active.Value;
					}
				}
			}
			Finish();
		}
	}
}
