using System;
using System.Collections.Generic;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.GameObject)]
	public class FlingObjectsFromGlobalPoolV2 : RigidBody2dActionBase
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

		private float vectorX;

		private float vectorY;

		private bool originAdjusted;

		private List<GameObject> gameObjects = new List<GameObject>();

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
		}

		public override void OnEnter()
		{
			Vector3 vector = Vector3.zero;
			Vector3 zero = Vector3.zero;
			if (SpawnPoint.Value != null)
			{
				vector = SpawnPoint.Value.transform.position;
				if (!Position.IsNone)
				{
					vector += Position.Value;
				}
			}
			else if (!Position.IsNone)
			{
				vector = Position.Value;
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
			int num = UnityEngine.Random.Range(SpawnMin.Value, SpawnMax.Value + 1);
			if (num > 0)
			{
				float value = SpeedMin.Value;
				float value2 = SpeedMax.Value;
				float value3 = AngleMin.Value;
				float value4 = AngleMax.Value;
				bool flag = OriginVariationX != null;
				float num2 = (flag ? OriginVariationX.Value : 0f);
				bool flag2 = OriginVariationY != null;
				float num3 = (flag2 ? OriginVariationY.Value : 0f);
				Vector2 linearVelocity = default(Vector2);
				for (int j = 1; j <= num; j++)
				{
					int index = ((gameObjects.Count > 1) ? UnityEngine.Random.Range(0, gameObjects.Count) : 0);
					GameObject prefab = gameObjects[index];
					float x = 0f;
					float y = 0f;
					if (flag)
					{
						x = UnityEngine.Random.Range(0f - num2, num2);
						originAdjusted = true;
					}
					if (flag2)
					{
						y = UnityEngine.Random.Range(0f - num3, num3);
						originAdjusted = true;
					}
					Vector3 position = vector;
					if (originAdjusted)
					{
						position += new Vector3(x, y, 0f);
					}
					GameObject gameObject2 = prefab.Spawn(position, Quaternion.Euler(zero));
					BlackThreadState.HandleDamagerSpawn(base.Owner, gameObject2);
					rb2d = gameObject2.GetComponent<Rigidbody2D>();
					float num4 = UnityEngine.Random.Range(value, value2);
					float num5 = UnityEngine.Random.Range(value3, value4);
					vectorX = num4 * Mathf.Cos(num5 * (MathF.PI / 180f));
					vectorY = num4 * Mathf.Sin(num5 * (MathF.PI / 180f));
					linearVelocity.x = vectorX;
					linearVelocity.y = vectorY;
					rb2d.linearVelocity = linearVelocity;
				}
			}
			Finish();
		}
	}
}
