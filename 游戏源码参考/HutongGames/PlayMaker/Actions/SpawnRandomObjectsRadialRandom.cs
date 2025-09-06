using System;
using System.Collections.Generic;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.GameObject)]
	public class SpawnRandomObjectsRadialRandom : FsmStateAction
	{
		[RequiredField]
		public FsmGameObject Prefab;

		[RequiredField]
		public FsmOwnerDefault SpawnPoint;

		public FsmBool SetRotation;

		public FsmBool SpawnAsChildren;

		public FsmInt MinCount;

		public FsmInt MaxCount;

		public FsmFloat MinAngle;

		public FsmFloat MaxAngle;

		public FsmFloat MinSpawnRadius;

		public FsmFloat MaxSpawnRadius;

		public FsmFloat MinSpacing;

		public FsmBool NeedsLineOfSight;

		private Action<GameObject> recycleResetDelegate;

		private readonly List<GameObject> spawnedObjs = new List<GameObject>();

		private readonly List<Vector2> tempPosStore = new List<Vector2>();

		public override void Reset()
		{
			Prefab = null;
			SpawnPoint = null;
			SetRotation = null;
			SpawnAsChildren = null;
			MinCount = null;
			MaxCount = null;
			MinAngle = null;
			MaxAngle = null;
			MinSpawnRadius = null;
			MaxSpawnRadius = null;
			MinSpacing = null;
			NeedsLineOfSight = null;
		}

		public override void Awake()
		{
			recycleResetDelegate = OnRecycleReset;
		}

		public override void OnEnter()
		{
			GameObject safe = SpawnPoint.GetSafe(this);
			GameObject value = Prefab.Value;
			if ((bool)safe && (bool)value)
			{
				tempPosStore.Clear();
				foreach (GameObject spawnedObj in spawnedObjs)
				{
					tempPosStore.Add(spawnedObj.transform.position);
				}
				Vector3 position = safe.transform.position;
				bool flag = false;
				int num = UnityEngine.Random.Range(MinCount.Value, MaxCount.Value + 1);
				for (int i = 0; i < num; i++)
				{
					int num2 = 100;
					float num3;
					Vector3 vector;
					do
					{
						IL_00c0:
						num3 = UnityEngine.Random.Range(MinAngle.Value, MaxAngle.Value);
						float x = Mathf.Cos(num3 * (MathF.PI / 180f));
						float y = Mathf.Sin(num3 * (MathF.PI / 180f));
						float num4 = UnityEngine.Random.Range(MinSpawnRadius.Value, MaxSpawnRadius.Value);
						vector = position + new Vector3(x, y, 0f) * num4;
						foreach (Vector2 item in tempPosStore)
						{
							if (!(Vector2.Distance(vector, item) > MinSpacing.Value))
							{
								num2--;
								if (num2 <= 0)
								{
									break;
								}
								goto IL_00c0;
							}
						}
						Vector3 vector2 = vector - position;
						if (!NeedsLineOfSight.Value || num2 <= 0 || !Helper.IsRayHittingNoTriggers(position, vector2.normalized, vector2.magnitude, 256))
						{
							break;
						}
						num2--;
					}
					while (num2 > 0);
					if (num2 <= 0)
					{
						if (flag)
						{
							break;
						}
						vector = position;
					}
					flag = true;
					GameObject gameObject = value.Spawn(SpawnAsChildren.Value ? safe.transform : null);
					gameObject.transform.position = vector;
					if (SetRotation.Value)
					{
						gameObject.transform.SetRotation2D(num3);
					}
					tempPosStore.Add(vector);
					if (spawnedObjs.AddIfNotPresent(gameObject))
					{
						RecycleResetHandler.Add(gameObject, recycleResetDelegate);
					}
				}
			}
			Finish();
		}

		private void OnRecycleReset(GameObject obj)
		{
			spawnedObjs.Remove(obj);
		}
	}
}
