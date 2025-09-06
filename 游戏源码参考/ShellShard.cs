using System.Collections;
using TeamCherry.SharedUtils;
using UnityEngine;

public class ShellShard : CurrencyObject<ShellShard>, IBreakOnContact, AntRegion.ICheck
{
	[Space]
	[SerializeField]
	private GameObject extraRendererObjects;

	[Space]
	[SerializeField]
	private int value = 1;

	[SerializeField]
	private PooledEffectProfile shineEffectProfile;

	[SerializeField]
	private GameObject shineEffectPrefab;

	[SerializeField]
	[Range(0f, 1f)]
	private float shineSpawnPercentage;

	[SerializeField]
	private MinMaxFloat shineEffectStartDelay;

	[SerializeField]
	private MinMaxFloat shineEffectFrequency = new MinMaxFloat(1f, 8f);

	private GameObject shineEffect;

	private bool hasShineEffect;

	private Coroutine shineRoutine;

	private bool hasExtrRenderer;

	protected override CurrencyType? CurrencyType => global::CurrencyType.Shard;

	public bool CanEnterAntRegion => true;

	protected override bool Collected()
	{
		CurrencyManager.AddShards(value);
		return true;
	}

	protected override void Awake()
	{
		base.Awake();
		hasShineEffect = shineEffectProfile != null;
		if (!hasShineEffect && (bool)shineEffectPrefab)
		{
			shineEffect = Object.Instantiate(shineEffectPrefab, Vector3.zero, Quaternion.Euler(0f, 0f, Random.Range(0f, 360f)), base.transform);
			shineEffect.transform.localPosition = new Vector3(0f, 0f, -0.0001f);
		}
		hasExtrRenderer = extraRendererObjects != null;
	}

	protected override void OnEnable()
	{
		base.OnEnable();
		if (hasShineEffect)
		{
			if (Random.Range(0f, 1f) <= shineSpawnPercentage)
			{
				shineRoutine = StartCoroutine(EnableShineEffect());
			}
		}
		else if ((bool)shineEffect)
		{
			shineEffect.SetActive(value: false);
			if (Random.Range(0f, 1f) <= shineSpawnPercentage)
			{
				shineRoutine = StartCoroutine(EnableShineEffect());
			}
		}
	}

	protected override void OnDisable()
	{
		base.OnDisable();
		StopAllCoroutines();
	}

	protected override void SetRendererActive(bool active)
	{
		base.SetRendererActive(active);
		if (hasExtrRenderer)
		{
			extraRendererObjects.SetActive(active);
		}
	}

	public override void CollectPopup()
	{
		PlayerData instance = PlayerData.instance;
		if (!instance.HasSeenShellShards)
		{
			instance.HasSeenShellShards = true;
			if (!popupName.IsEmpty)
			{
				UIMsgDisplay uIMsgDisplay = default(UIMsgDisplay);
				uIMsgDisplay.Name = popupName;
				uIMsgDisplay.Icon = popupSprite;
				uIMsgDisplay.IconScale = 1f;
				CollectableUIMsg.Spawn(uIMsgDisplay);
			}
		}
	}

	private IEnumerator EnableShineEffect()
	{
		yield return new WaitForSeconds(shineEffectStartDelay.GetRandomValue());
		if (hasShineEffect)
		{
			shineEffectProfile.SpawnEffect(new Vector3(0f, 0f, -0.0001f), Quaternion.Euler(0f, 0f, Random.Range(0f, 360f)), base.transform);
			float t = shineEffectFrequency.GetRandomValue();
			while (true)
			{
				t -= Time.deltaTime;
				if (t <= 0f)
				{
					t = shineEffectFrequency.GetRandomValue();
					shineEffectProfile.SpawnEffect(new Vector3(0f, 0f, -0.0001f), Quaternion.Euler(0f, 0f, Random.Range(0f, 360f)), base.transform);
				}
				yield return null;
			}
		}
		shineEffect.SetActive(value: true);
		shineRoutine = null;
	}

	public override void BurnUp()
	{
		base.BurnUp();
		if (shineRoutine != null)
		{
			StopCoroutine(shineRoutine);
			shineRoutine = null;
		}
		if ((bool)shineEffect)
		{
			shineEffect.SetActive(value: false);
		}
	}

	public bool TryGetRenderer(out Renderer renderer)
	{
		if ((bool)extraRendererObjects)
		{
			renderer = extraRendererObjects.GetComponentInChildren<Renderer>();
			if (renderer != null)
			{
				return true;
			}
		}
		renderer = null;
		return false;
	}
}
