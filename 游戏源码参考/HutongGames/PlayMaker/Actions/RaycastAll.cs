using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Physics)]
	[Tooltip("Casts a Ray against all Colliders in the scene. Use either a GameObject or Vector3 world position as the origin of the ray. Use GetRaycastAllInfo to get more detailed info.")]
	public class RaycastAll : FsmStateAction
	{
		public static RaycastHit[] RaycastAllHitInfo;

		[Tooltip("Start ray at game object position. \nOr use From Position parameter.")]
		public FsmOwnerDefault fromGameObject;

		[Tooltip("Start ray at a vector3 world position. \nOr use Game Object parameter.")]
		public FsmVector3 fromPosition;

		[Tooltip("A vector3 direction vector")]
		public FsmVector3 direction;

		[Tooltip("Cast the ray in world or local space. Note if no Game Object is specified, the direction is in world space.")]
		public Space space;

		[Tooltip("The length of the ray. Set to -1 for infinity.")]
		public FsmFloat distance;

		[ActionSection("Result")]
		[Tooltip("Event to send if the ray hits an object.")]
		[UIHint(UIHint.Variable)]
		public FsmEvent hitEvent;

		[Tooltip("Set a bool variable to true if hit something, otherwise false.")]
		[UIHint(UIHint.Variable)]
		public FsmBool storeDidHit;

		[Tooltip("Store the GameObjects hit in an array variable.")]
		[UIHint(UIHint.Variable)]
		[ArrayEditor(VariableType.GameObject, "", 0, 0, 65536)]
		public FsmArray storeHitObjects;

		[UIHint(UIHint.Variable)]
		[Tooltip("Get the world position of the ray hit point and store it in a variable.")]
		public FsmVector3 storeHitPoint;

		[UIHint(UIHint.Variable)]
		[Tooltip("Get the normal at the hit point and store it in a variable.\nNote, this is a direction vector not a rotation. Use Look At Direction to rotate a GameObject to this direction.")]
		public FsmVector3 storeHitNormal;

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

		private int repeat;

		public override void Reset()
		{
			fromGameObject = null;
			fromPosition = new FsmVector3
			{
				UseVariable = true
			};
			direction = new FsmVector3
			{
				UseVariable = true
			};
			space = Space.Self;
			distance = new FsmFloat
			{
				Value = 100f
			};
			hitEvent = null;
			storeDidHit = null;
			storeHitObjects = null;
			storeHitPoint = null;
			storeHitNormal = null;
			storeHitDistance = null;
			repeatInterval = new FsmInt
			{
				Value = 1
			};
			layerMask = new FsmInt[0];
			invertMask = null;
			debugColor = new FsmColor
			{
				Value = Color.yellow
			};
			debug = null;
		}

		public override void OnEnter()
		{
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
			if (distance.Value == 0f)
			{
				return;
			}
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(fromGameObject);
			Vector3 vector = ((ownerDefaultTarget != null) ? ownerDefaultTarget.transform.position : fromPosition.Value);
			float num = float.PositiveInfinity;
			if (distance.Value > 0f)
			{
				num = distance.Value;
			}
			Vector3 vector2 = direction.Value;
			if (ownerDefaultTarget != null && space == Space.Self)
			{
				vector2 = ownerDefaultTarget.transform.TransformDirection(direction.Value);
			}
			RaycastAllHitInfo = Physics.RaycastAll(vector, vector2, num, ActionHelpers.LayerArrayToLayerMask(layerMask, invertMask.Value));
			bool flag = RaycastAllHitInfo.Length != 0;
			storeDidHit.Value = flag;
			if (flag)
			{
				GameObject[] array = new GameObject[RaycastAllHitInfo.Length];
				for (int i = 0; i < RaycastAllHitInfo.Length; i++)
				{
					RaycastHit raycastHit = RaycastAllHitInfo[i];
					array[i] = raycastHit.collider.gameObject;
				}
				FsmArray fsmArray = storeHitObjects;
				object[] values = array;
				fsmArray.Values = values;
				storeHitPoint.Value = base.Fsm.RaycastHitInfo.point;
				storeHitNormal.Value = base.Fsm.RaycastHitInfo.normal;
				storeHitDistance.Value = base.Fsm.RaycastHitInfo.distance;
				base.Fsm.Event(hitEvent);
			}
			if (debug.Value)
			{
				float num2 = Mathf.Min(num, 1000f);
				Debug.DrawLine(vector, vector + vector2 * num2, debugColor.Value);
			}
		}
	}
}
