using System;
using GlobalSettings;
using UnityEngine;

public class ShardRegion : DebugDrawColliderRuntimeAdder, ICurrencyLimitRegion
{
	[Serializable]
	private class ProbabilityShardDrop : Probability.ProbabilityBase<int>
	{
		[SerializeField]
		private int dropAmount;

		public override int Item => dropAmount;
	}

	[SerializeField]
	private ProbabilityShardDrop[] dropChances;

	[SerializeField]
	private float architectCrestChanceMultiplier = 1f / 3f;

	[SerializeField]
	private FlingUtils.ObjectFlingParams shardFling;

	[SerializeField]
	private GameObject dropEffectPrefab;

	[SerializeField]
	private AudioEventRandom hitSound;

	[Space]
	[SerializeField]
	private int limit = 30;

	private BoxCollider2D box;

	private bool hasBox;

	private float[] architectProbabilities;

	private bool hasHC;

	private HeroController hc;

	public CurrencyType CurrencyType => CurrencyType.Shard;

	public int Limit => limit;

	protected override void Awake()
	{
		base.Awake();
		box = GetComponent<BoxCollider2D>();
		hasBox = box;
		architectProbabilities = new float[dropChances.Length];
		for (int i = 0; i < dropChances.Length; i++)
		{
			ProbabilityShardDrop obj = dropChances[i];
			float num = obj.Probability;
			if (obj.Item > 0)
			{
				num *= architectCrestChanceMultiplier;
			}
			architectProbabilities[i] = num;
		}
	}

	private void Start()
	{
		hc = HeroController.instance;
		hasHC = hc != null;
	}

	private void OnEnable()
	{
		NailSlashTerrainThunk.AnyThunked += OnNailSlashTerrainThunked;
		CurrencyObjectLimitRegion.AddRegion(this);
	}

	private void OnDisable()
	{
		NailSlashTerrainThunk.AnyThunked -= OnNailSlashTerrainThunked;
		CurrencyObjectLimitRegion.RemoveRegion(this);
	}

	private void OnNailSlashTerrainThunked(Vector2 thunkPos, int surfaceDir)
	{
		if (!hasBox || !box.OverlapPoint(thunkPos))
		{
			return;
		}
		float[] overrideProbabilities = null;
		if (hasHC && hc.IsArchitectCrestEquipped())
		{
			overrideProbabilities = architectProbabilities;
		}
		int randomItemByProbability = Probability.GetRandomItemByProbability<ProbabilityShardDrop, int>(dropChances, overrideProbabilities);
		if ((bool)dropEffectPrefab)
		{
			dropEffectPrefab.Spawn(thunkPos);
		}
		hitSound.SpawnAndPlayOneShot(thunkPos);
		if (randomItemByProbability > 0)
		{
			FlingUtils.ObjectFlingParams objectFlingParams = shardFling;
			switch (surfaceDir)
			{
			case 1:
				thunkPos += new Vector2(0f, 0.5f);
				objectFlingParams.AngleMin -= 90f;
				objectFlingParams.AngleMax -= 90f;
				break;
			case 3:
				thunkPos += new Vector2(0f, -0.5f);
				objectFlingParams.AngleMin += 90f;
				objectFlingParams.AngleMax += 90f;
				break;
			case 2:
				thunkPos += new Vector2(-0.5f, 0f);
				objectFlingParams.AngleMin += 180f;
				objectFlingParams.AngleMax += 180f;
				break;
			case 0:
				thunkPos += new Vector2(0.5f, 0f);
				break;
			}
			for (int i = 0; i < randomItemByProbability; i++)
			{
				GameObject flingObject = Gameplay.ShellShardPrefab.Spawn(thunkPos);
				FlingUtils.FlingObject(objectFlingParams.GetSelfConfig(flingObject), null, thunkPos);
			}
		}
	}

	public bool IsInsideLimitRegion(Vector2 point)
	{
		if (hasBox)
		{
			return box.OverlapPoint(point);
		}
		return false;
	}

	public override void AddDebugDrawComponent()
	{
		DebugDrawColliderRuntime.AddOrUpdate(base.gameObject, DebugDrawColliderRuntime.ColorType.ShardRegion);
	}
}
