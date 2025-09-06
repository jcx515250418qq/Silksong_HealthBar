using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.GameObject)]
	[Tooltip("Spawns a random amount of chosen GameObject and fires them off in random directions.")]
	public class SpawnRandomObjectsVelocity : RigidBody2dActionBase
	{
		[RequiredField]
		[Tooltip("GameObject to create.")]
		public FsmGameObject gameObject;

		[Tooltip("GameObject to spawn at (optional).")]
		public FsmGameObject spawnPoint;

		[Tooltip("Position. If a Spawn Point is defined, this is used as a local offset from the Spawn Point position.")]
		public FsmVector3 position;

		[Tooltip("Minimum amount of clones to be spawned.")]
		public FsmInt spawnMin;

		[Tooltip("Maximum amount of clones to be spawned.")]
		public FsmInt spawnMax;

		public FsmFloat speedMinX;

		public FsmFloat speedMaxX;

		public FsmFloat speedMinY;

		public FsmFloat speedMaxY;

		[Tooltip("Randomises spawn points of objects within this range. Leave as 0 and all objects will spawn at same point.")]
		public FsmFloat originVariationX;

		public FsmFloat originVariationY;

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
			speedMinX = null;
			speedMaxX = null;
			speedMinY = null;
			speedMaxY = null;
			originVariationX = null;
			originVariationY = null;
		}

		public override void OnEnter()
		{
			GameObject value = this.gameObject.Value;
			if (value != null)
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
				int num = Random.Range(spawnMin.Value, spawnMax.Value + 1);
				for (int i = 1; i <= num; i++)
				{
					GameObject gameObject = Object.Instantiate(value, vector, Quaternion.Euler(zero));
					float x = gameObject.transform.position.x;
					float y = gameObject.transform.position.y;
					float z = gameObject.transform.position.z;
					if (originVariationX != null)
					{
						x = gameObject.transform.position.x + Random.Range(0f - originVariationX.Value, originVariationX.Value);
						originAdjusted = true;
					}
					if (originVariationY != null)
					{
						y = gameObject.transform.position.y + Random.Range(0f - originVariationY.Value, originVariationY.Value);
						originAdjusted = true;
					}
					if (originAdjusted)
					{
						gameObject.transform.position = new Vector3(x, y, z);
					}
					CacheRigidBody2d(gameObject);
					float x2 = Random.Range(speedMinX.Value, speedMaxX.Value);
					float y2 = Random.Range(speedMinY.Value, speedMaxY.Value);
					Vector2 linearVelocity = new Vector2(x2, y2);
					rb2d.linearVelocity = linearVelocity;
				}
			}
			Finish();
		}
	}
}
