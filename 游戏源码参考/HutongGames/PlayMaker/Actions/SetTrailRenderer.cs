using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Trail Renderer")]
	[Tooltip("Set trail renderer parameters")]
	public class SetTrailRenderer : FsmStateAction
	{
		[RequiredField]
		[Tooltip("The particle emitting GameObject")]
		public FsmOwnerDefault gameObject;

		public FsmFloat startWidth;

		public FsmFloat endWidth;

		public FsmFloat time;

		public bool everyFrame;

		private TrailRenderer trail;

		public override void Reset()
		{
			gameObject = null;
			startWidth = new FsmFloat
			{
				UseVariable = true
			};
			endWidth = new FsmFloat
			{
				UseVariable = true
			};
			time = new FsmFloat
			{
				UseVariable = true
			};
		}

		public override void OnEnter()
		{
			if (gameObject != null)
			{
				GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(gameObject);
				trail = ownerDefaultTarget.GetComponent<TrailRenderer>();
				if (trail == null)
				{
					Finish();
				}
				DoSetTrail();
				if (!everyFrame)
				{
					Finish();
				}
			}
			else
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			DoSetTrail();
		}

		private void DoSetTrail()
		{
			if (!startWidth.IsNone)
			{
				trail.startWidth = startWidth.Value;
			}
			if (!endWidth.IsNone)
			{
				trail.endWidth = endWidth.Value;
			}
			if (!time.IsNone)
			{
				trail.time = time.Value;
			}
		}
	}
}
