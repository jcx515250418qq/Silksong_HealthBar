using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.GameObject)]
	[Tooltip("Spawns a prefab Game Object from the Global Object Pool on the Game Manager.")]
	public class SpawnObjectFromGlobalPoolDelay : FsmStateAction
	{
		[RequiredField]
		[Tooltip("GameObject to create. Usually a Prefab.")]
		public FsmGameObject gameObject;

		[Tooltip("Optional Spawn Point.")]
		public FsmGameObject spawnPoint;

		[Tooltip("Position. If a Spawn Point is defined, this is used as a local offset from the Spawn Point position.")]
		public FsmVector3 position;

		[Tooltip("Rotation. NOTE: Overrides the rotation of the Spawn Point.")]
		public FsmVector3 rotation;

		[UIHint(UIHint.Variable)]
		[Tooltip("Optionally store the created object.")]
		public FsmGameObject storeObject;

		public FsmFloat delayMin;

		public FsmFloat delayMax;

		private float timer;

		public override void Reset()
		{
			gameObject = null;
			spawnPoint = null;
			position = new FsmVector3
			{
				UseVariable = true
			};
			rotation = new FsmVector3
			{
				UseVariable = true
			};
			storeObject = null;
		}

		public override void OnEnter()
		{
			timer = Random.Range(delayMin.Value, delayMax.Value);
		}

		public override void OnUpdate()
		{
			if (timer > 0f)
			{
				timer -= Time.deltaTime;
			}
			else
			{
				SpawnObject();
			}
		}

		private void SpawnObject()
		{
			if (this.gameObject.Value != null)
			{
				Vector3 vector = Vector3.zero;
				Vector3 eulerAngles = Vector3.zero;
				if (spawnPoint.Value != null)
				{
					vector = spawnPoint.Value.transform.position;
					if (!position.IsNone)
					{
						vector += position.Value;
					}
					eulerAngles = ((!rotation.IsNone) ? rotation.Value : spawnPoint.Value.transform.eulerAngles);
				}
				else
				{
					if (!position.IsNone)
					{
						vector = position.Value;
					}
					if (!rotation.IsNone)
					{
						eulerAngles = rotation.Value;
					}
				}
				if (this.gameObject != null)
				{
					GameObject gameObject = this.gameObject.Value.Spawn();
					gameObject.transform.position = vector;
					gameObject.transform.eulerAngles = eulerAngles;
					storeObject.Value = gameObject;
					BlackThreadState.HandleDamagerSpawn(base.Owner, gameObject);
				}
			}
			Finish();
		}
	}
}
