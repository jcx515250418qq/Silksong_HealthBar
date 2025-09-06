using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.GameObject)]
	[ActionTarget(typeof(GameObject), "gameObject", true)]
	[Tooltip("Creates a Game Object, usually using a Prefab.")]
	public class CreateObjectV2 : FsmStateAction
	{
		[RequiredField]
		[Tooltip("GameObject to create. Usually a Prefab.")]
		public FsmGameObject gameObject;

		public FsmBool canPreSpawn;

		[Tooltip("Optional Spawn Point.")]
		public FsmGameObject spawnPoint;

		[Tooltip("Position. If a Spawn Point is defined, this is used as a local offset from the Spawn Point position.")]
		public FsmVector3 position;

		[Tooltip("Rotation. NOTE: Overrides the rotation of the Spawn Point.")]
		public FsmVector3 rotation;

		[UIHint(UIHint.Variable)]
		[Tooltip("Optionally store the created object.")]
		public FsmGameObject storeObject;

		private GameObject preSpawnedObject;

		public override void Awake()
		{
			base.Awake();
			GameObject value = gameObject.Value;
			if (value != null)
			{
				preSpawnedObject = Object.Instantiate(value);
				preSpawnedObject.SetActive(value: false);
			}
		}

		public override void Reset()
		{
			gameObject = null;
			canPreSpawn = true;
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
			GameObject value = this.gameObject.Value;
			if (value != null)
			{
				Vector3 vector = Vector3.zero;
				Vector3 euler = Vector3.zero;
				if (spawnPoint.Value != null)
				{
					vector = spawnPoint.Value.transform.position;
					if (!position.IsNone)
					{
						vector += position.Value;
					}
					euler = ((!rotation.IsNone) ? rotation.Value : spawnPoint.Value.transform.eulerAngles);
				}
				else
				{
					if (!position.IsNone)
					{
						vector = position.Value;
					}
					if (!rotation.IsNone)
					{
						euler = rotation.Value;
					}
				}
				GameObject gameObject = null;
				if ((bool)preSpawnedObject)
				{
					gameObject = preSpawnedObject;
					gameObject.transform.position = vector;
					gameObject.transform.rotation = Quaternion.Euler(euler);
					gameObject.gameObject.SetActive(value: true);
					preSpawnedObject = null;
				}
				else
				{
					gameObject = Object.Instantiate(value, vector, Quaternion.Euler(euler));
				}
				storeObject.Value = gameObject;
			}
			Finish();
		}
	}
}
