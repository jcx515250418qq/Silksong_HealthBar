using System;
using System.Collections.Generic;
using TeamCherry.SharedUtils;
using UnityEngine;
using UnityEngine.Pool;

public sealed class RainVibrationRegion : MonoBehaviour
{
	public sealed class RainVibrationPool : IDisposable
	{
		public sealed class PooledRain
		{
			private RainVibrationPool pool;

			public VibrationEmission emission;

			public PooledRain(RainVibrationPool pool, VibrationEmission emission)
			{
				this.pool = pool;
				this.emission = emission;
			}

			public void Release()
			{
				pool?.Release(this);
			}
		}

		private IObjectPool<PooledRain> pool;

		private VibrationDataAsset source;

		private bool loop;

		private bool isRealTime;

		private bool updating;

		private bool disposed;

		private string tag;

		public RainVibrationPool(VibrationDataAsset source)
		{
			this.source = source;
			pool = new LinkedPool<PooledRain>(CreateFunc, ActionOnGet, ActionOnRelease, ActionOnDestroy, collectionCheck: false, 20);
		}

		private PooledRain CreateFunc()
		{
			VibrationData vibrationData = source;
			bool isLooping = loop;
			bool isRealtime = isRealTime;
			string text = tag;
			return new PooledRain(this, VibrationManager.PlayVibrationClipOneShot(vibrationData, null, isLooping, text, isRealtime));
		}

		private void ActionOnGet(PooledRain obj)
		{
		}

		private void ActionOnRelease(PooledRain obj)
		{
			obj.emission?.Stop();
		}

		private void ActionOnDestroy(PooledRain obj)
		{
			obj.emission?.Stop();
		}

		public PooledRain Get()
		{
			if (disposed)
			{
				return null;
			}
			return pool.Get();
		}

		public void Release(PooledRain vibrationEmission)
		{
			if (vibrationEmission != null)
			{
				pool.Release(vibrationEmission);
			}
		}

		private void ReleaseUnmanagedResources()
		{
			if (!disposed)
			{
				disposed = true;
				pool.Clear();
				pool = null;
			}
		}

		public void Dispose()
		{
			ReleaseUnmanagedResources();
			GC.SuppressFinalize(this);
		}

		~RainVibrationPool()
		{
			ReleaseUnmanagedResources();
		}
	}

	[Serializable]
	public sealed class RainVibrationPlayer
	{
		public VibrationDataAsset vibration;

		public float strength = 1f;

		private bool init;

		private RainVibrationPool pool;

		~RainVibrationPlayer()
		{
			pool?.Dispose();
		}

		private void Init()
		{
			if (!init)
			{
				init = true;
				pool = new RainVibrationPool(vibration);
			}
		}

		public RainVibrationPool.PooledRain PlayVibration(float strengthMultiplier)
		{
			Init();
			RainVibrationPool.PooledRain pooledRain = pool.Get();
			VibrationEmission emission = pooledRain.emission;
			if (emission != null)
			{
				emission.SetStrength(strength * strengthMultiplier);
				VibrationManager.PlayVibrationClipOneShot(emission);
			}
			return pooledRain;
		}
	}

	[SerializeField]
	private List<RainVibrationPlayer> rainVibrations = new List<RainVibrationPlayer>();

	[Space]
	[SerializeField]
	private MinMaxFloat frequency = new MinMaxFloat(0.1f, 1.5f);

	[SerializeField]
	private bool useStrengthMultiplier;

	[SerializeField]
	private MinMaxFloat strengthMultiplier = new MinMaxFloat(0.9f, 1.1f);

	private LinkedList<RainVibrationPool.PooledRain> activeEmissions = new LinkedList<RainVibrationPool.PooledRain>();

	private float timer;

	private bool isInside;

	public static readonly List<RainVibrationRegion> rainVibrationRegions = new List<RainVibrationRegion>();

	public List<RainVibrationPlayer> RainVibrations
	{
		get
		{
			return rainVibrations;
		}
		set
		{
			rainVibrations = value;
		}
	}

	public MinMaxFloat Frequency
	{
		get
		{
			return frequency;
		}
		set
		{
			frequency = value;
		}
	}

	public bool UseStrengthMultiplier
	{
		get
		{
			return useStrengthMultiplier;
		}
		set
		{
			useStrengthMultiplier = value;
		}
	}

	public MinMaxFloat StrengthMultiplier
	{
		get
		{
			return strengthMultiplier;
		}
		set
		{
			strengthMultiplier = value;
		}
	}

	private void Start()
	{
		rainVibrationRegions.Add(this);
		if (!isInside)
		{
			base.enabled = false;
		}
	}

	private void OnDestroy()
	{
		foreach (RainVibrationPool.PooledRain activeEmission in activeEmissions)
		{
			activeEmission.emission?.Stop();
		}
		activeEmissions.Clear();
		rainVibrationRegions.Remove(this);
	}

	private void Update()
	{
		timer -= Time.deltaTime;
		int count = activeEmissions.Count;
		while (count-- > 0)
		{
			LinkedListNode<RainVibrationPool.PooledRain> first = activeEmissions.First;
			RainVibrationPool.PooledRain value = first.Value;
			VibrationEmission emission = value.emission;
			if (emission != null && emission.IsPlaying)
			{
				break;
			}
			value.Release();
			activeEmissions.Remove(first);
		}
		if (!(timer > 0f))
		{
			timer = frequency.GetRandomValue();
			PlayRainVibration();
		}
	}

	private void OnTriggerEnter2D(Collider2D other)
	{
		if (other.CompareTag("Player"))
		{
			Enter();
		}
	}

	private void OnTriggerExit2D(Collider2D other)
	{
		if (other.CompareTag("Player"))
		{
			Exit();
		}
	}

	private void Enter()
	{
		if (!isInside)
		{
			isInside = true;
			base.enabled = true;
		}
	}

	private void Exit()
	{
		if (isInside)
		{
			isInside = false;
			base.enabled = false;
		}
	}

	private void PlayRainVibration()
	{
		float num = (useStrengthMultiplier ? strengthMultiplier.GetRandomValue() : 1f);
		RainVibrationPool.PooledRain pooledRain = GetRainVibrationPlayer()?.PlayVibration(num);
		if (pooledRain != null)
		{
			activeEmissions.AddLast(pooledRain);
		}
	}

	private RainVibrationPlayer GetRainVibrationPlayer()
	{
		if (rainVibrations.Count == 0)
		{
			return null;
		}
		return rainVibrations[UnityEngine.Random.Range(0, rainVibrations.Count)];
	}

	public string GetNameString()
	{
		return string.Format("{0} : {1}", this, isInside ? "Active" : "Inactive");
	}
}
