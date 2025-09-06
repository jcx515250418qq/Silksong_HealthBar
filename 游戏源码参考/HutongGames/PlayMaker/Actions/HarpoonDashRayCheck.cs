using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	public class HarpoonDashRayCheck : FsmStateAction
	{
		private enum HitTypes
		{
			None = 0,
			Terrain = 1,
			Tinker = 2,
			HarpoonRing = 3,
			BouncePod = 4,
			Enemy = 5
		}

		private struct HitCheck
		{
			public HitTypes HitType;

			public RaycastHit2D Hit;
		}

		public FsmOwnerDefault Hero;

		public FsmFloat Direction;

		[UIHint(UIHint.Variable)]
		public FsmGameObject StoreHitObject;

		[UIHint(UIHint.Variable)]
		public FsmVector2 StoreHitPoint;

		public FsmEvent NoHitEvent;

		public FsmEvent TerrainEvent;

		public FsmEvent EnemyEvent;

		public FsmEvent RingEvent;

		public FsmEvent BouncePodEvent;

		public FsmEvent TinkEvent;

		private readonly RaycastHit2D[] results = new RaycastHit2D[10];

		private readonly HitCheck[] hitChecks = new HitCheck[6];

		public override void Reset()
		{
			Hero = null;
			Direction = null;
			StoreHitObject = null;
			StoreHitPoint = null;
			NoHitEvent = null;
			TerrainEvent = null;
			EnemyEvent = null;
			RingEvent = null;
			BouncePodEvent = null;
			TinkEvent = null;
		}

		public override void OnEnter()
		{
			StoreHitObject.Value = null;
			StoreHitPoint.Value = Vector2.zero;
			GameObject safe = Hero.GetSafe(this);
			if (!safe)
			{
				Finish();
				return;
			}
			Vector2 vector = safe.transform.position;
			hitChecks[0] = CheckRay(vector + new Vector2(0f, 0.425f), isTerrainCheck: false);
			hitChecks[1] = CheckRay(vector + new Vector2(0f, -1.05f), isTerrainCheck: false);
			hitChecks[2] = CheckRay(vector + new Vector2(0f, -0.31249997f), isTerrainCheck: false);
			hitChecks[3] = CheckRay(vector + new Vector2(0f, 0.05625002f), isTerrainCheck: true);
			hitChecks[4] = CheckRay(vector + new Vector2(0f, -109f / 160f), isTerrainCheck: true);
			hitChecks[5] = CheckRay(vector + new Vector2(0f, -0.31249997f), isTerrainCheck: true);
			HitTypes hitTypes = HitTypes.None;
			RaycastHit2D hit = default(RaycastHit2D);
			HitCheck[] array = hitChecks;
			for (int i = 0; i < array.Length; i++)
			{
				HitCheck hitCheck = array[i];
				if (hitCheck.HitType >= hitTypes)
				{
					hitTypes = hitCheck.HitType;
					hit = hitCheck.Hit;
				}
			}
			float num = float.PositiveInfinity;
			RaycastHit2D raycastHit2D = default(RaycastHit2D);
			array = hitChecks;
			for (int i = 0; i < array.Length; i++)
			{
				HitCheck hitCheck2 = array[i];
				if (hitCheck2.HitType == HitTypes.Terrain)
				{
					RaycastHit2D hit2 = hitCheck2.Hit;
					if (!(hit2.distance > num))
					{
						hit2 = hitCheck2.Hit;
						num = hit2.distance;
						raycastHit2D = hitCheck2.Hit;
					}
				}
			}
			if (num < hit.distance)
			{
				hitTypes = HitTypes.Terrain;
				hit = raycastHit2D;
			}
			switch (hitTypes)
			{
			case HitTypes.None:
				base.Fsm.Event(NoHitEvent);
				Finish();
				break;
			case HitTypes.Enemy:
				RecordSuccess(hit);
				base.Fsm.Event(EnemyEvent);
				Finish();
				break;
			case HitTypes.Terrain:
				RecordSuccess(hit);
				base.Fsm.Event(TerrainEvent);
				Finish();
				break;
			case HitTypes.HarpoonRing:
				RecordSuccess(hit);
				base.Fsm.Event(RingEvent);
				Finish();
				break;
			case HitTypes.BouncePod:
				RecordSuccess(hit);
				base.Fsm.Event(BouncePodEvent);
				Finish();
				break;
			case HitTypes.Tinker:
				RecordSuccess(hit);
				base.Fsm.Event(TinkEvent);
				Finish();
				break;
			default:
				throw new ArgumentOutOfRangeException();
			}
		}

		private HitCheck CheckRay(Vector2 origin, bool isTerrainCheck)
		{
			Vector2 direction = new Vector2(Mathf.Sign(Direction.Value), 0f);
			float distance = 10.5f;
			int a = Physics2D.Raycast(origin, direction, new ContactFilter2D
			{
				useLayerMask = true,
				layerMask = (isTerrainCheck ? 256 : 657408),
				useTriggers = true
			}, results, distance);
			HitCheck result;
			for (int i = 0; i < Mathf.Min(a, results.Length); i++)
			{
				RaycastHit2D hit = results[i];
				Collider2D collider = hit.collider;
				if (collider.gameObject.layer == 11)
				{
					if (!HitTaker.TryGetHealthManager(collider.gameObject, out var healthManager) || !healthManager.IsInvincible || !healthManager.PreventInvincibleEffect || healthManager.InvincibleFromDirection == 2 || healthManager.InvincibleFromDirection == 4 || healthManager.InvincibleFromDirection == 7)
					{
						result = default(HitCheck);
						result.Hit = hit;
						result.HitType = HitTypes.Enemy;
						return result;
					}
					continue;
				}
				if (collider.CompareTag("Bounce Pod") || (bool)collider.GetComponent<BouncePod>() || (bool)collider.GetComponent<HarpoonHook>())
				{
					result = default(HitCheck);
					result.Hit = hit;
					result.HitType = HitTypes.BouncePod;
					return result;
				}
				if (collider.CompareTag("Harpoon Ring"))
				{
					result = default(HitCheck);
					result.Hit = hit;
					result.HitType = HitTypes.HarpoonRing;
					return result;
				}
				if (collider.gameObject.layer == 17 && (bool)collider.GetComponent<TinkEffect>() && !collider.GetComponent<TinkEffect>().noHarpoonHook)
				{
					result = default(HitCheck);
					result.Hit = hit;
					result.HitType = HitTypes.Tinker;
					return result;
				}
				if (collider.gameObject.layer == 8)
				{
					result = default(HitCheck);
					result.Hit = hit;
					result.HitType = HitTypes.Terrain;
					return result;
				}
			}
			result = default(HitCheck);
			result.Hit = default(RaycastHit2D);
			result.HitType = HitTypes.None;
			return result;
		}

		private void RecordSuccess(RaycastHit2D hit)
		{
			StoreHitObject.Value = hit.collider.gameObject;
			StoreHitPoint.Value = hit.point;
		}
	}
}
