using UnityEngine;

public class SilkAcidRegion : MonoBehaviour
{
	[SerializeField]
	private HeroSilkAcid.SizzleTypes sizzleType;

	[Space]
	[SerializeField]
	private float sizzleStartDelay;

	[SerializeField]
	private float sizzleTime = 1f;

	[Space]
	[SerializeField]
	private int dispelAfterSilk;

	[SerializeField]
	private EventBase[] dispelEvents;

	[SerializeField]
	private PlayerDataTest protectCondition;

	private HeroSilkAcid hero;

	private int sizzledSilk;

	public HeroSilkAcid.SizzleTypes SizzleType => sizzleType;

	public float SizzleStartDelay => sizzleStartDelay;

	public float SizzleTime => sizzleTime;

	public bool IsProtected
	{
		get
		{
			if (protectCondition.IsDefined)
			{
				return protectCondition.IsFulfilled;
			}
			return false;
		}
	}

	private void OnEnable()
	{
		sizzledSilk = 0;
		if ((bool)base.transform.parent)
		{
			Entered(base.transform.parent.gameObject);
		}
		EventBase[] array = dispelEvents;
		foreach (EventBase eventBase in array)
		{
			if ((bool)eventBase)
			{
				eventBase.ReceivedEvent += Dispel;
			}
		}
	}

	private void OnDisable()
	{
		EventBase[] array = dispelEvents;
		foreach (EventBase eventBase in array)
		{
			if ((bool)eventBase)
			{
				eventBase.ReceivedEvent -= Dispel;
			}
		}
		Dispel();
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		Entered(collision.gameObject);
	}

	private void Entered(GameObject obj)
	{
		if (!hero)
		{
			hero = obj.GetComponent<HeroSilkAcid>();
			if ((bool)hero)
			{
				hero.AddInside(this);
			}
		}
	}

	private void OnTriggerExit2D(Collider2D collision)
	{
		if ((bool)hero && collision.GetComponent<HeroSilkAcid>() == hero)
		{
			Dispel();
		}
	}

	public void Dispel()
	{
		if ((bool)hero)
		{
			hero.RemoveInside(this);
			hero = null;
		}
	}

	public void OnTakenSilk(bool isAnySilkLeft)
	{
		sizzledSilk++;
		if (dispelAfterSilk > 0 && (sizzledSilk >= dispelAfterSilk || !isAnySilkLeft))
		{
			Dispel();
		}
	}
}
