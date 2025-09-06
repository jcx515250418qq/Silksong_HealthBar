using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.GameObject)]
	[Tooltip("Measures the Distance between a GameObject and a target GameObject/Position. If both GameObject and Position are defined, position is taken a local offset from the GameObject's position.")]
	public class GetDistanceXYZ : ComponentAction<Transform>
	{
		[RequiredField]
		[Tooltip("Measure distance from this GameObject.")]
		public FsmOwnerDefault gameObject;

		[Tooltip("Measure distance to this GameObject (or set world position below).")]
		public FsmGameObject target;

		[Tooltip("World position or local offset from target GameObject, if defined.")]
		public FsmVector3 position;

		[UIHint(UIHint.Variable)]
		[Tooltip("Store the distance in a float variable.")]
		public FsmFloat storeDistance;

		[Tooltip("Space used to measure the distance in. E.g. along the world X axis or the GameObject's local X axis.")]
		public Space space;

		[UIHint(UIHint.Variable)]
		[Tooltip("Store the distance along the X axis.")]
		public FsmFloat storeXDistance;

		[UIHint(UIHint.Variable)]
		[Tooltip("Store the distance along the Y axis.")]
		public FsmFloat storeYDistance;

		[UIHint(UIHint.Variable)]
		[Tooltip("Store the distance along the Z axis.")]
		public FsmFloat storeZDistance;

		[Tooltip("Repeat every frame.")]
		public bool everyFrame;

		private GameObject cachedTargetGameObject;

		private Transform targetTransform;

		private Transform gameObjectTransform => cachedComponent;

		public override void Reset()
		{
			gameObject = null;
			target = null;
			position = null;
			storeDistance = null;
			space = Space.World;
			storeXDistance = null;
			storeYDistance = null;
			storeZDistance = null;
			everyFrame = true;
		}

		public override void OnEnter()
		{
			DoGetDistanceXYZ();
			if (!everyFrame)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			DoGetDistanceXYZ();
		}

		private void DoGetDistanceXYZ()
		{
			if (!UpdateCache(base.Fsm.GetOwnerDefaultTarget(gameObject)))
			{
				return;
			}
			if (target.Value != null && cachedTargetGameObject != target.Value)
			{
				cachedTargetGameObject = target.Value;
				targetTransform = cachedTargetGameObject.transform;
			}
			Vector3 b = Vector3.zero;
			if (position.IsNone)
			{
				if (targetTransform != null)
				{
					b = targetTransform.position;
				}
			}
			else
			{
				b = ((targetTransform == null) ? position.Value : targetTransform.TransformPoint(position.Value));
			}
			if (!storeDistance.IsNone)
			{
				storeDistance.Value = Vector3.Distance(gameObjectTransform.position, b);
			}
			if (!storeXDistance.IsNone || !storeYDistance.IsNone || !storeZDistance.IsNone)
			{
				if (space == Space.Self)
				{
					b = gameObjectTransform.InverseTransformPoint(b);
				}
				else
				{
					b -= gameObjectTransform.position;
				}
				storeXDistance.Value = b.x;
				storeYDistance.Value = b.y;
				storeZDistance.Value = b.z;
			}
		}
	}
}
