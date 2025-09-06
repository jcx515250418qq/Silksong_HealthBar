using System.Collections.Generic;
using GlobalSettings;
using UnityEngine;
using UnityEngine.Events;

public class SilkGrubCocoon : MonoBehaviour
{
	[SerializeField]
	private PersistentBoolItem persistent;

	[SerializeField]
	private TrackTriggerObjects grubSingRange;

	[SerializeField]
	private Transform hitEffectSpawnPoint;

	[SerializeField]
	private GameObject[] hitEffectPrefabs;

	[SerializeField]
	private GameObject enableOnBreak;

	[SerializeField]
	private GameObject[] breakEffectPrefabs;

	[SerializeField]
	private BloodSpawner.Config hitBlood;

	[SerializeField]
	private BloodSpawner.Config breakBlood;

	[SerializeField]
	private CameraShakeTarget breakCameraShake;

	[SerializeField]
	private int hitsToBreak;

	[SerializeField]
	private CollectableItem dropItem;

	[SerializeField]
	private CollectableItemPickup dropItemPrefab;

	[SerializeField]
	private Transform dropItemSpawnPoint;

	[SerializeField]
	private FlingUtils.ObjectFlingParams dropItemFling;

	[SerializeField]
	private GameObject soulThreadPrefab;

	[SerializeField]
	[PlayerDataField(typeof(bool), false)]
	private string setPDBoolOnBreak;

	[SerializeField]
	[PlayerDataField(typeof(bool), false)]
	private string unsetPDBoolOnBreak;

	[Space]
	public UnityEvent OnHit;

	private int hitsLeft;

	private bool isBroken;

	private float addSilkTimer;

	private static readonly List<SilkGrubCocoon> _cocoons = new List<SilkGrubCocoon>();

	public static bool IsAnyActive
	{
		get
		{
			foreach (SilkGrubCocoon cocoon in _cocoons)
			{
				if (!cocoon.isBroken && (bool)cocoon.grubSingRange && cocoon.grubSingRange.IsInside)
				{
					return true;
				}
			}
			return false;
		}
	}

	private void Awake()
	{
		if (!persistent)
		{
			return;
		}
		persistent.OnGetSaveState += delegate(out bool value)
		{
			value = isBroken;
		};
		persistent.OnSetSaveState += delegate(bool value)
		{
			isBroken = value;
			if (isBroken)
			{
				SetBroken();
			}
		};
	}

	private void OnEnable()
	{
		_cocoons.Add(this);
	}

	private void OnDisable()
	{
		_cocoons.Remove(this);
	}

	private void Start()
	{
		hitsLeft = hitsToBreak;
		if ((bool)enableOnBreak)
		{
			enableOnBreak.SetActive(value: false);
		}
	}

	private void SetBroken()
	{
		isBroken = true;
		if ((bool)persistent)
		{
			persistent.SaveState();
		}
		base.gameObject.SetActive(value: false);
	}

	public void WasHit()
	{
		if (isBroken)
		{
			return;
		}
		Transform transform = (hitEffectSpawnPoint ? hitEffectSpawnPoint : base.transform);
		Vector3 position = transform.position;
		HeroController.instance.SilkGain();
		hitEffectPrefabs.SpawnAll(position);
		hitsLeft--;
		if (hitsLeft > 0)
		{
			BloodSpawner.SpawnBlood(hitBlood, transform, null);
			OnHit.Invoke();
			return;
		}
		BloodSpawner.SpawnBlood(breakBlood, transform, null);
		CollectableItemPickup collectableItemPickup = (dropItemPrefab ? dropItemPrefab : Gameplay.CollectableItemPickupInstantPrefab);
		if ((bool)collectableItemPickup && (bool)dropItem)
		{
			Transform transform2 = (dropItemSpawnPoint ? dropItemSpawnPoint : base.transform);
			CollectableItemPickup collectableItemPickup2 = Object.Instantiate(collectableItemPickup);
			collectableItemPickup2.transform.SetPosition2D(transform2.position);
			collectableItemPickup2.SetItem(dropItem);
			FlingUtils.FlingObject(dropItemFling.GetSelfConfig(collectableItemPickup2.gameObject), transform2, Vector2.zero);
		}
		breakCameraShake.DoShake(this);
		if ((bool)enableOnBreak)
		{
			enableOnBreak.transform.SetParent(null, worldPositionStays: true);
			enableOnBreak.SetActive(value: true);
		}
		breakEffectPrefabs.SpawnAll(position);
		if (setPDBoolOnBreak != "")
		{
			GameManager.instance.playerData.SetBool(setPDBoolOnBreak, value: true);
		}
		if (unsetPDBoolOnBreak != "")
		{
			GameManager.instance.playerData.SetBool(unsetPDBoolOnBreak, value: false);
		}
		NoiseMaker component = GetComponent<NoiseMaker>();
		if ((bool)component)
		{
			component.CreateNoise();
		}
		SetBroken();
	}
}
