using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Transform)]
	[Tooltip("Clamps a position to min/max ranges. Set any limit to None to leave un-clamped.")]
	public class ClampPosition : ComponentAction<Transform>
	{
		[RequiredField]
		[Tooltip("The GameObject to clamp position.")]
		public FsmOwnerDefault gameObject;

		[Tooltip("Clamp the minimum value of x.")]
		public FsmFloat minX;

		[Tooltip("Clamp the maximum value of x.")]
		public FsmFloat maxX;

		[Tooltip("Clamp the minimum value of y.")]
		public FsmFloat minY;

		[Tooltip("Clamp the maximum value of y.")]
		public FsmFloat maxY;

		[Tooltip("Clamp the minimum value of z.")]
		public FsmFloat minZ;

		[Tooltip("Clamp the maximum value of z.")]
		public FsmFloat maxZ;

		[Tooltip("Clamp position in local (relative to parent) or world space.")]
		public Space space;

		[Tooltip("Repeat every frame")]
		public bool everyFrame;

		[Tooltip("Perform in LateUpdate. This is useful if you want to clamp the position of objects that are animated or otherwise positioned in Update.")]
		public bool lateUpdate;

		public override void Reset()
		{
			gameObject = null;
			minX = new FsmFloat
			{
				UseVariable = true
			};
			maxX = new FsmFloat
			{
				UseVariable = true
			};
			minY = new FsmFloat
			{
				UseVariable = true
			};
			maxY = new FsmFloat
			{
				UseVariable = true
			};
			minZ = new FsmFloat
			{
				UseVariable = true
			};
			maxZ = new FsmFloat
			{
				UseVariable = true
			};
			space = Space.Self;
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
			if (!everyFrame && !lateUpdate)
			{
				DoClampPosition();
				Finish();
			}
		}

		public override void OnUpdate()
		{
			if (!lateUpdate)
			{
				DoClampPosition();
			}
		}

		public override void OnLateUpdate()
		{
			DoClampPosition();
			if (lateUpdate)
			{
				DoClampPosition();
			}
			if (!everyFrame)
			{
				Finish();
			}
		}

		private void DoClampPosition()
		{
			if (UpdateCachedTransform(base.Fsm.GetOwnerDefaultTarget(gameObject)))
			{
				Vector3 vector = ((space == Space.World) ? base.cachedTransform.position : base.cachedTransform.localPosition);
				if (!minX.IsNone)
				{
					vector.x = Mathf.Max(minX.Value, vector.x);
				}
				if (!maxX.IsNone)
				{
					vector.x = Mathf.Min(maxX.Value, vector.x);
				}
				if (!minY.IsNone)
				{
					vector.y = Mathf.Max(minY.Value, vector.y);
				}
				if (!maxY.IsNone)
				{
					vector.y = Mathf.Min(maxY.Value, vector.y);
				}
				if (!minZ.IsNone)
				{
					vector.z = Mathf.Max(minZ.Value, vector.z);
				}
				if (!maxZ.IsNone)
				{
					vector.z = Mathf.Min(maxZ.Value, vector.z);
				}
				if (space == Space.World)
				{
					base.cachedTransform.position = vector;
				}
				else
				{
					base.cachedTransform.localPosition = vector;
				}
			}
		}
	}
}
