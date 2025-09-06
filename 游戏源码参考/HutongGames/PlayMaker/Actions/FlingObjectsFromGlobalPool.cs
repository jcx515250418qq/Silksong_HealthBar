using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.GameObject)]
	[Tooltip("Spawns a random amount of chosen GameObject from global pool and fires them off in random directions.")]
	public class FlingObjectsFromGlobalPool : RigidBody2dActionBase
	{
		[RequiredField]
		[Tooltip("GameObject to spawn.")]
		public FsmGameObject gameObject;

		[Tooltip("GameObject to spawn at (optional).")]
		public FsmGameObject spawnPoint;

		[Tooltip("Position. If a Spawn Point is defined, this is used as a local offset from the Spawn Point position.")]
		public FsmVector3 position;

		[Tooltip("Minimum amount of objects to be spawned.")]
		public FsmInt spawnMin;

		[Tooltip("Maximum amount of objects to be spawned.")]
		public FsmInt spawnMax;

		[Tooltip("Minimum speed objects are fired at.")]
		public FsmFloat speedMin;

		[Tooltip("Maximum speed objects are fired at.")]
		public FsmFloat speedMax;

		[Tooltip("Minimum angle objects are fired at.")]
		public FsmFloat angleMin;

		[Tooltip("Maximum angle objects are fired at.")]
		public FsmFloat angleMax;

		[Tooltip("Randomises spawn points of objects within this range. Leave as 0 and all objects will spawn at same point.")]
		public FsmFloat originVariationX;

		public FsmFloat originVariationY;

		[Tooltip("Optional: Name of FSM on object you want to send an event to after spawn")]
		public FsmString FSM;

		[Tooltip("Optional: Event you want to send to object after spawn")]
		public FsmString FSMEvent;

		private float vectorX;

		private float vectorY;

		private bool originAdjusted;

		public override void Reset()
		{
			gameObject = null;
			spawnPoint = null;
			position = new FsmVector3
			{
				UseVariable = true
			};
			spawnMin = null;
			spawnMax = null;
			speedMin = null;
			speedMax = null;
			angleMin = null;
			angleMax = null;
			originVariationX = null;
			originVariationY = null;
			FSM = new FsmString
			{
				UseVariable = true
			};
			FSMEvent = new FsmString
			{
				UseVariable = true
			};
		}

		public override void OnEnter()
		{
			if (this.gameObject.Value != null)
			{
				Vector3 vector = Vector3.zero;
				Vector3 zero = Vector3.zero;
				if (spawnPoint.Value != null)
				{
					vector = spawnPoint.Value.transform.position;
					if (!position.IsNone)
					{
						vector += position.Value;
					}
				}
				else if (!position.IsNone)
				{
					vector = position.Value;
				}
				int num = UnityEngine.Random.Range(spawnMin.Value, spawnMax.Value + 1);
				Vector2 linearVelocity = default(Vector2);
				for (int i = 1; i <= num; i++)
				{
					GameObject gameObject = this.gameObject.Value.Spawn(vector, Quaternion.Euler(zero));
					float x = gameObject.transform.position.x;
					float y = gameObject.transform.position.y;
					float z = gameObject.transform.position.z;
					if (originVariationX != null)
					{
						x = gameObject.transform.position.x + UnityEngine.Random.Range(0f - originVariationX.Value, originVariationX.Value);
						originAdjusted = true;
					}
					if (originVariationY != null)
					{
						y = gameObject.transform.position.y + UnityEngine.Random.Range(0f - originVariationY.Value, originVariationY.Value);
						originAdjusted = true;
					}
					if (originAdjusted)
					{
						gameObject.transform.position = new Vector3(x, y, z);
					}
					BlackThreadState.HandleDamagerSpawn(base.Owner, gameObject);
					CacheRigidBody2d(gameObject);
					float num2 = UnityEngine.Random.Range(speedMin.Value, speedMax.Value);
					float num3 = UnityEngine.Random.Range(angleMin.Value, angleMax.Value);
					vectorX = num2 * Mathf.Cos(num3 * (MathF.PI / 180f));
					vectorY = num2 * Mathf.Sin(num3 * (MathF.PI / 180f));
					linearVelocity.x = vectorX;
					linearVelocity.y = vectorY;
					rb2d.linearVelocity = linearVelocity;
					if (!FSM.IsNone)
					{
						FSMUtility.LocateFSM(gameObject, FSM.Value).SendEvent(FSMEvent.Value);
					}
				}
			}
			Finish();
		}

		private string GetFriendlyNameForAngle(float angle)
		{
			angle = Mathf.Repeat(angle, 360f);
			if (angle >= 45f && angle < 135f)
			{
				return "Up";
			}
			if (angle >= 135f && angle < 225f)
			{
				return "Left";
			}
			if (angle >= 225f && angle < 315f)
			{
				return "Down";
			}
			return "Right";
		}
	}
}
