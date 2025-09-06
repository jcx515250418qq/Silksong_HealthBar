using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.GameObject)]
	[Tooltip("Spawns a random amount of chosen GameObject over time and fires them off in random directions.")]
	public class SpawnRandomObjectsOverTimeV2 : RigidBody2dActionBase
	{
		[RequiredField]
		[Tooltip("GameObject to create.")]
		public FsmGameObject gameObject;

		[Tooltip("GameObject to spawn at (optional).")]
		public FsmGameObject spawnPoint;

		[Tooltip("Position. If a Spawn Point is defined, this is used as a local offset from the Spawn Point position.")]
		public FsmVector3 position;

		[Tooltip("How often, in seconds, spawn occurs.")]
		public FsmFloat frequency;

		[Tooltip("Minimum amount of clones to be spawned.")]
		public FsmInt spawnMin;

		[Tooltip("Maximum amount of clones to be spawned.")]
		public FsmInt spawnMax;

		[Tooltip("Minimum speed clones are fired at.")]
		public FsmFloat speedMin;

		[Tooltip("Maximum speed clones are fired at.")]
		public FsmFloat speedMax;

		[Tooltip("Minimum angle clones are fired at.")]
		public FsmFloat angleMin;

		[Tooltip("Maximum angle clones are fired at.")]
		public FsmFloat angleMax;

		[Tooltip("Randomises spawn points of objects within this range. Leave as 0 and all objects will spawn at same point.")]
		public FsmFloat originVariationX;

		public FsmFloat originVariationY;

		[Tooltip("Minimum scale of clone.")]
		public FsmFloat scaleMin = 1f;

		[Tooltip("Maximum scale of clone.")]
		public FsmFloat scaleMax = 1f;

		private float vectorX;

		private float vectorY;

		private float timer;

		private bool originAdjusted;

		public override void OnEnter()
		{
		}

		public override void Reset()
		{
			gameObject = null;
			spawnPoint = null;
			position = new FsmVector3
			{
				UseVariable = true
			};
			spawnMin = null;
			frequency = null;
			spawnMax = null;
			speedMin = null;
			speedMax = null;
			angleMin = null;
			angleMax = null;
			originVariationX = null;
			originVariationY = null;
			scaleMin = 1f;
			scaleMax = 1f;
		}

		public override void OnUpdate()
		{
			DoSpawn();
		}

		private void DoSpawn()
		{
			timer += Time.deltaTime;
			if (!(timer >= frequency.Value))
			{
				return;
			}
			timer = 0f;
			GameObject value = this.gameObject.Value;
			if (!(value != null))
			{
				return;
			}
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
				GameObject gameObject = UnityEngine.Object.Instantiate(value, vector, Quaternion.Euler(zero));
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
				if (gameObject.GetComponent<Rigidbody2D>() != null)
				{
					CacheRigidBody2d(gameObject);
					float num2 = UnityEngine.Random.Range(speedMin.Value, speedMax.Value);
					float num3 = UnityEngine.Random.Range(angleMin.Value, angleMax.Value);
					vectorX = num2 * Mathf.Cos(num3 * (MathF.PI / 180f));
					vectorY = num2 * Mathf.Sin(num3 * (MathF.PI / 180f));
					linearVelocity.x = vectorX;
					linearVelocity.y = vectorY;
					rb2d.linearVelocity = linearVelocity;
				}
				if (scaleMin != null && scaleMax != null)
				{
					float num4 = UnityEngine.Random.Range(scaleMin.Value, scaleMax.Value);
					if (num4 != 1f)
					{
						gameObject.transform.localScale = new Vector3(num4, num4, num4);
					}
				}
			}
		}
	}
}
