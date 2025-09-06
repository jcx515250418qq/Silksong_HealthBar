using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Physics2D)]
	[Tooltip("Casts a Ray against all Colliders in the scene.A linecast is an imaginary line between two points in world space. Any object making contact with the beam can be detected and reported. This differs from the similar raycast in that raycasting specifies the line using an origin and direction.Use GetRaycastHit2dInfo to get more detailed info.")]
	public class LineCast2d : FsmStateAction
	{
		[ActionSection("Setup")]
		[Tooltip("Start ray at game object position. \nOr use From Position parameter.")]
		public FsmOwnerDefault fromGameObject;

		[Tooltip("Start ray at a vector2 world position. \nOr use fromGameObject parameter. If both define, will add fromPosition to the fromGameObject position")]
		public FsmVector2 fromPosition;

		[Tooltip("End ray at game object position. \nOr use From Position parameter.")]
		public FsmGameObject toGameObject;

		[Tooltip("End ray at a vector2 world position. \nOr use fromGameObject parameter. If both define, will add toPosition to the ToGameObject position")]
		public FsmVector2 toPosition;

		[Tooltip("Only include objects with a Z coordinate (depth) greater than this value. leave to none for no effect")]
		public FsmInt minDepth;

		[Tooltip("Only include objects with a Z coordinate (depth) less than this value. leave to none")]
		public FsmInt maxDepth;

		[ActionSection("Result")]
		[Tooltip("Event to send if the ray hits an object.")]
		[UIHint(UIHint.Variable)]
		public FsmEvent hitEvent;

		[Tooltip("Set a bool variable to true if hit something, otherwise false.")]
		[UIHint(UIHint.Variable)]
		public FsmBool storeDidHit;

		[Tooltip("Store the game object hit in a variable.")]
		[UIHint(UIHint.Variable)]
		public FsmGameObject storeHitObject;

		[UIHint(UIHint.Variable)]
		[Tooltip("Get the 2d position of the ray hit point and store it in a variable.")]
		public FsmVector2 storeHitPoint;

		[UIHint(UIHint.Variable)]
		[Tooltip("Get the 2d normal at the hit point and store it in a variable.\nNote, this is a direction vector not a rotation.")]
		public FsmVector2 storeHitNormal;

		[UIHint(UIHint.Variable)]
		[Tooltip("Get the distance along the ray to the hit point and store it in a variable.")]
		public FsmFloat storeHitDistance;

		[ActionSection("Filter")]
		[Tooltip("Set how often to cast a ray. 0 = once, don't repeat; 1 = everyFrame; 2 = every other frame... \nBecause raycasts can get expensive use the highest repeat interval you can get away with.")]
		public FsmInt repeatInterval;

		[UIHint(UIHint.Layer)]
		[Tooltip("Pick only from these layers.")]
		public FsmInt[] layerMask;

		[Tooltip("Invert the mask, so you pick from all layers except those defined above.")]
		public FsmBool invertMask;

		[ActionSection("Debug")]
		[Tooltip("The color to use for the debug line.")]
		public FsmColor debugColor;

		[Tooltip("Draw a debug line. Note: Check Gizmos in the Game View to see it in game.")]
		public FsmBool debug;

		private Transform _fromTrans;

		private Transform _toTrans;

		private int repeat;

		public override void Reset()
		{
			fromGameObject = null;
			fromPosition = new FsmVector2
			{
				UseVariable = true
			};
			toGameObject = null;
			toPosition = new FsmVector2
			{
				UseVariable = true
			};
			hitEvent = null;
			storeDidHit = null;
			storeHitObject = null;
			storeHitPoint = null;
			storeHitNormal = null;
			storeHitDistance = null;
			repeatInterval = 1;
			layerMask = new FsmInt[0];
			invertMask = false;
			debugColor = Color.yellow;
			debug = false;
		}

		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(fromGameObject);
			if (ownerDefaultTarget != null)
			{
				_fromTrans = ownerDefaultTarget.transform;
			}
			GameObject value = toGameObject.Value;
			if (value != null)
			{
				_toTrans = value.transform;
			}
			DoRaycast();
			if (repeatInterval.Value == 0)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			repeat--;
			if (repeat == 0)
			{
				DoRaycast();
			}
		}

		private void DoRaycast()
		{
			repeat = repeatInterval.Value;
			Vector2 value = fromPosition.Value;
			if (_fromTrans != null)
			{
				value.x += _fromTrans.position.x;
				value.y += _fromTrans.position.y;
			}
			Vector2 value2 = toPosition.Value;
			if (_toTrans != null)
			{
				value2.x += _toTrans.position.x;
				value2.y += _toTrans.position.y;
			}
			RaycastHit2D info;
			if (minDepth.IsNone && maxDepth.IsNone)
			{
				info = Physics2D.Linecast(value, value2, ActionHelpers.LayerArrayToLayerMask(layerMask, invertMask.Value));
			}
			else
			{
				float num = (minDepth.IsNone ? float.NegativeInfinity : ((float)minDepth.Value));
				float num2 = (maxDepth.IsNone ? float.PositiveInfinity : ((float)maxDepth.Value));
				info = Physics2D.Linecast(value, value2, ActionHelpers.LayerArrayToLayerMask(layerMask, invertMask.Value), num, num2);
			}
			Fsm.RecordLastRaycastHit2DInfo(base.Fsm, info);
			bool flag = info.collider != null;
			storeDidHit.Value = flag;
			if (flag)
			{
				storeHitObject.Value = info.collider.gameObject;
				storeHitPoint.Value = info.point;
				storeHitNormal.Value = info.normal;
				storeHitDistance.Value = info.fraction;
				base.Fsm.Event(hitEvent);
			}
			if (debug.Value)
			{
				Vector3 start = new Vector3(value.x, value.y, 0f);
				Vector3 end = new Vector3(value2.x, value2.y, 0f);
				Debug.DrawLine(start, end, debugColor.Value);
			}
		}
	}
}
