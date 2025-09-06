using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.GameObject)]
	[Tooltip("Spawns a prefab Game Object from the Global Object Pool on the Game Manager.")]
	public class SpawnObjectFromGlobalPoolOverTime : FsmStateAction
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

		[Tooltip("How often, in seconds, spawn occurs.")]
		public FsmFloat frequency;

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
			frequency = null;
		}

		public override void OnEnter()
		{
			timer = 0f;
			base.OnEnter();
		}

		public override void OnUpdate()
		{
			timer += Time.deltaTime;
			if (!(timer >= frequency.Value))
			{
				return;
			}
			timer -= frequency.Value;
			if (!(gameObject.Value != null))
			{
				return;
			}
			Vector3 vector = Vector3.zero;
			Vector3 euler = Vector3.up;
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
			if (gameObject != null)
			{
				GameObject spawned = gameObject.Value.Spawn(vector, Quaternion.Euler(euler));
				BlackThreadState.HandleDamagerSpawn(base.Owner, spawned);
			}
		}
	}
}
