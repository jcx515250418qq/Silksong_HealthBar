using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Physics2D)]
	[Tooltip("Iterate through a list of all colliders that fall within a rectangular area.The colliders iterated are sorted in order of increasing Z coordinate. No iteration will take place if there are no colliders within the area.")]
	public class GetNextOverlapArea2d : FsmStateAction
	{
		[ActionSection("Setup")]
		[Tooltip("First corner of the rectangle area using the game object position. \nOr use firstCornerPosition parameter.")]
		public FsmOwnerDefault firstCornerGameObject;

		[Tooltip("First Corner of the rectangle area as a world position. \nOr use FirstCornerGameObject parameter. If both define, will add firstCornerPosition to the FirstCornerGameObject position")]
		public FsmVector2 firstCornerPosition;

		[Tooltip("Second corner of the rectangle area using the game object position. \nOr use secondCornerPosition parameter.")]
		public FsmGameObject secondCornerGameObject;

		[Tooltip("Second Corner rectangle area as a world position. \nOr use SecondCornerGameObject parameter. If both define, will add secondCornerPosition to the SecondCornerGameObject position")]
		public FsmVector2 secondCornerPosition;

		[Tooltip("Only include objects with a Z coordinate (depth) greater than this value. leave to none for no effect")]
		public FsmInt minDepth;

		[Tooltip("Only include objects with a Z coordinate (depth) less than this value. leave to none")]
		public FsmInt maxDepth;

		[Tooltip("If you want to reset the iteration, raise this flag to true when you enter the state, it will indicate you want to start from the beginning again")]
		[UIHint(UIHint.Variable)]
		public FsmBool resetFlag;

		[ActionSection("Filter")]
		[UIHint(UIHint.Layer)]
		[Tooltip("Pick only from these layers.")]
		public FsmInt[] layerMask;

		[Tooltip("Invert the mask, so you pick from all layers except those defined above.")]
		public FsmBool invertMask;

		[ActionSection("Result")]
		[Tooltip("Store the number of colliders found for this overlap.")]
		[UIHint(UIHint.Variable)]
		public FsmInt collidersCount;

		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the next collider in a GameObject variable.")]
		public FsmGameObject storeNextCollider;

		[Tooltip("Event to send to get the next collider.")]
		public FsmEvent loopEvent;

		[Tooltip("Event to send when there are no more colliders to iterate.")]
		public FsmEvent finishedEvent;

		private Collider2D[] colliders;

		private int colliderCount;

		private int nextColliderIndex;

		public override void Reset()
		{
			firstCornerGameObject = null;
			firstCornerPosition = new FsmVector2
			{
				UseVariable = true
			};
			secondCornerGameObject = null;
			secondCornerPosition = new FsmVector2
			{
				UseVariable = true
			};
			minDepth = new FsmInt
			{
				UseVariable = true
			};
			maxDepth = new FsmInt
			{
				UseVariable = true
			};
			layerMask = new FsmInt[0];
			invertMask = false;
			resetFlag = null;
			collidersCount = null;
			storeNextCollider = null;
			loopEvent = null;
			finishedEvent = null;
		}

		public override void OnEnter()
		{
			if (colliders == null || resetFlag.Value)
			{
				nextColliderIndex = 0;
				colliders = GetOverlapAreaAll();
				colliderCount = colliders.Length;
				collidersCount.Value = colliderCount;
				resetFlag.Value = false;
			}
			DoGetNextCollider();
			Finish();
		}

		private void DoGetNextCollider()
		{
			if (nextColliderIndex >= colliderCount)
			{
				nextColliderIndex = 0;
				base.Fsm.Event(finishedEvent);
				return;
			}
			storeNextCollider.Value = colliders[nextColliderIndex].gameObject;
			if (nextColliderIndex >= colliderCount)
			{
				colliders = null;
				nextColliderIndex = 0;
				base.Fsm.Event(finishedEvent);
				return;
			}
			nextColliderIndex++;
			if (loopEvent != null)
			{
				base.Fsm.Event(loopEvent);
			}
		}

		private Collider2D[] GetOverlapAreaAll()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(firstCornerGameObject);
			Vector2 value = firstCornerPosition.Value;
			if (ownerDefaultTarget != null)
			{
				value.x += ownerDefaultTarget.transform.position.x;
				value.y += ownerDefaultTarget.transform.position.y;
			}
			GameObject value2 = secondCornerGameObject.Value;
			Vector2 value3 = secondCornerPosition.Value;
			if (value2 != null)
			{
				value3.x += value2.transform.position.x;
				value3.y += value2.transform.position.y;
			}
			if (minDepth.IsNone && maxDepth.IsNone)
			{
				return Physics2D.OverlapAreaAll(value, value3, ActionHelpers.LayerArrayToLayerMask(layerMask, invertMask.Value));
			}
			float num = (minDepth.IsNone ? float.NegativeInfinity : ((float)minDepth.Value));
			float num2 = (maxDepth.IsNone ? float.PositiveInfinity : ((float)maxDepth.Value));
			return Physics2D.OverlapAreaAll(value, value3, ActionHelpers.LayerArrayToLayerMask(layerMask, invertMask.Value), num, num2);
		}
	}
}
