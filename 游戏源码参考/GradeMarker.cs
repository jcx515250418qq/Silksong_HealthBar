using System;
using System.Collections.Generic;
using GlobalEnums;
using TeamCherry;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

[Serializable]
public class GradeMarker : MonoBehaviour
{
	public bool enableGrade = true;

	private bool activating;

	private bool deactivating;

	[SerializeField]
	[Range(0f, 1f)]
	private float alpha = 1f;

	[SerializeField]
	private OverrideMapZone useMapZone;

	[Header("Range")]
	public float maxIntensityRadius;

	public float cutoffRadius;

	public int priority;

	[Header("Target Color Grade")]
	[Range(0f, 5f)]
	[ModifiableProperty]
	[Conditional("IsUsingMapZone", false, true, false)]
	public float saturation;

	[ModifiableProperty]
	[Conditional("IsUsingMapZone", false, true, false)]
	public AnimationCurve redChannel;

	[ModifiableProperty]
	[Conditional("IsUsingMapZone", false, true, false)]
	public AnimationCurve greenChannel;

	[ModifiableProperty]
	[Conditional("IsUsingMapZone", false, true, false)]
	public AnimationCurve blueChannel;

	[Header("Target Scene Lighting")]
	[Range(0f, 1f)]
	[ModifiableProperty]
	[Conditional("IsUsingMapZone", false, true, false)]
	public float ambientIntensity;

	[ModifiableProperty]
	[Conditional("IsUsingMapZone", false, true, false)]
	public Color ambientColor;

	[Header("Target Hero Light")]
	[ModifiableProperty]
	[Conditional("IsUsingMapZone", false, true, false)]
	public Color heroLightColor;

	private GameManager gm;

	private GameCameras gc;

	private HeroController hero;

	private SceneColorManager scm;

	private Vector2 heading;

	private float sqrNear;

	private float sqrFar;

	private float sqrEffectRange;

	private float t;

	private float u;

	private float origMaxIntensityRadius;

	private float origCutoffRadius;

	private float startMaxIntensityRadius;

	private float startCutoffRadius;

	private float finalMaxIntensityRadius;

	private float finalCutoffRadius;

	private float shrunkPercentage = 30f;

	[NonSerialized]
	public float EaseDuration = 1.5f;

	private float easeTimer = 2f;

	private static readonly List<GradeMarker> _activeMarkers = new List<GradeMarker>();

	private static GradeMarker _previousClosest;

	public float Alpha
	{
		get
		{
			return alpha;
		}
		set
		{
			if (!(Math.Abs(alpha - value) <= Mathf.Epsilon))
			{
				alpha = value;
				if ((bool)scm)
				{
					scm.UpdateScript();
				}
			}
		}
	}

	private bool IsUsingMapZone()
	{
		if (useMapZone.IsEnabled)
		{
			return useMapZone.Value != MapZone.NONE;
		}
		return false;
	}

	protected void OnEnable()
	{
		_activeMarkers.Add(this);
		gm = GameManager.instance;
		if (gm != null)
		{
			gm.NextSceneWillActivate += OnUnloadingLevel;
		}
	}

	protected void OnDisable()
	{
		_activeMarkers.Remove(this);
		if (_previousClosest == this)
		{
			_previousClosest = null;
		}
		if (gm != null)
		{
			gm.NextSceneWillActivate -= OnUnloadingLevel;
		}
	}

	private void Start()
	{
		gc = GameCameras.instance;
		scm = gc.sceneColorManager;
		hero = HeroController.instance;
		if (IsUsingMapZone())
		{
			PlayerData instance = PlayerData.instance;
			AsyncOperationHandle<References> handle = Addressables.LoadAssetAsync<References>("ReferencesData");
			References references = handle.WaitForCompletion();
			if ((bool)references && (bool)references.sceneDefaultSettings)
			{
				CopyLightingFromMapZone(references.sceneDefaultSettings.GetMapZoneSettingsRuntime(useMapZone.Value, instance.blackThreadWorld ? SceneManagerSettings.Conditions.BlackThread : SceneManagerSettings.Conditions.None));
			}
			else
			{
				Debug.LogError("Couldn't load SceneDefaultSettings", this);
			}
			Addressables.Release(handle);
		}
		if (enableGrade)
		{
			Activate();
		}
	}

	private void OnUnloadingLevel()
	{
		Deactivate();
		base.enabled = false;
	}

	public void SetStartSizeForTrigger()
	{
		origCutoffRadius = cutoffRadius;
		origMaxIntensityRadius = maxIntensityRadius;
		cutoffRadius = origCutoffRadius * (shrunkPercentage / 100f);
		maxIntensityRadius = origMaxIntensityRadius * (shrunkPercentage / 100f);
		startCutoffRadius = cutoffRadius;
		startMaxIntensityRadius = maxIntensityRadius;
	}

	public void Activate()
	{
		heading = hero.transform.position - base.transform.position;
		sqrNear = maxIntensityRadius * maxIntensityRadius;
		sqrFar = cutoffRadius * cutoffRadius;
		sqrEffectRange = sqrFar - sqrNear;
		u = (heading.sqrMagnitude - sqrNear) / sqrEffectRange;
		t = Mathf.Clamp01(1f - u);
		if (_previousClosest == this)
		{
			SetScmParams();
		}
		enableGrade = true;
		scm.SetMarkerActive(active: true);
	}

