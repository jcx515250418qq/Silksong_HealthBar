using System;
using System.Collections;
using UnityEngine;

public class StalactiteControl : MonoBehaviour
{
	public GameObject top;

	[Space]
	public float startFallOffset = 0.1f;

	public GameObject startFallEffect;

	public AudioClip startFallSound;

	public float fallDelay = 0.25f;

	[Space]
	public GameObject fallEffect;

	public GameObject trailEffect;

	public GameObject nailStrikePrefab;

	[Space]
	public GameObject embeddedVersion;

	public GameObject[] landEffectPrefabs;

	[Space]
	public float hitVelocity = 40f;

	[Space]
	public GameObject[] hitUpEffectPrefabs;

	public AudioClip hitSound;

	public GameObject hitUpRockPrefabs;

	public int spawnMin = 10;

	public int spawnMax = 12;

	public int speedMin = 15;

	public int speedMax = 20;

	public AudioClip breakSound;

	private TriggerEnterEvent trigger;

	private DamageHero heroDamage;

	private Rigidbody2D body;

	private AudioSource source;

	private DamageEnemies damageEnemies;

	private bool fallen;

	private void Awake()
	{
		body = GetComponent<Rigidbody2D>();
		source = GetComponent<AudioSource>();
		heroDamage = GetComponent<DamageHero>();
		damageEnemies = GetComponent<DamageEnemies>();
	}

	private void Start()
	{
		trigger = GetComponentInChildren<TriggerEnterEvent>();
		if ((bool)trigger)
		{
			trigger.OnTriggerEntered += HandleTriggerEnter;
		}
		if ((bool)heroDamage)
		{
			heroDamage.damageDealt = 0;
		}
		body.isKinematic = true;
		if ((bool)damageEnemies)
		{
			damageEnemies.enabled = false;
		}
	}

	private void HandleTriggerEnter(Collider2D collider, GameObject sender)
	{
		if (collider.tag == "Player" && Physics2D.Linecast(base.transform.position, collider.transform.position, 256).collider == null)
		{
			StartCoroutine(Fall(fallDelay));
			trigger.OnTriggerEntered -= HandleTriggerEnter;
			sender.SetActive(value: false);
		}
	}

	private IEnumerator Fall(float delay)
	{
		if ((bool)top)
		{
			top.transform.SetParent(base.transform.parent);
		}
		base.transform.position += Vector3.down * startFallOffset;
		if ((bool)startFallEffect)
		{
			startFallEffect.SetActive(value: true);
			startFallEffect.transform.SetParent(base.transform.parent);
		}
		if ((bool)source && (bool)startFallSound)
		{
			source.PlayOneShot(startFallSound);
		}
		yield return new WaitForSeconds(delay);
		if ((bool)fallEffect)
		{
			fallEffect.SetActive(value: true);
			fallEffect.transform.SetParent(base.transform.parent);
		}
		if ((bool)trailEffect)
		{
			trailEffect.SetActive(value: true);
		}
		if ((bool)heroDamage)
		{
			heroDamage.damageDealt = 1;
		}
		if ((bool)damageEnemies)
		{
			damageEnemies.enabled = true;
		}
		body.isKinematic = false;
		fallen = true;
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (fallen && collision.gameObject.layer == 8)
		{
			body.isKinematic = true;
			if ((bool)trailEffect)
			{
				trailEffect.transform.parent = null;
			}
			trailEffect.GetComponent<ParticleSystem>().Stop();
			if ((bool)embeddedVersion)
			{
				embeddedVersion.SetActive(value: true);
				embeddedVersion.transform.SetParent(base.transform.parent, worldPositionStays: true);
			}
			RaycastHit2D raycastHit2D = Helper.Raycast2D(base.transform.position, Vector2.down, 10f, 256);
			GameObject[] array = landEffectPrefabs;
			foreach (GameObject gameObject in array)
			{
				Vector3 vector = new Vector3(raycastHit2D.point.x, raycastHit2D.point.y, gameObject.transform.position.z);
				gameObject.Spawn((raycastHit2D.collider != null) ? vector : base.transform.position);
			}
			base.gameObject.SetActive(value: false);
		}
		else
		{
			if (!(collision.tag == "Nail Attack"))
			{
				return;
			}
			if (!fallen)
			{
				StartCoroutine(Fall(0f));
			}
			if ((bool)heroDamage)
			{
				heroDamage.damageDealt = 0;
				heroDamage = null;
			}
			float value = PlayMakerFSM.FindFsmOnGameObject(collision.gameObject, "damages_enemy").FsmVariables.FindFsmFloat("direction").Value;
			float num = 0f;
			if (value < 45f)
			{
				num = 45f;
			}
			else
			{
				if (value < 135f)
				{
					GameObject[] array = hitUpEffectPrefabs;
					for (int i = 0; i < array.Length; i++)
					{
						array[i].Spawn(base.transform.position);
					}
					FlingObjects();
					if ((bool)source && (bool)breakSound)
					{
						AudioSource audioSource = new GameObject("StalactiteBreakEffect").AddComponent<AudioSource>();
						audioSource.outputAudioMixerGroup = source.outputAudioMixerGroup;
						audioSource.loop = false;
						audioSource.playOnAwake = false;
						audioSource.rolloffMode = source.rolloffMode;
						audioSource.minDistance = source.minDistance;
						audioSource.maxDistance = source.maxDistance;
						audioSource.clip = breakSound;
						audioSource.volume = source.volume;
						audioSource.Play();
					}
					base.gameObject.SetActive(value: false);
					return;
				}
				if (value < 225f)
				{
					num = -45f;
				}
				else if (value < 360f)
				{
					num = 0f;
				}
			}
			_ = (Vector3)body.linearVelocity;
			Vector3 vector2 = Quaternion.Euler(0f, 0f, num) * Vector3.down * hitVelocity;
			body.rotation = num;
			body.gravityScale = 0f;
			body.linearVelocity = vector2;
			nailStrikePrefab.Spawn(base.transform.position);
			if ((bool)source && (bool)hitSound)
			{
				source.PlayOneShot(hitSound);
			}
		}
	}

	private void FlingObjects()
	{
		int num = UnityEngine.Random.Range(spawnMin, spawnMax + 1);
		Vector2 linearVelocity = default(Vector2);
		for (int i = 1; i <= num; i++)
		{
			GameObject obj = hitUpRockPrefabs.Spawn(base.transform.position, base.transform.rotation);
			_ = obj.transform.position;
			_ = obj.transform.position;
			_ = obj.transform.position;
			float num2 = UnityEngine.Random.Range(speedMin, speedMax);
			float num3 = UnityEngine.Random.Range(0f, 360f);
			float x = num2 * Mathf.Cos(num3 * (MathF.PI / 180f));
			float y = num2 * Mathf.Sin(num3 * (MathF.PI / 180f));
			linearVelocity.x = x;
			linearVelocity.y = y;
			Rigidbody2D component = obj.GetComponent<Rigidbody2D>();
			if ((bool)component)
			{
				component.linearVelocity = linearVelocity;
			}
		}
	}
}
