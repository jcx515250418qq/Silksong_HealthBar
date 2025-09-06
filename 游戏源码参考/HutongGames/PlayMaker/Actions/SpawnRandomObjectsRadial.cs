using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.GameObject)]
	public class SpawnRandomObjectsRadial : FsmStateAction
	{
		public FsmGameObject Prefab;

		public FsmOwnerDefault SpawnPoint;

		public FsmInt MinCount;

		public FsmInt MaxCount;

		public FsmFloat MinAngle;

		public FsmFloat MaxAngle;

		public FsmFloat AngleErrorMax;

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
			MinCount = null;
			MaxCount = null;
			MinAngle = null;
			MaxAngle = null;
			AngleErrorMax = null;
		}

		public override void OnEnter()
		{
			GameObject safe = SpawnPoint.GetSafe(this);
			GameObject value = Prefab.Value;
			if ((bool)safe && (bool)value)
			{
				Vector3 position = safe.transform.position;
				int num = Random.Range(MinCount.Value, MaxCount.Value + 1);
				for (int i = 0; i < num; i++)
				{
					GameObject gameObject = value.Spawn();
					gameObject.transform.position = position;
					float num2 = Mathf.Lerp(MinAngle.Value, MaxAngle.Value, (float)i / (float)num);
					global::Extensions.SetRotation2D(rotation: num2 + Random.Range(0f - AngleErrorMax.Value, AngleErrorMax.Value), t: gameObject.transform);
				}
			}
			Finish();
		}
	}
}
