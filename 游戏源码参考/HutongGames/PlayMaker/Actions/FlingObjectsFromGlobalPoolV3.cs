using System;
using System.Collections.Generic;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.GameObject)]
	public sealed class FlingObjectsFromGlobalPoolV3 : RigidBody2dActionBase
	{
		[RequiredField]
		[ArrayEditor(VariableType.GameObject, "", 0, 0, 65536)]
		public FsmArray GameObjects;

		public FsmGameObject SpawnPoint;

		public FsmVector3 Position;

		public FsmInt SpawnMin;

		public FsmInt SpawnMax;

		public FsmFloat SpeedMin;

		public FsmFloat SpeedMax;

		public FsmFloat AngleMin;

		public FsmFloat AngleMax;

		public FsmFloat OriginVariationX;

		public FsmFloat OriginVariationY;

		public FsmInt spreadFrames;

		private float vectorX;

		private float vectorY;

		private bool originAdjusted;

		private List<GameObject> gameObjects = new List<GameObject>();

		private int spawns;

		private int spawnRate;

		private Vector3 spawnPosition;

		private Vector3 spawnRotation;

		public override void Awake()
		{
			base.Awake();
			if (!Application.isPlaying || GameObjects == null)
			{
				return;
			}
			GameObject gameObject = base.Owner;
			int value = SpawnMax.Value;
			bool flag = false;
			object[] values = GameObjects.Values;
			for (int i = 0; i < values.Length; i++)
			{
				GameObject gameObject2 = (GameObject)values[i];
				if (!(gameObject2 == null))
				{
					flag = true;
					PersonalObjectPool.EnsurePooledInScene(gameObject, gameObject2, value, finished: false);
				}
			}
			if (flag)
			{
				PersonalObjectPool.CreateIfRequired(gameObject);
			}
		}

		public override void Reset()
		{
			GameObjects = null;
			SpawnPoint = null;
			Position = new FsmVector3
			{
				UseVariable = true
			};
			SpawnMin = null;
			SpawnMax = null;
			SpeedMin = null;
			SpeedMax = null;
			AngleMin = null;
			AngleMax = null;
			OriginVariationX = null;
			OriginVariationY = null;
			spreadFrames = null;
		}

		public override void OnEnter()
		{
			spawnPosition = Vector3.zero;
			spawnRotation = Vector3.zero;
			if (SpawnPoint.Value != null)
			{
				spawnPosition = SpawnPoint.Value.transform.position;
				if (!Position.IsNone)
				{
					spawnPosition += Position.Value;
				}
			}
			else if (!Position.IsNone)
			{
				spawnPosition = Position.Value;
			}
			gameObjects.Clear();
			object[] values = GameObjects.Values;
			for (int i = 0; i < values.Length; i++)
			{
				GameObject gameObject = (GameObject)values[i];
				if (!(gameObject == null))
				{
					gameObjects.Add(gameObject);
				}
			}
			spawns = UnityEngine.Random.Range(SpawnMin.Value, SpawnMax.Value + 1);
			if (spawns > 0)
			{
				int value = spreadFrames.Value;
				if (value <= 1)
				{
					DoSpawn(spawns);
					spawns = 0;
				}
				else
				{
					spawnRate = Mathf.CeilToInt((float)spawns / (float)value);
				}
			}
			if (spawns <= 0)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			DoStaggeredSpawn();
			if (spawns <= 0)
			{
				Finish();
			}
		}

		private void DoSpawn(int spawns)
		{
			if (spawns <= 0)
			{
				return;
			}
			this.spawns -= spawns;
			float value = SpeedMin.Value;
			float value2 = SpeedMax.Value;
			float value3 = AngleMin.Value;
			float value4 = AngleMax.Value;
			bool flag = OriginVariationX != null;
			float num = (flag ? OriginVariationX.Value : 0f);
			bool flag2 = OriginVariationY != null;
			float num2 = (flag2 ? OriginVariationY.Value : 0f);
			Vector2 linearVelocity = default(Vector2);
			for (int i = 1; i <= spawns; i++)
			{
				int index = ((gameObjects.Count > 1) ? UnityEngine.Random.Range(0, gameObjects.Count) : 0);
				GameObject prefab = gameObjects[index];
				float x = 0f;
				float y = 0f;
				if (flag)
				{
					x = UnityEngine.Random.Range(0f - num, num);
					originAdjusted = true;
				}
				if (flag2)
				{
					y = UnityEngine.Random.Range(0f - num2, num2);
					originAdjusted = true;
				}
				Vector3 position = spawnPosition;
				if (originAdjusted)
				{
					position += new Vector3(x, y, 0f);
				}
				GameObject gameObject = prefab.Spawn(position, Quaternion.Euler(spawnRotation));
				BlackThreadState.HandleDamagerSpawn(base.Owner, gameObject);
				rb2d = gameObject.GetComponent<Rigidbody2D>();
				float num3 = UnityEngine.Random.Range(value, value2);
				float num4 = UnityEngine.Random.Range(value3, value4);
				vectorX = num3 * Mathf.Cos(num4 * (MathF.PI / 180f));
				vectorY = num3 * Mathf.Sin(num4 * (MathF.PI / 180f));
				linearVelocity.x = vectorX;
				linearVelocity.y = vectorY;
				rb2d.linearVelocity = linearVelocity;
			}
		}

		private void DoStaggeredSpawn()
		{
			if (spawnRate > 0)
			{
				DoSpawn(Mathf.Min(spawnRate, spawns));
			}
		}
	}
}
