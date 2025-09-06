using GlobalEnums;
using UnityEngine;

public class NailSlashRecoil : MonoBehaviour
{
	[SerializeField]
	private DamageEnemies enemyDamager;

	[SerializeField]
	private bool drillPull;

	private NailAttackBase nailSlash;

	private HeroController heroCtrl;

	private void Reset()
	{
		enemyDamager = GetComponent<DamageEnemies>();
	}

	private void Awake()
	{
		heroCtrl = GetComponentInParent<HeroController>();
		nailSlash = GetComponent<NailAttackBase>();
	}

	private void Start()
	{
		enemyDamager.HitResponded += HitResponse;
	}

	public static void Add(GameObject gameObject, DamageEnemies enemyDamager, bool drillPull)
	{
		if (!gameObject.GetComponent<NailSlashRecoil>())
		{
			NailSlashRecoil nailSlashRecoil = gameObject.AddComponent<NailSlashRecoil>();
			nailSlashRecoil.enemyDamager = enemyDamager;
			nailSlashRecoil.drillPull = drillPull;
		}
	}

	private void HitResponse(DamageEnemies.HitResponse response)
	{
		if (!heroCtrl)
		{
			return;
		}
		MonoBehaviour monoBehaviour = response.Responder as MonoBehaviour;
		if (monoBehaviour == null)
		{
			return;
		}
		float num = GetActualHitDirection(response.Target.transform, enemyDamager);
		if (num >= 360f)
		{
			num -= 360f;
		}
		int cardinalDirection = DirectionUtils.GetCardinalDirection(response.Hit.Direction);
		int cardinalDirection2 = DirectionUtils.GetCardinalDirection(enemyDamager.GetDirection());
		if (cardinalDirection != cardinalDirection2)
		{
			return;
		}
		if (!nailSlash)
		{
			heroCtrl.SetAllowRecoilWhileRelinquished(value: true);
		}
		GameObject gameObject = monoBehaviour.gameObject;
		int cardinalDirection3 = DirectionUtils.GetCardinalDirection(num);
		NonBouncer component = gameObject.GetComponent<NonBouncer>();
		switch (cardinalDirection3)
		{
		case 0:
			switch (response.LayerOnHit)
			{
			case PhysLayers.ENEMIES:
				if (component == null || !component.active)
				{
					if (gameObject.GetComponent<BounceShroom>() != null)
					{
						heroCtrl.RecoilLeftLong();
						Bounce(gameObject, useEffects: false);
					}
					else if (drillPull)
					{
						heroCtrl.DrillPull(isRight: false);
					}
					else
					{
						heroCtrl.RecoilLeft();
					}
				}
				break;
			case PhysLayers.TERRAIN:
			case PhysLayers.INTERACTIVE_OBJECT:
				if (gameObject.CompareTag("Recoiler"))
				{
					heroCtrl.RecoilLeft();
				}
				break;
			}
			break;
		case 2:
			switch (response.LayerOnHit)
			{
			case PhysLayers.ENEMIES:
				if (component == null || !component.active)
				{
					if (gameObject.GetComponent<BounceShroom>() != null)
					{
						heroCtrl.RecoilRightLong();
						Bounce(gameObject, useEffects: false);
					}
					else if (drillPull)
					{
						heroCtrl.DrillPull(isRight: true);
					}
					else
					{
						heroCtrl.RecoilRight();
					}
				}
				break;
			case PhysLayers.TERRAIN:
			case PhysLayers.INTERACTIVE_OBJECT:
				if (gameObject.CompareTag("Recoiler"))
				{
					heroCtrl.RecoilRight();
				}
				break;
			}
			break;
		case 1:
			switch (response.LayerOnHit)
			{
			case PhysLayers.ENEMIES:
				if (component == null || !component.active)
				{
					if (gameObject.GetComponent<BounceShroom>() != null)
					{
						heroCtrl.RecoilDown();
						Bounce(gameObject, useEffects: false);
					}
					else
					{
						heroCtrl.RecoilDown();
					}
				}
				break;
			case PhysLayers.TERRAIN:
			case PhysLayers.INTERACTIVE_OBJECT:
				if (gameObject.CompareTag("Recoiler"))
				{
					heroCtrl.RecoilDown();
				}
				break;
			}
			break;
		case 3:
			break;
		}
	}

	private static void Bounce(GameObject obj, bool useEffects)
	{
		PlayMakerFSM playMakerFSM = FSMUtility.LocateFSM(obj, "Bounce Shroom");
		if ((bool)playMakerFSM)
		{
			playMakerFSM.SendEvent("BOUNCE UPWARD");
			return;
		}
		BounceShroom component = obj.GetComponent<BounceShroom>();
		if ((bool)component)
		{
			component.BounceLarge(useEffects);
		}
	}

	private float GetActualHitDirection(Transform target, DamageEnemies damager)
	{
		if (!damager)
		{
			return 0f;
		}
		if (!damager.CircleDirection)
		{
			return damager.GetDirection();
		}
		Vector2 vector = (Vector2)target.position - (Vector2)damager.transform.position;
		return Mathf.Atan2(vector.y, vector.x) * 57.29578f;
	}
}
