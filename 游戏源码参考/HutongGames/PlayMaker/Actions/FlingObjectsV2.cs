using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.GameObject)]
	[Tooltip("Sets the velocity of all children of chosen GameObject")]
	public class FlingObjectsV2 : RigidBody2dActionBase
	{
		[RequiredField]
		[Tooltip("Object containing the objects to be flung.")]
		public FsmGameObject containerObject;

		public FsmVector3 adjustPosition;

		public FsmBool randomisePosition;

		[Tooltip("Minimum speed clones are fired at.")]
		public FsmFloat speedMin;

		[Tooltip("Maximum speed clones are fired at.")]
		public FsmFloat speedMax;

		[Tooltip("Minimum angle clones are fired at.")]
		public FsmFloat angleMin;

		[Tooltip("Maximum angle clones are fired at.")]
		public FsmFloat angleMax;

		public Space space;

		public bool unsetKinematic;

		private float vectorX;

		private float vectorY;

		private bool originAdjusted;

		public override void Reset()
		{
			containerObject = null;
			adjustPosition = null;
			speedMin = null;
			speedMax = null;
			angleMin = null;
			angleMax = null;
			space = Space.World;
		}

		public override void OnEnter()
		{
			GameObject value = containerObject.Value;
			if (value != null)
			{
				int childCount = value.transform.childCount;
				Vector2 vector = default(Vector2);
				for (int i = 1; i <= childCount; i++)
				{
					GameObject gameObject = value.transform.GetChild(i - 1).gameObject;
					CacheRigidBody2d(gameObject);
					if (!(rb2d != null))
					{
						continue;
					}
					float num = UnityEngine.Random.Range(speedMin.Value, speedMax.Value);
					float num2 = UnityEngine.Random.Range(angleMin.Value, angleMax.Value);
					vectorX = num * Mathf.Cos(num2 * (MathF.PI / 180f));
					vectorY = num * Mathf.Sin(num2 * (MathF.PI / 180f));
					vector.x = vectorX;
					vector.y = vectorY;
					if (space == Space.Self)
					{
						vector = value.transform.TransformVector(vector);
					}
					if (unsetKinematic & rb2d.isKinematic)
					{
						rb2d.isKinematic = false;
					}
					rb2d.linearVelocity = vector;
					if (!adjustPosition.IsNone)
					{
						if (randomisePosition.Value)
						{
							gameObject.transform.position = new Vector3(gameObject.transform.position.x + UnityEngine.Random.Range(0f - adjustPosition.Value.x, adjustPosition.Value.x), gameObject.transform.position.y + UnityEngine.Random.Range(0f - adjustPosition.Value.y, adjustPosition.Value.y), gameObject.transform.position.z);
						}
						else
						{
							gameObject.transform.position += adjustPosition.Value;
						}
					}
				}
			}
			Finish();
		}
	}
}
