using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Transform)]
	[Tooltip("Sets the Scale of a Game Object. To leave any axis unchanged, set variable to 'None'.")]
	public class SetScale : FsmStateAction
	{
		[RequiredField]
		[Tooltip("The GameObject to scale.")]
		public FsmOwnerDefault gameObject;

		[UIHint(UIHint.Variable)]
		[Tooltip("Use stored Vector3 value, and/or set each axis below.")]
		public FsmVector3 vector;

		[Tooltip("Scale along the X axis (1 = normal).")]
		public FsmFloat x;

		[Tooltip("Scale along the Y axis (1 = normal).")]
		public FsmFloat y;

		[Tooltip("Scale along the Z axis (1 = normal).")]
		public FsmFloat z;

		[Tooltip("Repeat every frame.")]
		public bool everyFrame;

		[Tooltip("Perform in LateUpdate. This is useful if you want to override the position of objects that are animated or otherwise positioned in Update.")]
		public bool lateUpdate;

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
			everyFrame = false;
			lateUpdate = false;
		}

		public override void OnPreprocess()
		{
			if (lateUpdate)
			{
				base.Fsm.HandleLateUpdate = true;
			}
		}

		public override void OnEnter()
		{
			DoSetScale();
			if (!everyFrame)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			if (!lateUpdate)
			{
				DoSetScale();
			}
		}

		public override void OnLateUpdate()
		{
			if (lateUpdate)
			{
				DoSetScale();
			}
			if (!everyFrame)
			{
				Finish();
			}
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
