using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Transform)]
	[Tooltip("Sets the Scale of a Game Object. To leave any axis unchanged, set variable to 'None'.")]
	public class FlipScaleDelay : FsmStateAction
	{
		[RequiredField]
		[Tooltip("The GameObject to scale.")]
		public FsmOwnerDefault gameObject;

		public bool flipHorizontally;

		public bool flipVertically;

		public FsmFloat delay;

		public FsmBool checkBool;

		private float timer;

		public override void Reset()
		{
			flipHorizontally = false;
			flipVertically = false;
			delay = null;
			checkBool = null;
		}

		public override void OnEnter()
		{
			timer = 0f;
		}

		public override void OnUpdate()
		{
			if (timer < delay.Value)
			{
				timer += Time.deltaTime;
				return;
			}
			if (checkBool.Value || checkBool.IsNone)
			{
				if (checkBool.Value)
				{
					checkBool.Value = false;
				}
				DoFlipScale();
			}
			Finish();
		}

		private void DoFlipScale()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(gameObject);
			if (!(ownerDefaultTarget == null))
			{
				Vector3 localScale = ownerDefaultTarget.transform.localScale;
				if (flipHorizontally)
				{
					localScale.x = 0f - localScale.x;
				}
				if (flipVertically)
				{
					localScale.y = 0f - localScale.y;
				}
				ownerDefaultTarget.transform.localScale = localScale;
			}
		}
	}
}
