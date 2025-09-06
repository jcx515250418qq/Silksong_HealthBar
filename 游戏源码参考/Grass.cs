using System.Collections.Generic;
using UnityEngine;

public class Grass : MonoBehaviour, IHitResponder
{
	public enum GrassTypes
	{
		White = 0,
		Green = 1,
		SimpleType = 2,
		Rag = 3,
		ChildType = 4
	}

	private Animator animator;

	private Collider2D bodyCollider;

	private AudioSource audioSource;

	[SerializeField]
	private bool isInfectable;

	[SerializeField]
	private float inertBackgroundThreshold;

	[SerializeField]
	private float inertForegroundThreshold;

	[SerializeField]
	private Color infectedColor;

	[SerializeField]
	private bool preventPushAnimation;

	[SerializeField]
	private GameObject slashImpactPrefab;

	[SerializeField]
	private float slashImpactRotationMin;

	[SerializeField]
	private float slashImpactRotationMax;

	[SerializeField]
	private float slashImpactScale;

	[SerializeField]
	private GameObject infectedCutPrefab0;

	[SerializeField]
	private GameObject infectedCutPrefab1;

	[SerializeField]
	private GameObject cutPrefab0;

	[SerializeField]
	private GameObject cutPrefab1;

	[SerializeField]
	private ParticleSystem childParticleSystem;

	[SerializeField]
	private float childParticleSystemDuration;

	[SerializeField]
	private RandomAudioClipTable pushAudioClipTable;

	[SerializeField]
	private RandomAudioClipTable cutAudioClipTable;

	private static readonly int IdleStateId = Animator.StringToHash("Idle");

	private static readonly int PushStateId = Animator.StringToHash("Push");

	private static readonly int DeadStateId = Animator.StringToHash("Dead");

	private bool isInfected;

	private bool isCut;

	private float childParticleSystemTimer;

	private static List<Grass> grasses;

	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
	private static void Init()
	{
		grasses = new List<Grass>();
	}

	protected void Reset()
	{
		inertBackgroundThreshold = 1.8f;
		inertForegroundThreshold = -1.8f;
		infectedColor = new Color32(byte.MaxValue, 140, 54, byte.MaxValue);
		slashImpactRotationMin = 340f;
		slashImpactRotationMax = 380f;
		slashImpactScale = 0.6f;
		preventPushAnimation = false;
		childParticleSystemDuration = 5f;
	}

	protected void Awake()
	{
		animator = GetComponent<Animator>();
		bodyCollider = GetComponent<Collider2D>();
		audioSource = GetComponent<AudioSource>();
		grasses.Add(this);
	}

	protected void OnDestroy()
	{
		grasses.Remove(this);
	}

	protected void Start()
	{
		float z = base.transform.position.z;
		if (z > inertBackgroundThreshold || z < inertForegroundThreshold)
		{
			base.enabled = false;
			return;
		}
		isInfected = isInfectable && GameObject.FindGameObjectWithTag("Infected Flag") != null;
		if (isInfected)
		{
			FSMActionReplacements.SetMaterialColor(this, infectedColor);
		}
		animator.Play(IdleStateId, 0, Random.Range(0f, 1f));
	}

	protected void OnTriggerEnter2D(Collider2D other)
	{
		if (!preventPushAnimation)
		{
			Push(isAllGrass: false);
		}
	}

	public void Push(bool isAllGrass)
	{
		if (!isCut)
		{
			if (!isAllGrass)
			{
				pushAudioClipTable.PlayOneShot(audioSource);
			}
			animator.Play(PushStateId, 0, 0f);
		}
	}

	public static void PushAll()
	{
		if (grasses != null)
		{
			for (int i = 0; i < grasses.Count; i++)
			{
				grasses[i].Push(isAllGrass: true);
			}
		}
	}

	public IHitResponder.HitResponse Hit(HitInstance damageInstance)
	{
		if (damageInstance.DamageDealt <= 0)
		{
			return IHitResponder.Response.None;
		}
		if (isCut)
		{
			return IHitResponder.Response.None;
		}
		isCut = true;
		bodyCollider.enabled = false;
		FSMActionReplacements.Directions directions = FSMActionReplacements.CheckDirectionWithBrokenBehaviour(0f);
		GameObject obj = slashImpactPrefab.Spawn(base.transform.position, Quaternion.Euler(0f, 0f, Random.Range(slashImpactRotationMin, slashImpactRotationMax)));
		obj.transform.localScale = new Vector3((directions == FSMActionReplacements.Directions.Left) ? (0f - slashImpactScale) : slashImpactScale, slashImpactScale, 1f);
		Vector3 localPosition = obj.transform.localPosition;
		localPosition.z = 0f;
		obj.transform.localPosition = localPosition;
		Quaternion rotation = Quaternion.Euler(-90f, -90f, -0.01f);
		if (isInfected)
		{
			if (infectedCutPrefab0 != null)
			{
				infectedCutPrefab0.Spawn(base.transform.position, rotation);
			}
			if (infectedCutPrefab1 != null)
			{
				infectedCutPrefab1.Spawn(base.transform.position, rotation);
			}
		}
		else
		{
			if (cutPrefab0 != null)
			{
				cutPrefab0.Spawn(base.transform.position, rotation);
			}
			if (cutPrefab1 != null)
			{
				cutPrefab1.Spawn(base.transform.position, rotation);
			}
		}
		cutAudioClipTable.PlayOneShot(audioSource);
		animator.Play(DeadStateId, 0, 0f);
		if (!isInfected && childParticleSystem != null)
		{
			childParticleSystem.Play();
			childParticleSystemTimer = childParticleSystemDuration;
			base.enabled = true;
		}
		else
		{
			base.enabled = false;
		}
		return IHitResponder.Response.GenericHit;
	}

	protected void Update()
	{
		childParticleSystemTimer -= Time.deltaTime;
		if (childParticleSystemTimer <= 0f)
		{
			if (childParticleSystem != null)
			{
				childParticleSystem.Stop();
			}
			base.enabled = false;
		}
	}
}
