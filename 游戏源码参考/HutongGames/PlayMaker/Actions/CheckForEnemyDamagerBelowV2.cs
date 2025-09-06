using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	public class CheckForEnemyDamagerBelowV2 : FSMUtility.CheckFsmStateEveryFrameAction
	{
		[CheckForComponent(typeof(BoxCollider2D))]
		public FsmOwnerDefault Target;

		public FsmFloat MaxDistance;

		private GameObject obj;

		private BoxCollider2D collider;

		private int layerMask;

		private readonly RaycastHit2D[] results = new RaycastHit2D[20];

		private float maxDistance;

		public override bool IsTrue
		{
			get
			{
				if (!collider)
				{
					return false;
				}
				Vector2 origin = obj.transform.TransformPoint(collider.offset);
				Vector2 size = collider.size.MultiplyElements((Vector2)obj.transform.lossyScale).Abs();
				int a = Physics2D.BoxCastNonAlloc(origin, size, obj.transform.eulerAngles.z, Vector2.down, results, maxDistance, layerMask);
				float num = float.MaxValue;
				float num2 = float.MaxValue;
				bool flag = false;
				for (int i = 0; i < Mathf.Min(a, results.Length); i++)
				{
					RaycastHit2D raycastHit2D = results[i];
					GameObject gameObject = raycastHit2D.collider.gameObject;
					float num3 = origin.y - raycastHit2D.point.y;
					if (num3 < 0f)
					{
						continue;
					}
					if (gameObject.layer == 17 || (bool)gameObject.GetComponent<DamageEnemies>())
					{
						if (num3 < num2)
						{
							num2 = num3;
						}
						flag = true;
					}
					else if (gameObject.layer == 8)
					{
						if (num3 < num)
						{
							num = num3;
						}
						flag = true;
					}
				}
				if (!flag)
				{
					return false;
				}
				return num2 < num;
			}
		}

		public override void Reset()
		{
			base.Reset();
			Target = null;
			MaxDistance = null;
		}

		public override void OnEnter()
		{
			obj = Target.GetSafe(this);
			if ((bool)obj)
			{
				collider = obj.GetComponent<BoxCollider2D>();
				layerMask = Helper.GetCollidingLayerMaskForLayer(obj.layer);
				maxDistance = MaxDistance.Value;
				if (maxDistance <= 0f)
				{
					maxDistance = 100f;
				}
			}
			base.OnEnter();
		}
	}
}
