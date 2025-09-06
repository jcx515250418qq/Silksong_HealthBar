using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.GameObject)]
	[Tooltip("Destroys a Game Object.")]
	public class DestroyObject : FsmStateAction
	{
		[RequiredField]
		[Tooltip("The GameObject to destroy.")]
		public FsmGameObject gameObject;

		[HasFloatSlider(0f, 5f)]
		[Tooltip("Optional delay before destroying the Game Object.")]
		public FsmFloat delay;

		[Tooltip("Detach children before destroying the Game Object.")]
		public FsmBool detachChildren;

		public override void Reset()
		{
			gameObject = null;
			delay = 0f;
		}

		public override void OnEnter()
		{
			GameObject value = gameObject.Value;
			if (value != null)
			{
				if (delay.Value <= 0f)
				{
					Object.Destroy(value);
				}
				else
				{
					Object.Destroy(value, delay.Value);
				}
				if (detachChildren.Value)
				{
					value.transform.DetachChildren();
				}
			}
			Finish();
		}

		public override void OnUpdate()
		{
		}
	}
}
