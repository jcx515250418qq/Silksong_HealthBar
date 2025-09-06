using System;
using UnityEngine;
using UnityEngine.Events;

public class SuspendedPlatformCut : MonoBehaviour, IHitResponder
{
	[SerializeField]
	[HideInInspector]
	[Obsolete]
	private GameObject sprites;

	[SerializeField]
	private GameObject[] disableGameObjects;

	[Space]
	[SerializeField]
	private GameObject cutParticles;

	[SerializeField]
	private GameObject cutPointParticles;

	[SerializeField]
	private GameObject cutEffectPrefab;

	[Space]
	[SerializeField]
	private AudioClip cutSound;

	[SerializeField]
	private AudioClip finalCutSound;

	[Space]
	[SerializeField]
	private TrackTriggerObjects canCutTrigger;

	[SerializeField]
	private int cutsToBreak = 1;

	[Space]
	[SerializeField]
	private UnityEvent onHit;

	[SerializeField]
	private UnityEvent onBreak;

	private bool activated;

	private int cutsLeft;

	private SuspendedPlatformBase platform;

	private AudioSource audioSource;

	private Collider2D col;

	private void OnValidate()
	{
		if (cutsToBreak < 1)
		{
			cutsToBreak = 1;
		}
		if ((bool)sprites)
		{
			disableGameObjects = new GameObject[1] { sprites };
			sprites = null;
		}
	}

	private void Awake()
	{
		OnValidate();
		platform = GetComponentInParent<SuspendedPlatformBase>();
		audioSource = GetComponentInParent<AudioSource>();
		col = GetComponent<Collider2D>();
	}

	private void OnEnable()
	{
		cutsLeft = cutsToBreak;
		if ((bool)cutPointParticles)
		{
			cutPointParticles.SetActive(value: false);
		}
		if ((bool)cutParticles)
		{
			cutParticles.SetActive(value: false);
		}
	}

	public void Cut()
	{
		activated = true;
		if ((bool)cutParticles)
		{
			cutParticles.SetActive(value: true);
		}
		if ((bool)audioSource && (bool)finalCutSound)
		{
			audioSource.PlayOneShot(finalCutSound);
		}
		platform.CutDown();
		onBreak.Invoke();
		Disable();
	}

	public void Disable()
	{
		disableGameObjects.SetAllActive(value: false);
	}

	public IHitResponder.HitResponse Hit(HitInstance damageInstance)
	{
		if (activated)
		{
			return IHitResponder.Response.None;
		}
		if ((bool)canCutTrigger && !canCutTrigger.IsInside)
		{
			return IHitResponder.Response.None;
		}
		AttackTypes attackType = damageInstance.AttackType;
		if (attackType != 0 && attackType != AttackTypes.Heavy && !damageInstance.IsNailTag)
		{
			return IHitResponder.Response.None;
		}
		Vector3 position = damageInstance.Source.transform.position;
		if (position.y < col.bounds.min.y || position.y > col.bounds.max.y)
		{
			return IHitResponder.Response.None;
		}
		Vector3 position2;
		if ((bool)cutPointParticles)
		{
			position2 = cutPointParticles.transform.position;
			position2.y = position.y;
			if ((bool)cutPointParticles)
			{
				cutPointParticles.SetActive(value: false);
				cutPointParticles.transform.position = position2;
				cutPointParticles.SetActive(value: true);
			}
		}
		else
		{
			position2 = base.transform.position;
			position2.y = position.y;
		}
		if ((bool)cutEffectPrefab)
		{
			cutEffectPrefab.Spawn(position2);
		}
		onHit.Invoke();
		if ((bool)audioSource && (bool)cutSound)
		{
			audioSource.PlayOneShot(cutSound);
		}
		cutsLeft--;
		if (cutsLeft <= 0)
		{
			Cut();
		}
		return IHitResponder.Response.GenericHit;
	}
}
