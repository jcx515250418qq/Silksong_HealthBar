using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.GameObject)]
	[Tooltip("Spawns a random amount of chosen GameObject and fires them off in random directions.")]
	public class CreatePoolObjects : RigidBody2dActionBase
	{
		[RequiredField]
		[Tooltip("GameObject to create.")]
		public FsmGameObject gameObject;

		[RequiredField]
		[Tooltip("GameObject which will be used as pool (spawned objects will be parented to this).")]
		public FsmGameObject pool;

		[Tooltip("Position. If a Spawn Point is defined, this is used as a local offset from the Spawn Point position.")]
		public FsmVector3 position;

		[Tooltip("Amount of clones to be spawned.")]
		public FsmInt amount;

		[Tooltip("Randomises spawn points of objects within this range. Leave as 0 and all objects will spawn at same point.")]
		public FsmFloat originVariationX;

		public FsmFloat originVariationY;

		[Tooltip("Deactivate the pool objects after creating. Use if the objects don't deactivate themselves.")]
		public bool deactivate;

		private float vectorX;

		private float vectorY;

		private bool originAdjusted;

		public override void Reset()
		{
			gameObject = null;
			position = new FsmVector3
			{
				UseVariable = true
			};
			amount = null;
			originVariationX = null;
			originVariationY = null;
			deactivate = false;
		}

		public override void OnEnter()
		{
			GameObject value = this.gameObject.Value;
			if (value != null)
			{
				Vector3 vector = pool.Value.transform.position;
				Vector3 zero = Vector3.zero;
				if (!position.IsNone)
				{
					vector = position.Value + vector;
				}
				int value2 = amount.Value;
				for (int i = 1; i <= value2; i++)
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
					gameObject.transform.parent = pool.Value.transform;
					if (deactivate)
					{
						gameObject.SetActive(value: false);
					}
				}
			}
			Finish();
		}
	}
}
