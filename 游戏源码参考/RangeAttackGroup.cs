using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public sealed class RangeAttackGroup : MonoBehaviour
{
	[SerializeField]
	private TrackTriggerObjects groupRange;

	[SerializeField]
	private List<RangeAttacker> rangeAttackers = new List<RangeAttacker>();

	[SerializeField]
	private float attackerAppearRadius = 3.7f;

	[SerializeField]
	private Vector3 appearOriginOffset = new Vector3(0f, 0.5f, 0f);

	[SerializeField]
	private TriggerEnterEvent explosionTrigger;

	[SerializeField]
	private TriggerEnterEvent customDamageTrigger;

	[SerializeField]
	private string customDamageEventRegister;

	[SerializeField]
	private Transform sinkTarget;

	private bool hasChildren;

	private bool anyAttackerActive;

	private List<Vector2> targetPositions = new List<Vector2>();

	private static int nextID;

	private int groupIDMask;

	private bool groupEnabled = true;

	private bool dirty;

	public bool GroupEnabled
	{
		get
		{
			return groupEnabled;
		}
		set
		{
			groupEnabled = value;
			base.gameObject.SetActive(value);
			foreach (RangeAttacker rangeAttacker in rangeAttackers)
			{
				if (!(rangeAttacker == null))
				{
					rangeAttacker.gameObject.SetActive(value);
				}
			}
		}
	}

	private void Awake()
	{
		groupIDMask = 1 << nextID++;
		if (nextID >= 32)
		{
			nextID = 0;
		}
		rangeAttackers.RemoveAll((RangeAttacker o) => o == null);
		foreach (RangeAttacker rangeAttacker in rangeAttackers)
		{
			rangeAttacker.SetOrigin(appearOriginOffset);
			rangeAttacker.MarkChild();
		}
		if (customDamageTrigger != null)
		{
			customDamageTrigger.OnTriggerEntered += OnCustomDamageTriggerEntered;
		}
		if ((bool)explosionTrigger)
		{
			explosionTrigger.OnTriggerEntered += OnExplosionTriggerEntered;
		}
		hasChildren = rangeAttackers.Count != 0;
		if (groupRange != null)
		{
			groupRange.InsideStateChanged += OnInsideStateChanged;
			if (hasChildren)
			{
				base.enabled = groupRange.IsInside;
			}
			else
			{
				base.enabled = false;
			}
		}
		else
		{
			base.enabled = false;
		}
	}

	private void OnDestroy()
	{
		if (groupRange != null)
		{
			groupRange.InsideStateChanged -= OnInsideStateChanged;
		}
		if (customDamageTrigger != null)
		{
			customDamageTrigger.OnTriggerEntered -= OnCustomDamageTriggerEntered;
		}
		if ((bool)explosionTrigger)
		{
			explosionTrigger.OnTriggerEntered -= OnExplosionTriggerEntered;
		}
	}

	private void OnValidate()
	{
		dirty = true;
	}

	private void OnDisable()
	{
		for (int i = 0; i < rangeAttackers.Count; i++)
		{
			rangeAttackers[i].SetInsideState(groupIDMask, isInside: false);
		}
	}

	private void FixedUpdate()
	{
		if (groupRange.insideObjectsList.Count == 0)
		{
			return;
		}
		targetPositions.Clear();
		anyAttackerActive = false;
		for (int i = 0; i < groupRange.insideObjectsList.Count; i++)
		{
			GameObject gameObject = groupRange.insideObjectsList[i];
			targetPositions.Add(gameObject.transform.position);
		}
		float num = attackerAppearRadius * attackerAppearRadius;
		for (int j = 0; j < rangeAttackers.Count; j++)
		{
			RangeAttacker rangeAttacker = rangeAttackers[j];
			Vector2 vector = rangeAttacker.GetOrigin();
			bool isInside = false;
			for (int k = 0; k < targetPositions.Count; k++)
			{
				if (((Vector3)(targetPositions[k] - vector)).sqrMagnitude <= num)
				{
					isInside = true;
					anyAttackerActive = true;
					break;
				}
			}
			rangeAttacker.SetInsideState(groupIDMask, isInside);
		}
	}

	[ContextMenu("Find Attackers In Region")]
	private void FindAttackersInRegion()
	{
		if (groupRange == null)
		{
			return;
		}
		Collider2D[] components = groupRange.GetComponents<Collider2D>();
		if (components.Length == 0)
		{
			return;
		}
		HashSet<RangeAttacker> hashSet = new HashSet<RangeAttacker>();
		RangeAttacker[] array = Object.FindObjectsByType<RangeAttacker>(FindObjectsInactive.Include, FindObjectsSortMode.None);
		foreach (RangeAttacker rangeAttacker in array)
		{
			Vector2 point = rangeAttacker.transform.position;
			Collider2D[] array2 = components;
			for (int j = 0; j < array2.Length; j++)
			{
				if (array2[j].OverlapPoint(point))
				{
					hashSet.Add(rangeAttacker);
					break;
				}
			}
		}
		rangeAttackers.RemoveAll((RangeAttacker o) => o == null);
		rangeAttackers = rangeAttackers.Union(hashSet).ToList();
	}

	private void SetAttackersAsChildren()
	{
		rangeAttackers.RemoveAll((RangeAttacker o) => o == null);
		foreach (RangeAttacker rangeAttacker in rangeAttackers)
		{
			_ = rangeAttacker;
		}
	}

	private void CleanUpChildObjects()
	{
		foreach (RangeAttacker rangeAttacker in rangeAttackers)
		{
			rangeAttacker.CleanChild();
		}
	}

	[ContextMenu("Set Attackers as children and clean up")]
	private void SetAttackersAsChildrenAndClean()
	{
		SetAttackersAsChildren();
		CleanUpChildObjects();
	}

	private void OnInsideStateChanged(bool inside)
	{
		if (hasChildren)
		{
			base.enabled = inside;
		}
	}

	private void OnCustomDamageTriggerEntered(Collider2D collision, GameObject sender)
	{
		if (anyAttackerActive && collision.gameObject.layer == 20 && (!collision.gameObject.GetComponentInParent<HeroController>() || CheatManager.Invincibility != CheatManager.InvincibilityStates.FullInvincible))
		{
			if ((bool)sinkTarget)
			{
				Vector2 vector = base.transform.position;
				RangeAttacker.LastDamageSinkDirection = ((Vector2)sinkTarget.position - vector).normalized;
			}
			EventRegister.SendEvent(customDamageEventRegister);
		}
	}

	private bool CanDamagerShred(DamageEnemies otherDamager)
	{
		if (otherDamager.attackType == AttackTypes.Explosion || otherDamager.CompareTag("Explosion"))
		{
			return true;
		}
		ToolItem representingTool = otherDamager.RepresentingTool;
		if ((bool)representingTool && (representingTool.DamageFlags & ToolDamageFlags.Shredding) != 0)
		{
			return true;
		}
		return false;
	}

	private void OnExplosionTriggerEntered(Collider2D collision, GameObject sender)
	{
		DamageEnemies component = collision.GetComponent<DamageEnemies>();
		if (!component || !CanDamagerShred(component))
		{
			return;
		}
		float num = 4f;
		num = ((!(collision is CircleCollider2D circleCollider2D)) ? ((collision.bounds.extents.x + collision.bounds.extents.y) * 0.5f) : (circleCollider2D.radius * collision.transform.localScale.x));
		num += 2f;
		num *= num;
		Vector3 position = collision.transform.position;
		for (int i = 0; i < rangeAttackers.Count; i++)
		{
			RangeAttacker rangeAttacker = rangeAttackers[i];
			if (Vector3.SqrMagnitude(rangeAttacker.GetOrigin() - position) <= num)
			{
				rangeAttacker.ReactToExplosion();
			}
		}
	}
}
