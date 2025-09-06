using GlobalEnums;
using TeamCherry.SharedUtils;
using UnityEngine;
using UnityEngine.Events;

public class BreakOnHazard : MonoBehaviour
{
	[SerializeField]
	[EnumPickerBitmask(typeof(HazardType))]
	private int hazardMask;

	[Space]
	[SerializeField]
	private GameObject breakEffectPrefab;

	[SerializeField]
	private RandomAudioClipTable breakAudioClipTable;

	[SerializeField]
	private bool doNotRecycle;

	[SerializeField]
	private bool migrateUpOnStart;

	[Space]
	public UnityEvent OnBreak;

	private void Start()
	{
		if (migrateUpOnStart)
		{
			GameObject gameObject = base.transform.parent.gameObject;
			if (!gameObject.GetComponent<BreakOnHazard>())
			{
				BreakOnHazard breakOnHazard = gameObject.AddComponent<BreakOnHazard>();
				breakOnHazard.hazardMask = hazardMask;
				breakOnHazard.breakEffectPrefab = breakEffectPrefab;
				breakOnHazard.breakAudioClipTable = breakAudioClipTable;
				breakOnHazard.OnBreak = OnBreak;
			}
			Object.Destroy(this);
		}
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (!collision.CompareTag("Geo"))
		{
			if (IsCogDamager(collision.gameObject))
			{
				Break();
			}
			else
			{
				HandleCollision(collision);
			}
		}
	}

	private void OnCollisionEnter2D(Collision2D collision)
	{
		HandleCollision(collision.collider);
	}

	private void HandleCollision(Collider2D col)
	{
		if (!col.gameObject.CompareTag("Geo") && DamageHero.TryGet(col.gameObject, out var damageHero))
		{
			HazardType hazardType = damageHero.hazardType;
			if (hazardMask.IsBitSet((int)hazardType))
			{
				Break();
			}
		}
	}

	public void Break()
	{
		if ((bool)breakEffectPrefab)
		{
			breakEffectPrefab.Spawn(base.transform.position);
		}
		if ((bool)breakAudioClipTable)
		{
			breakAudioClipTable.SpawnAndPlayOneShot(base.transform.position);
		}
		if (!doNotRecycle)
		{
			base.gameObject.Recycle();
		}
		OnBreak.Invoke();
	}

	public static bool IsCogDamager(GameObject target)
	{
		DamageEnemies component = target.GetComponent<DamageEnemies>();
		if (!component)
		{
			return false;
		}
		if (component.multiHitter && component.attackType == AttackTypes.Generic)
		{
			if (!(component.contactFSMEvent == "COG ENTER"))
			{
				return target.CompareTag("Breaker");
			}
			return true;
		}
		return false;
	}
}
