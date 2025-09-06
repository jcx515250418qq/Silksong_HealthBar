using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Sprite Renderer")]
	public class SetSpriteRendererOrder : FsmStateAction
	{
		[RequiredField]
		public FsmOwnerDefault gameObject;

		public FsmInt order;

		public FsmFloat delay;

		private float timer;

		public override void Reset()
		{
			gameObject = null;
			order = null;
			delay = null;
			timer = 0f;
		}

		public override void OnEnter()
		{
			timer = 0f;
			if ((delay.IsNone || delay.Value == 0f) && gameObject != null)
			{
				GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(gameObject);
				if (ownerDefaultTarget != null)
				{
					SpriteRenderer component = ownerDefaultTarget.GetComponent<SpriteRenderer>();
					if (component != null)
					{
						component.sortingOrder = order.Value;
						return;
					}
				}
			}
			Finish();
		}

		public override void OnUpdate()
		{
			if (timer < delay.Value)
			{
				timer += Time.deltaTime;
				return;
			}
			if (gameObject != null)
			{
				GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(gameObject);
				if ((bool)ownerDefaultTarget)
				{
					SpriteRenderer component = ownerDefaultTarget.GetComponent<SpriteRenderer>();
					if (component != null)
					{
						component.sortingOrder = order.Value;
					}
				}
			}
			Finish();
		}
	}
}
