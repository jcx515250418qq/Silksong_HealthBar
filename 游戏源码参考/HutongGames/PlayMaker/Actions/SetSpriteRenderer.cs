using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("GameObject")]
	[Tooltip("Set sprite renderer to active or inactive. Can only be one sprite renderer on object. ")]
	public class SetSpriteRenderer : FsmStateAction
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
					SpriteRenderer component = ownerDefaultTarget.GetComponent<SpriteRenderer>();
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
