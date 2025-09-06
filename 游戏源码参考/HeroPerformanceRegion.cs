using System;
using System.Collections;
using GlobalSettings;
using UnityEngine;

public class HeroPerformanceRegion : MonoBehaviour
{
	public enum AffectedState
	{
		None = 0,
		ActiveInner = 1,
		ActiveOuter = 2
	}

	private const float AMPLIFY_FADE_UP_TIME = 2f;

	private const float AMPLIFY_FADE_DOWN_TIME = 1.2f;

	private const float AMPLIFY_VOLUME = 0.4f;

	[SerializeField]
	private Vector2 centreOffset;

	[SerializeField]
	private Vector2 innerSize;

	[SerializeField]
	private Vector2 outerSize;

	[SerializeField]
	private Animator amplifyEffect;

	[SerializeField]
	private AudioSource amplifyAudioSource;

	[SerializeField]
	private AudioSource needolinAudioSource;

	private bool amplifyEffectIsB;

	private Animator amplifyEffectB;

	private Coroutine amplifyAudioFadeRoutine;

	private Coroutine endAmplifyEffectRoutineA;

	private Coroutine endAmplifyEffectRoutineB;

	private static readonly int _disappearAnimId = Animator.StringToHash("Disappear");

	private static HeroPerformanceRegion _instance;

	private bool isPerforming;

	public static bool IsPerforming
	{
		get
		{
			if ((bool)_instance)
			{
				return _instance.isPerforming;
			}
			return false;
		}
		set
		{
			if ((bool)_instance)
			{
				_instance.SetIsPerforming(value);
			}
		}
	}

	public static event Action StartedPerforming;

	public static event Action StoppedPerforming;

	private void OnDrawGizmosSelected()
	{
		Vector2 vector = (Vector2)base.transform.position + centreOffset;
		Gizmos.color = Color.green;
		Gizmos.DrawWireCube(vector, innerSize);
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireCube(vector, outerSize);
	}

	private void Awake()
	{
		amplifyEffect.gameObject.SetActive(value: false);
		amplifyEffectB = UnityEngine.Object.Instantiate(amplifyEffect, amplifyEffect.transform.parent);
		amplifyAudioSource.loop = true;
		amplifyAudioSource.playOnAwake = false;
		amplifyAudioSource.Stop();
	}

	private void Start()
	{
		if (_instance == null)
		{
			_instance = this;
		}
	}

	private void OnDestroy()
	{
		if (!(_instance != this))
		{
			_instance = null;
			HeroPerformanceRegion.StartedPerforming = null;
			HeroPerformanceRegion.StoppedPerforming = null;
		}
	}

	private void SetIsPerforming(bool value)
	{
		if (_instance.isPerforming == value)
		{
			return;
		}
		_instance.isPerforming = value;
		if (amplifyAudioFadeRoutine != null)
		{
			StopCoroutine(amplifyAudioFadeRoutine);
			amplifyAudioFadeRoutine = null;
		}
		if (_instance.isPerforming)
		{
			HeroPerformanceRegion.StartedPerforming?.Invoke();
			if (!OverrideNeedolinLoop.IsOverridden && Gameplay.MusicianCharmTool.IsEquipped)
			{
				GameObject gameObject;
				Coroutine coroutine;
				if (amplifyEffectIsB)
				{
					amplifyEffectIsB = false;
					gameObject = amplifyEffect.gameObject;
					coroutine = endAmplifyEffectRoutineA;
					endAmplifyEffectRoutineA = null;
				}
				else
				{
					amplifyEffectIsB = true;
					gameObject = amplifyEffectB.gameObject;
					coroutine = endAmplifyEffectRoutineB;
					endAmplifyEffectRoutineB = null;
				}
				if (coroutine != null)
				{
					StopCoroutine(coroutine);
				}
				gameObject.SetActive(value: false);
				gameObject.SetActive(value: true);
				GameCameras.instance.forceCameraAspect.SetFovOffset(Gameplay.MusicianCharmFovOffset, Gameplay.MusicianCharmFovStartDuration, Gameplay.MusicianCharmFovStartCurve);
				amplifyAudioSource.volume = 0f;
				amplifyAudioSource.Play();
				if (needolinAudioSource.isPlaying)
				{
					amplifyAudioSource.timeSamples = needolinAudioSource.timeSamples;
				}
				else
				{
					Debug.LogError("Needolin audio is not already playing, so amplify can't sync!", this);
				}
				amplifyAudioFadeRoutine = this.StartTimerRoutine(0f, 2f, delegate(float t)
				{
					amplifyAudioSource.volume = t * 0.4f;
				});
			}
			return;
		}
		HeroPerformanceRegion.StoppedPerforming?.Invoke();
		if (amplifyEffectIsB)
		{
			if (endAmplifyEffectRoutineB == null && amplifyEffectB.gameObject.activeSelf)
			{
				endAmplifyEffectRoutineB = StartCoroutine(EndAmplifyEffect());
			}
		}
		else if (endAmplifyEffectRoutineA == null && amplifyEffect.gameObject.activeSelf)
		{
			endAmplifyEffectRoutineA = StartCoroutine(EndAmplifyEffect());
		}
		GameCameras.instance.forceCameraAspect.SetFovOffset(0f, Gameplay.MusicianCharmFovEndDuration, Gameplay.MusicianCharmFovEndCurve);
		amplifyAudioFadeRoutine = this.StartTimerRoutine(0f, 1.2f, delegate(float t)
		{
			amplifyAudioSource.volume = Mathf.Clamp01(1f - t) * 0.4f;
		}, null, delegate
		{
			amplifyAudioSource.Stop();
		});
	}

