using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Transform)]
	[Tooltip("Sets the Scale of a Game Object. To leave any axis unchanged, set variable to 'None'.")]
	public class SetScaleDelay : FsmStateAction
	{
		[RequiredField]
		[Tooltip("The GameObject to scale.")]
		public FsmOwnerDefault gameObject;

		[UIHint(UIHint.Variable)]
		[Tooltip("Use stored Vector3 value, and/or set each axis below.")]
		public FsmVector3 vector;

		public FsmFloat x;

		public FsmFloat y;

		public FsmFloat z;

		public FsmFloat delay;

		public FsmBool checkBool;

		private float timer;

		public override void Reset()
		{
			gameObject = null;
			vector = null;
			x = new FsmFloat
			{
				UseVariable = true
			};
			y = new FsmFloat
			{
				UseVariable = true
			};
			z = new FsmFloat
			{
				UseVariable = true
			};
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
				DoSetScale();
			}
			Finish();
		}

		private void DoSetScale()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(gameObject);
			if (!(ownerDefaultTarget == null))
			{
				Vector3 localScale = (vector.IsNone ? ownerDefaultTarget.transform.localScale : vector.Value);
				if (!x.IsNone)
				{
					localScale.x = x.Value;
				}
				if (!y.IsNone)
				{
					localScale.y = y.Value;
				}
				if (!z.IsNone)
				{
					localScale.z = z.Value;
				}
				ownerDefaultTarget.transform.localScale = localScale;
			}
		}
	}
}
