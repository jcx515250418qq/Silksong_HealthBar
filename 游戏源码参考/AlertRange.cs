using System;
using UnityEngine;

public class AlertRange : TrackTriggerObjects
{
	private enum LineOfSightChecks
	{
		None = 0,
		Self = 1,
		Parent = 2
	}

	[SerializeField]
	[HideInInspector]
	[Obsolete]
	private bool checkParentSight;

	[Space]
	[SerializeField]
	private LineOfSightChecks lineOfSight;

	[Space]
	[SerializeField]
	private bool countUnalertTime;

	private float unalertTimer;

	private Transform initialParent;

	private bool isHeroInRange;

	private bool haveLineOfSight;

	private HeroController hero;

	private bool hasHero;

	public bool ChecksLineOfSight => lineOfSight != LineOfSightChecks.None;

	private void OnValidate()
	{
		if (checkParentSight)
		{
			lineOfSight = LineOfSightChecks.Parent;
			checkParentSight = false;
		}
	}

	protected override void Awake()
	{
		base.Awake();
		OnValidate();
		hero = HeroController.instance;
		hasHero = hero != null;
		initialParent = base.transform.parent;
		HealthManager componentInParent = GetComponentInParent<HealthManager>();
		if ((bool)componentInParent)
		{
			componentInParent.TookDamage += delegate
			{
				unalertTimer = 0f;
			};
		}
	}

	protected override void OnEnable()
	{
		base.OnEnable();
		hero = HeroController.instance;
		hasHero = hero != null;
	}

	protected override void OnDisable()
	{
		base.OnDisable();
		haveLineOfSight = false;
		isHeroInRange = false;
	}

	protected override void OnInsideStateChanged(bool isInside)
	{
		isHeroInRange = isInside;
	}

	private void Update()
	{
		if (!countUnalertTime)
		{
			return;
		}
		if (isHeroInRange && (haveLineOfSight || lineOfSight == LineOfSightChecks.None))
		{
			unalertTimer = 0f;
			return;
		}
		unalertTimer += Time.deltaTime;
		if (unalertTimer > 100f)
		{
			unalertTimer = 100f;
		}
	}

	private void FixedUpdate()
	{
		if (isHeroInRange)
		{
			UpdateLineOfSight();
		}
	}

	private void UpdateLineOfSight()
	{
		if (!ChecksLineOfSight)
		{
			return;
		}
		if (!hasHero)
		{
			haveLineOfSight = false;
			return;
		}
		Transform transform;
		switch (lineOfSight)
		{
		case LineOfSightChecks.Self:
			transform = base.transform;
			break;
		case LineOfSightChecks.Parent:
		{
			Transform parent = base.transform.parent;
			transform = (parent ? parent : initialParent);
			break;
		}
		default:
			throw new NotImplementedException();
		}
		if (!transform)
		{
			haveLineOfSight = false;
			return;
		}
		Vector2 start = transform.position;
		Vector2 end = hero.transform.position;
		haveLineOfSight = !Helper.LineCast2DHit(start, end, 256, out var _);
	}

	public bool IsHeroInRange()
	{
		if (!ChecksLineOfSight || haveLineOfSight)
		{
			return isHeroInRange;
		}
		return false;
	}

	public float GetUnalertTime()
	{
		return unalertTimer;
	}

	public static AlertRange Find(GameObject root, string childName)
	{
		if (root == null)
		{
			return null;
		}
		bool flag = !string.IsNullOrEmpty(childName);
		Transform transform = root.transform;
		for (int i = 0; i < transform.childCount; i++)
		{
			Transform child = transform.GetChild(i);
			AlertRange component = child.GetComponent<AlertRange>();
			if (!(component == null) && (!flag || !(child.gameObject.name != childName)))
			{
				return component;
			}
		}
		return null;
	}
}