	private IEnumerator EndAmplifyEffect()
	{
		bool wasAmplifyEffectB = amplifyEffectIsB;
		Animator effect = (amplifyEffectIsB ? amplifyEffectB : amplifyEffect);
		effect.Play(_disappearAnimId);
		yield return null;
		yield return new WaitForSeconds(effect.GetCurrentAnimatorStateInfo(0).length);
		effect.gameObject.SetActive(value: false);
		if (wasAmplifyEffectB)
		{
			endAmplifyEffectRoutineB = null;
		}
		else
		{
			endAmplifyEffectRoutineA = null;
		}
	}

	public static AffectedState GetAffectedState(Transform transform, bool ignoreRange)
	{
		if (!IsPerforming)
		{
			return AffectedState.None;
		}
		return _instance.InternalGetAffectedRange(transform, ignoreRange);
	}

	public static AffectedState GetAffectedStateWithRadius(Transform transform, float radius)
	{
		if (!IsPerforming)
		{
			return AffectedState.None;
		}
		return _instance.InternalGetAffectedRangeWithRadius(transform, radius);
	}

	public static bool IsPlayingInRange(Vector2 position, float radius)
	{
		if (!IsPerforming)
		{
			return false;
		}
		return _instance.InternalIsInRange(position, radius);
	}

	private AffectedState InternalGetAffectedRange(Transform otherTransform, bool ignoreRange)
	{
		if (ignoreRange)
		{
			return AffectedState.ActiveInner;
		}
		Vector2 centre = (Vector2)base.transform.position + centreOffset;
		Vector3 position = otherTransform.position;
		if (IsInRange(position, centre, innerSize))
		{
			return AffectedState.ActiveInner;
		}
		if (IsInRange(position, centre, outerSize))
		{
			return AffectedState.ActiveOuter;
		}
		return AffectedState.None;
	}

	private AffectedState InternalGetAffectedRangeWithRadius(Transform otherTransform, float radius)
	{
		Vector2 vector = (Vector2)base.transform.position + centreOffset;
		Vector2 posInRadius = GetPosInRadius(vector, otherTransform.position, radius);
		if (IsInRange(posInRadius, vector, innerSize))
		{
			return AffectedState.ActiveInner;
		}
		if (IsInRange(posInRadius, vector, outerSize))
		{
			return AffectedState.ActiveOuter;
		}
		return AffectedState.None;
	}

	private bool InternalIsInRange(Vector2 position, float radius)
	{
		return Vector2.Distance((Vector2)base.transform.position + centreOffset, position) <= radius;
	}

	private static bool IsInRange(Vector2 pos, Vector2 centre, Vector2 size)
	{
		float num = (Gameplay.MusicianCharmTool.IsEquipped ? Gameplay.MusicianCharmNeedolinRangeMult : 1f);
		Vector2 vector = size * (num * 0.5f);
		Vector2 vector2 = centre - vector;
		Vector2 vector3 = centre + vector;
		if (pos.x >= vector2.x && pos.x <= vector3.x && pos.y >= vector2.y)
		{
			return pos.y <= vector3.y;
		}
		return false;
	}

	public static Vector2 GetPosInRadius(Vector2 noiseSourcePos, Vector2 otherPos, float radius)
	{
		Vector2 vector = noiseSourcePos - otherPos;
		Vector2 vector2 = Mathf.Clamp(vector.magnitude, 0f, radius) * vector.normalized;
		return otherPos + vector2;
	}
}