	private void SetScmParams()
	{
		scm.SaturationB = CustomSceneManager.AdjustSaturationForPlatform(saturation, null);
		scm.RedB = redChannel;
		scm.GreenB = greenChannel;
		scm.BlueB = blueChannel;
		scm.AmbientColorB = ambientColor;
		scm.AmbientIntensityB = ambientIntensity;
		if (GameManager.instance.IsGameplayScene())
		{
			scm.HeroLightColorB = heroLightColor;
		}
	}

	public void Deactivate()
	{
		enableGrade = false;
		if ((bool)scm)
		{
			scm.SetMarkerActive(active: false);
			scm.SetFactor(0f);
			scm.UpdateScript();
		}
	}

	public void ActivateGradual()
	{
		startCutoffRadius = cutoffRadius;
		startMaxIntensityRadius = maxIntensityRadius;
		finalCutoffRadius = origCutoffRadius;
		finalMaxIntensityRadius = origMaxIntensityRadius;
		cutoffRadius = startCutoffRadius;
		maxIntensityRadius = startMaxIntensityRadius;
		Activate();
		activating = true;
		deactivating = false;
		easeTimer = 0f;
	}

	public void DeactivateGradual()
	{
		startCutoffRadius = cutoffRadius;
		startMaxIntensityRadius = maxIntensityRadius;
		finalCutoffRadius = cutoffRadius * (shrunkPercentage / 100f);
		finalMaxIntensityRadius = maxIntensityRadius * (shrunkPercentage / 100f);
		activating = false;
		deactivating = true;
		easeTimer = 0f;
	}

	private void Update()
	{
		if (!enableGrade)
		{
			return;
		}
		if (Time.frameCount % 2 == 0)
		{
			Vector2 a = hero.transform.position;
			float num = float.MaxValue;
			GradeMarker gradeMarker = null;
			foreach (GradeMarker activeMarker in _activeMarkers)
			{
				Vector2 b = activeMarker.transform.position;
				float num2 = Vector2.Distance(a, b) - activeMarker.cutoffRadius;
				if (gradeMarker != null)
				{
					if (activeMarker.priority < gradeMarker.priority && (!(num > 0f) || !(num2 <= 0f)))
					{
						continue;
					}
					if (num2 <= 0f && activeMarker.priority > gradeMarker.priority)
					{
						num = float.MaxValue;
						gradeMarker = null;
					}
				}
				if (!(num2 > num))
				{
					num = num2;
					gradeMarker = activeMarker;
				}
			}
			if (gradeMarker == this)
			{
				if (_previousClosest != this)
				{
					_previousClosest = this;
					SetScmParams();
					scm.UpdateScript(forceUpdate: true);
				}
				UpdateScm();
			}
		}
		if (!(easeTimer < EaseDuration))
		{
			return;
		}
		easeTimer += Time.deltaTime;
		float num3 = easeTimer / EaseDuration;
		maxIntensityRadius = Mathf.Lerp(startMaxIntensityRadius, finalMaxIntensityRadius, num3);
		cutoffRadius = Mathf.Lerp(startCutoffRadius, finalCutoffRadius, num3);
		if (activating)
		{
			if (easeTimer >= EaseDuration)
			{
				activating = false;
			}
		}
		else if (deactivating && easeTimer >= EaseDuration)
		{
			deactivating = false;
			enableGrade = false;
		}
		if (easeTimer > EaseDuration)
		{
			easeTimer = EaseDuration;
		}
	}

	public void UpdateScm()
	{
		heading = hero.transform.position - base.transform.position;
		sqrNear = maxIntensityRadius * maxIntensityRadius;
		sqrFar = cutoffRadius * cutoffRadius;
		sqrEffectRange = sqrFar - sqrNear;
		u = (heading.sqrMagnitude - sqrNear) / sqrEffectRange;
		t = Mathf.Clamp01(1f - u);
		if (scm.StartBufferActive)
		{
			scm.SetMarkerActive(active: true);
			SetFactor(t);
			return;
		}
		bool markerActive = scm.MarkerActive;
		if (u < 0f)
		{
			scm.SetMarkerActive(active: false);
			SetFactor(1f);
		}
		else if (u < 1.1f)
		{
			scm.SetMarkerActive(active: true);
			SetFactor(t);
		}
		else
		{
			scm.SetMarkerActive(active: false);
			SetFactor(0f);
		}
		if (markerActive != scm.MarkerActive)
		{
			scm.UpdateScript();
		}
	}

	private void SetFactor(float newT)
	{
		scm.SetFactor(newT * alpha);
	}

	public void CopyLightingFromMapZone(SceneManagerSettings storedSettings)
	{
		saturation = storedSettings.saturation;
		redChannel = new AnimationCurve(storedSettings.redChannel.keys);
		greenChannel = new AnimationCurve(storedSettings.greenChannel.keys);
		blueChannel = new AnimationCurve(storedSettings.blueChannel.keys);
		ambientColor = storedSettings.defaultColor;
		ambientIntensity = storedSettings.defaultIntensity;
		heroLightColor = storedSettings.heroLightColor;
	}
}
