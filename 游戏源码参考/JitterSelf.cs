using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class JitterSelf : MonoBehaviour, IUpdateBatchableUpdate
{
	[SerializeField]
	private JitterSelfProfile profile;

	[SerializeField]
	[ModifiableProperty]
	[Conditional("profile", false, false, false)]
	protected JitterSelfConfig config;

	[HideInInspector]
	[SerializeField]
	[Obsolete]
	private float frequency;

	[HideInInspector]
	[SerializeField]
	[Obsolete]
	private Vector3 amount;

	[HideInInspector]
	[SerializeField]
	[Obsolete]
	private bool useCameraRenderHooks;

	[SerializeField]
	protected bool startInactive;

	[SerializeField]
	private bool isRealtime;

	[SerializeField]
	private CameraRenderHooks.CameraSource hookCamera = CameraRenderHooks.CameraSource.MainCamera;

	[Space]
	[SerializeField]
	private Transform overrideTransform;

	[Space]
	public UnityEvent OnJitterStart;

	public UnityEvent OnJitterEnd;

	private bool isChangeQueued;

	private bool queueActiveState;

	private double queueEndTime;

	private bool isActive;

	private double nextJitterTime;

	private Vector3 initialPosition;

	private Vector3 targetPosition;

	private Vector3 preCullPosition;

	private float amountLerp;

	private float multiplier;

	private bool stoppingWithDecay;

	private Coroutine decayRoutine;

	private UpdateBatcher updateBatcher;

	private Renderer renderer;

	private bool hasRenderer;

	private bool isVisible;

	private Renderer[] childRenderers;

	private bool hasProfile;

	private Transform Transform
	{
		get
		{
			if (!overrideTransform)
			{
				return base.transform;
			}
			return overrideTransform;
		}
	}

	public bool ShouldUpdate
	{
		get
		{
			if (isChangeQueued)
			{
				return true;
			}
			if (!isActive)
			{
				return false;
			}
			JitterSelfConfig jitterSelfConfig = Config;
			if (jitterSelfConfig.AmountMin.magnitude <= Mathf.Epsilon && jitterSelfConfig.AmountMax.magnitude <= Mathf.Epsilon)
			{
				return false;
			}
			if (hasRenderer && renderer.enabled)
			{
				return isVisible;
			}
			if (childRenderers.Length != 0)
			{
				Renderer[] array = childRenderers;
				for (int i = 0; i < array.Length; i++)
				{
					if (array[i].isVisible)
					{
						return true;
					}
				}
			}
			return false;
		}
	}

	public float Multiplier
	{
		get
		{
			return multiplier;
		}
		set
		{
			multiplier = value;
		}
	}

	private JitterSelfConfig Config
	{
		get
		{
			if (hasProfile)
			{
				return profile.Config;
			}
			return config;
		}
	}

	public event Action PositionRestored;

	private void OnValidate()
	{
		if (frequency > 0f)
		{
			config = new JitterSelfConfig
			{
				Frequency = frequency,
				AmountMin = amount,
				AmountMax = amount,
				UseCameraRenderHooks = useCameraRenderHooks
			};
			frequency = 0f;
		}
		hasProfile = profile;
	}

	private void Reset()
	{
		config = new JitterSelfConfig
		{
			Frequency = 30f
		};
	}

	private void Awake()
	{
		OnValidate();
		hasProfile = profile;
	}

	private void Start()
	{
		if (!isActive)
		{
			isActive = !startInactive;
			multiplier = 1f;
		}
		if (Config.UseCameraRenderHooks)
		{
			CameraRenderHooks.CameraPreCull += OnCameraPreCull;
			CameraRenderHooks.CameraPostRender += OnCameraPostRender;
		}
	}

	private void OnEnable()
	{
		ResetAmountLerp();
		SetInitPos();
		renderer = GetComponent<Renderer>();
		hasRenderer = renderer;
		if (hasRenderer)
		{
			isVisible = renderer.isVisible;
		}
		Transform transform = Transform;
		childRenderers = transform.GetComponentsInChildren<Renderer>(includeInactive: true);
		updateBatcher = GameManager.instance.GetComponent<UpdateBatcher>();
		updateBatcher.Add(this);
	}

	private void OnDisable()
	{
		updateBatcher.Remove(this);
		updateBatcher = null;
		if (startInactive)
		{
			isActive = false;
		}
	}

	private void OnBecameVisible()
	{
		isVisible = true;
	}

	private void OnBecameInvisible()
	{
		isVisible = false;
	}

	private void OnDestroy()
	{
		if (Config.UseCameraRenderHooks)
		{
			CameraRenderHooks.CameraPreCull -= OnCameraPreCull;
			CameraRenderHooks.CameraPostRender -= OnCameraPostRender;
		}
	}

	public void BatchedUpdate()
	{
		if (isChangeQueued && Time.timeAsDouble >= queueEndTime)
		{
			isChangeQueued = false;
			if (queueActiveState)
			{
				InternalStartJitter();
			}
			else
			{
				InternalStopJitter(stoppingWithDecay);
			}
		}
		if (!isActive)
		{
			return;
		}
		JitterSelfConfig jitterSelfConfig = Config;
		double num = (isRealtime ? Time.unscaledTimeAsDouble : Time.timeAsDouble);
		if (!(num < nextJitterTime))
		{
			float num2 = ((jitterSelfConfig.Frequency > 0f) ? jitterSelfConfig.Frequency : 60f);
			nextJitterTime = num + (double)(1f / num2);
			Vector3 original = Vector3.Lerp(jitterSelfConfig.AmountMin, jitterSelfConfig.AmountMax, amountLerp);
			targetPosition = initialPosition + original.RandomInRange() * multiplier;
			if (!jitterSelfConfig.UseCameraRenderHooks)
			{
				Transform.localPosition = targetPosition;
			}
		}
	}

	private void OnCameraPreCull(CameraRenderHooks.CameraSource cameraType)
	{
		if (isActive && cameraType == hookCamera && base.isActiveAndEnabled && (hookCamera != CameraRenderHooks.CameraSource.MainCamera || !(Time.timeScale <= Mathf.Epsilon)))
		{
			preCullPosition = Transform.localPosition;
			Transform.localPosition = targetPosition;
		}
	}

	private void OnCameraPostRender(CameraRenderHooks.CameraSource cameraType)
	{
		if (isActive && cameraType == hookCamera && base.isActiveAndEnabled && (hookCamera != CameraRenderHooks.CameraSource.MainCamera || !(Time.timeScale <= Mathf.Epsilon)))
		{
			Transform.localPosition = preCullPosition;
			initialPosition = preCullPosition;
			if (this.PositionRestored != null)
			{
				this.PositionRestored();
			}
		}
	}

	private void ResetAmountLerp()
	{
		amountLerp = UnityEngine.Random.Range(0f, 1f);
	}

	private void SetInitPos()
	{
		initialPosition = Transform.localPosition;
		targetPosition = initialPosition;
	}

	public void StartJitter()
	{
		float randomValue = Config.Delay.GetRandomValue();
		if (randomValue > 0f)
		{
			isChangeQueued = true;
			queueActiveState = true;
			queueEndTime = Time.timeAsDouble + (double)randomValue;
		}
		else
		{
			isChangeQueued = false;
			InternalStartJitter();
		}
	}

	private void InternalStartJitter()
	{
		if (isActive)
		{
			InternalStopJitter(withDecay: false);
		}
		isActive = true;
		multiplier = 1f;
		ResetAmountLerp();
		SetInitPos();
		if (OnJitterStart != null)
		{
			OnJitterStart.Invoke();
		}
	}

	public void StopJitter()
	{
		StopJitterShared(withDecay: false);
	}

	public void StopJitterWithDecay()
	{
		StopJitterShared(withDecay: true);
	}

	protected virtual void OnStopJitter()
	{
	}

	private void StopJitterShared(bool withDecay)
	{
		if (isActive)
		{
			stoppingWithDecay = withDecay;
			float randomValue = Config.Delay.GetRandomValue();
			if (randomValue > 0f)
			{
				isChangeQueued = true;
				queueActiveState = false;
				queueEndTime = Time.timeAsDouble + (double)randomValue;
			}
			else
			{
				isChangeQueued = false;
				InternalStopJitter(withDecay);
			}
		}
	}

	protected void InternalStopJitter(bool withDecay)
	{
		if (withDecay)
		{
			if (decayRoutine == null)
			{
				decayRoutine = StartCoroutine(JitterDecayThenStop());
			}
		}
		else
		{
			StopJitterCompletely();
		}
	}

	private void StopJitterCompletely()
	{
		OnStopJitter();
		isActive = false;
		if (!config.UseCameraRenderHooks)
		{
			Transform.localPosition = initialPosition;
		}
		if (OnJitterEnd != null)
		{
			OnJitterEnd.Invoke();
		}
	}

	private IEnumerator JitterDecayThenStop()
	{
		for (float elapsed = 0f; elapsed < 0.5f; elapsed += Time.deltaTime)
		{
			multiplier = 1f - elapsed / 0.5f;
			yield return null;
		}
		multiplier = 0f;
		decayRoutine = null;
		StopJitterCompletely();
	}

	public static JitterSelf Add(GameObject gameObject, JitterSelfConfig config, CameraRenderHooks.CameraSource hookCamera)
	{
		JitterSelf jitterSelf = gameObject.AddComponent<JitterSelf>();
		jitterSelf.startInactive = true;
		jitterSelf.config = config;
		jitterSelf.hookCamera = hookCamera;
		return jitterSelf;
	}
}
