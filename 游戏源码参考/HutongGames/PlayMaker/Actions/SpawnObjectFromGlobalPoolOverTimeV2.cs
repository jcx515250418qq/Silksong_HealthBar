using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.GameObject)]
	[Tooltip("Spawns a prefab Game Object from the Global Object Pool on the Game Manager.")]
	public class SpawnObjectFromGlobalPoolOverTimeV2 : FsmStateAction
	{
		[RequiredField]
		[Tooltip("GameObject to create. Usually a Prefab.")]
		public FsmGameObject gameObject;

		[Tooltip("Optional Spawn Point.")]
		public FsmGameObject spawnPoint;

		[Tooltip("Position. If a Spawn Point is defined, this is used as a local offset from the Spawn Point position.")]
		public FsmVector3 position;

		[Tooltip("Randomises spawn points of objects within this range. Leave as 0 and all objects will spawn at same point.")]
		public FsmFloat originVariationX;

		public FsmFloat originVariationY;

		[Tooltip("Rotation. NOTE: Overrides the rotation of the Spawn Point.")]
		public FsmVector3 rotation;

		[Tooltip("How often, in seconds, spawn occurs.")]
		public FsmFloat frequency;

		[Tooltip("Minimum scale of clone.")]
		public FsmFloat scaleMin = 1f;

		[Tooltip("Maximum scale of clone.")]
		public FsmFloat scaleMax = 1f;

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
			originVariationX = null;
			originVariationY = null;
		}

		public override void OnUpdate()
		{
			timer += Time.deltaTime;
			if (!(timer >= frequency.Value))
			{
				return;
			}
			timer = 0f;
			if (!(this.gameObject.Value != null))
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
			if (this.gameObject == null)
			{
				return;
			}
			GameObject gameObject = this.gameObject.Value.Spawn(vector, Quaternion.Euler(euler));
			if (originVariationX != null)
			{
				gameObject.transform.Translate(UnityEngine.Random.Range(0f - originVariationX.Value, originVariationX.Value), 0f, 0f, Space.World);
			}
			if (originVariationY != null)
			{
				gameObject.transform.Translate(0f, UnityEngine.Random.Range(0f - originVariationY.Value, originVariationY.Value), 0f, Space.World);
			}
			if (scaleMin != null && scaleMax != null)
			{
				float num = UnityEngine.Random.Range(scaleMin.Value, scaleMax.Value);
				if (Math.Abs(num - 1f) > 0.001f)
				{
					gameObject.transform.localScale = new Vector3(num, num, num);
				}
			}
			BlackThreadState.HandleDamagerSpawn(base.Owner, gameObject);
		}
	}
}
