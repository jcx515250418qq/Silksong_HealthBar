using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.GameObject)]
	public class SpawnRandomObjectsRadialV2 : FsmStateAction
	{
		public FsmGameObject Prefab;

		public FsmOwnerDefault SpawnPoint;

		public FsmBool SpawnAsChildren;

		public FsmInt MinCount;

		public FsmInt MaxCount;

		public FsmFloat MinAngle;

		public FsmFloat MaxAngle;

		public FsmFloat AngleErrorMax;

		public FsmFloat SpawnRadius;

		public override void Awake()
		{
			GameObject value = Prefab.Value;
			if (value != null)
			{
				PersonalObjectPool.EnsurePooledInScene(base.Fsm.Owner.gameObject, value, MaxCount.Value);
			}
		}

		public override void Reset()
		{
			Prefab = null;
			SpawnPoint = null;
			SpawnAsChildren = null;
			MinCount = null;
			MaxCount = null;
			MinAngle = null;
			MaxAngle = null;
			AngleErrorMax = null;
			SpawnRadius = null;
		}

		public override void OnEnter()
		{
			GameObject safe = SpawnPoint.GetSafe(this);
			GameObject value = Prefab.Value;
			if ((bool)safe && (bool)value)
			{
				Vector3 position = safe.transform.position;
				int num = UnityEngine.Random.Range(MinCount.Value, MaxCount.Value + 1);
				for (int i = 0; i < num; i++)
				{
					GameObject gameObject = value.Spawn(SpawnAsChildren.Value ? safe.transform : null);
					float num2 = Mathf.Lerp(MinAngle.Value, MaxAngle.Value, (float)i / (float)num);
					num2 += UnityEngine.Random.Range(0f - AngleErrorMax.Value, AngleErrorMax.Value);
					gameObject.transform.SetRotation2D(num2);
					float x = Mathf.Cos(num2 * (MathF.PI / 180f));
					float y = Mathf.Sin(num2 * (MathF.PI / 180f));
					gameObject.transform.position = position + new Vector3(x, y, 0f) * SpawnRadius.Value;
				}
			}
			Finish();
		}
	}
}
