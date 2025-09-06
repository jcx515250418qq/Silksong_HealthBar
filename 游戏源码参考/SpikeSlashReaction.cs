using GlobalSettings;
using UnityEngine;

public class SpikeSlashReaction : MonoBehaviour, IHitResponder
{
	[SerializeField]
	[AssetPickerDropdown]
	private SpikeSlashReactionProfile profile;

	[Space]
	[SerializeField]
	private bool useNailPosition = true;

	private Collider2D collider;

	private bool hasTinkEffect;

	private void Awake()
	{
		TinkEffect component = GetComponent<TinkEffect>();
		hasTinkEffect = component != null;
		if (hasTinkEffect)
		{
			component.OnSpawnedTink += OnSpawnedTink;
		}
		collider = GetComponent<Collider2D>();
	}

	private void OnEnable()
	{
	}

	private void OnSpawnedTink(Vector3 position, Quaternion rotation)
	{
		if (base.enabled)
		{
			SpawnSlashReaction(position, rotation);
		}
	}

	public void SpawnSlashReaction(Vector3 position, Quaternion rotation)
	{
		Effects.SpikeSlashEffectPrefab.Spawn(position, rotation);
		if (profile != null)
		{
			profile.SpawnEffect(position, rotation);
		}
	}

	public IHitResponder.HitResponse Hit(HitInstance damageInstance)
	{
		if (hasTinkEffect)
		{
			return IHitResponder.Response.None;
		}
		if (!base.enabled)
		{
			return IHitResponder.Response.None;
		}
		Vector3 euler = new Vector3(0f, 0f, 0f);
		bool flag = collider != null;
		bool flag2 = flag;
		if (useNailPosition)
		{
			flag2 = false;
		}
		Vector2 vector = Vector2.zero;
		float num = 0f;
		float num2 = 0f;
		if (flag2)
		{
			Bounds bounds = collider.bounds;
			vector = base.transform.TransformPoint(collider.offset);
			num = bounds.size.x * 0.5f;
			num2 = bounds.size.y * 0.5f;
		}
		GameObject source = damageInstance.Source;
		bool flag3 = source.CompareTag("Nail Attack");
		HeroController instance = HeroController.instance;
		Vector3 position = source.transform.position;
		Vector3 vector2 = (flag3 ? instance.transform.position : position);
		Vector3 vector3;
		switch (DirectionUtils.GetCardinalDirection(GetActualHitDirection(damageInstance.Source.GetComponent<DamageEnemies>())))
		{
		case 0:
			vector3 = ((!flag2) ? ((!flag3) ? new Vector3(vector2.x, vector2.y, 0.002f) : new Vector3(vector2.x + 2f, vector2.y, 0.002f)) : new Vector3(vector.x - num, position.y, 0.002f));
			break;
		case 1:
			vector3 = (flag2 ? new Vector3(position.x, Mathf.Max(vector.y - num2, position.y), 0.002f) : ((!flag3) ? new Vector3(vector2.x, vector2.y, 0.002f) : new Vector3(vector2.x, vector2.y + 2f, 0.002f)));
			euler = new Vector3(0f, 0f, 90f);
			break;
		case 2:
			vector3 = (flag2 ? new Vector3(vector.x + num, position.y, 0.002f) : ((!flag3) ? new Vector3(vector2.x, vector2.y, 0.002f) : new Vector3(vector2.x - 2f, vector2.y, 0.002f)));
			euler = new Vector3(0f, 0f, 180f);
			break;
		default:
			if (!flag2)
			{
				vector3 = ((!flag3) ? new Vector3(vector2.x, vector2.y, 0.002f) : new Vector3(vector2.x, vector2.y - 2f, 0.002f));
			}
			else
			{
				float num3 = position.x;
				if (num3 < vector.x - num)
				{
					num3 = vector.x - num;
				}
				if (num3 > vector.x + num)
				{
					num3 = vector.x + num;
				}
				vector3 = new Vector3(num3, Mathf.Min(vector.y + num2, position.y), 0.002f);
			}
			euler = new Vector3(0f, 0f, 270f);
			break;
		}
		if (flag)
		{
			vector3 = collider.ClosestPoint(vector3);
		}
		SpawnSlashReaction(vector3, Quaternion.Euler(euler));
		return default(IHitResponder.HitResponse);
	}

	private float GetActualHitDirection(DamageEnemies damager)
	{
		if (!damager)
		{
			return 0f;
		}
		if (!damager.CircleDirection)
		{
			return damager.GetDirection();
		}
		Vector2 vector = (Vector2)base.transform.position - (Vector2)damager.transform.position;
		return Mathf.Atan2(vector.y, vector.x) * 57.29578f;
	}
}
