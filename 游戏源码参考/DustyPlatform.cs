using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class DustyPlatform : DebugDrawColliderRuntimeAdder
{
	private BoxCollider2D bodyCollider;

	[SerializeField]
	private float inset;

	[SerializeField]
	private LayerMask dustIgnoredLayers;

	[SerializeField]
	private RandomAudioClipTable dustFallClips;

	[SerializeField]
	private AudioSource dustFallSourcePrefab;

	[SerializeField]
	private ParticleSystem dustPrefab;

	[SerializeField]
	private ParticleSystem rocksPrefab;

	[SerializeField]
	private float dustRateAreaFactor;

	[SerializeField]
	private float dustRateConstant;

	[SerializeField]
	private GameObject streamPrefab;

	[SerializeField]
	private Vector3 streamOffset;

	[SerializeField]
	private float rocksChance;

	[SerializeField]
	private float cooldownDuration;

	private float rocksDelayTimer;

	private float cooldownTimer;

	private bool isRunning;

	protected void Reset()
	{
		inset = 0.3f;
		dustIgnoredLayers.value = 327680;
		dustRateAreaFactor = 10f;
		dustRateConstant = 5f;
		streamOffset = new Vector3(0f, 0.1f, 0.01f);
		rocksChance = 0.5f;
		cooldownDuration = 0.45f;
	}

	protected override void Awake()
	{
		base.Awake();
		bodyCollider = GetComponent<BoxCollider2D>();
	}

	public override void AddDebugDrawComponent()
	{
		DebugDrawColliderRuntime.AddOrUpdate(base.gameObject, DebugDrawColliderRuntime.ColorType.TerrainCollider);
	}

	protected void Update()
	{
		if (!isRunning)
		{
			return;
		}
		bool flag = true;
		if (cooldownTimer > 0f)
		{
			cooldownTimer -= Time.deltaTime;
			if (cooldownTimer > 0f)
			{
				flag = false;
			}
		}
		if (flag)
		{
			isRunning = false;
		}
	}

	protected void OnCollisionEnter2D(Collision2D collision)
	{
		if (isRunning)
		{
			return;
		}
		int layer = collision.collider.gameObject.layer;
		if ((dustIgnoredLayers.value & (1 << layer)) != 0)
		{
			return;
		}
		Vector2 vector = Vector2.zero;
		if (collision.contacts.Length != 0)
		{
			vector = collision.contacts[0].normal;
		}
		if (!(Mathf.Abs(vector.y - -1f) > 0.1f))
		{
			Vector3 position = base.transform.position;
			dustFallClips.SpawnAndPlayOneShot(dustFallSourcePrefab, position);
			Vector2 vector2 = bodyCollider.size - new Vector2(inset, inset);
			Vector3 vector3 = position;
			vector3.z = -0.1f;
			if (dustPrefab != null)
			{
				ParticleSystem particleSystem = dustPrefab.Spawn(vector3);
				SetRateOverTime(particleSystem, vector2.x * vector2.y * dustRateAreaFactor + dustRateConstant);
				Transform transform = particleSystem.transform;
				transform.localScale = new Vector3(vector2.x, vector2.y, transform.localScale.z);
			}
			if (streamPrefab != null)
			{
				GameObject obj = streamPrefab.Spawn(vector3 + new Vector3(0f, (0f - bodyCollider.size.y) * 0.5f, 0.01f) + streamOffset);
				Vector3 localScale = obj.transform.localScale;
				localScale.x = vector2.x;
				obj.transform.localScale = localScale;
			}
			if (Random.value < rocksChance && rocksPrefab != null)
			{
				Transform transform2 = rocksPrefab.Spawn(vector3).transform;
				Vector3 position2 = transform2.position;
				transform2.position = new Vector3(position2.x, position2.y, 0.003f);
				transform2.localScale = new Vector3(vector2.x, vector2.y, transform2.localScale.z);
			}
			cooldownTimer = cooldownDuration;
			isRunning = true;
		}
	}

	private void SetRateOverTime(ParticleSystem ps, float rateOverTime)
	{
		ParticleSystem.EmissionModule emission = ps.emission;
		emission.rateOverTime = rateOverTime;
	}
}
