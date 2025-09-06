using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TeamCherry.SharedUtils;
using UnityEngine;

public class BlackThreadStrand : MonoBehaviour, IInitialisable
{
	private class ProbabilitySpine : Probability.ProbabilityBase<BlackThreadSpine>
	{
		public override BlackThreadSpine Item { get; }

		public ProbabilitySpine(BlackThreadSpine spine)
		{
			Item = spine;
		}
	}

	private struct AudioLoopInfo
	{
		public BlackThreadStrand Strand;

		public Vector3 LoopPosition;

		public float Distance;
	}

	private static readonly int _idleAnim = Animator.StringToHash("Idle");

	private static readonly int _alertAnim = Animator.StringToHash("Alert");

	private static readonly int _rageStartAnim = Animator.StringToHash("Rage Start");

	private static readonly int _rageAnim = Animator.StringToHash("Rage");

	private static readonly int _rageEndAnim = Animator.StringToHash("Rage End");

	[SerializeField]
	private GameObject strandObject;

	[SerializeField]
	private Animator animator;

	[SerializeField]
	private TrackTriggerObjects rangeTrigger;

	[SerializeField]
	private float heroRadius;

	[SerializeField]
	private BlackThreadSpine[] spinePrefabs;

	[SerializeField]
	private bool useProbability;

	[SerializeField]
	private bool setSpineScaleRelative;

	[SerializeField]
	private MinMaxFloat spineSpawnDelay;

	[SerializeField]
	private MinMaxFloat spineSpawnDelayInRange;

	[SerializeField]
	private MinMaxFloat spineSpawnDelayNeedolin;

	[SerializeField]
	private Vector3 spineSpawnMin;

	[SerializeField]
	private Vector3 spineSpawnMax;

	[SerializeField]
	private ParticleSystem[] particles;

	[SerializeField]
	private float emissionMultInRange;

	[SerializeField]
	private float emissionMultNeedolin;

	[Space]
	[SerializeField]
	private EventRegister rageStartEvent;

	[SerializeField]
	private EventRegister rageEndEvent;

	[SerializeField]
	private Transform rageChild;

	[Space]
	[SerializeField]
	private RandomAudioClipTable loopAudioTable;

	[SerializeField]
	private AudioSource loopAudioSource;

	[SerializeField]
	private int activeLoopsCount;

	[SerializeField]
	private float audioFadeDuration;

	private float[] baseEmissionRates;

	private bool isMenuScene;

	private VisibilityGroup visibilityGroup;

	private bool isVisible;

	private bool isInactive;

	private Coroutine behaviourRoutine;

	private bool forceRage;

	private bool skipSpawnDelay;

	private Coroutine rageTimer;

	private ProbabilitySpine[] probabilitySpines;

	private float[] probabilities;

	private bool hasAwaken;

	private bool hasStarted;

	private HeroController hc;

	private static List<BlackThreadStrand> _activeStrands;

	private static BlackThreadStrand _audioManagerStrand;

	private List<AudioLoopInfo> closestStrandsTemp;

	private Coroutine audioManagerRoutine;

	private Coroutine audioFadeRoutine;

	private float audioFadeTarget;

	private bool isFadingOut;

	private bool CanSpawnCreature
	{
		get
		{
			if (isVisible)
			{
				return !isInactive;
			}
			return false;
		}
	}

	GameObject IInitialisable.gameObject => base.gameObject;

	private void OnDrawGizmosSelected()
	{
		Gizmos.DrawWireSphere(base.transform.position, heroRadius);
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.DrawLine(spineSpawnMin, spineSpawnMax);
	}

	public bool OnAwake()
	{
		if (hasAwaken)
		{
			return false;
		}
		hasAwaken = true;
		hc = HeroController.SilentInstance;
		for (int i = 0; i < spinePrefabs.Length; i++)
		{
			PersonalObjectPool.EnsurePooledInScene(base.gameObject, spinePrefabs[i].gameObject, 30, i == spinePrefabs.Length - 1, initialiseSpawned: false, shared: true);
		}
		baseEmissionRates = new float[particles.Length];
		for (int j = 0; j < particles.Length; j++)
		{
			baseEmissionRates[j] = particles[j].emission.rateOverTimeMultiplier;
		}
		RegisterRageEvents();
		if (useProbability)
		{
			probabilitySpines = spinePrefabs.Select((BlackThreadSpine prefab) => new ProbabilitySpine(prefab)).ToArray();
		}
		isMenuScene = GameManager.instance.IsMenuScene();
		if (isMenuScene)
		{
			return true;
		}
		if (strandObject != null)
		{
			visibilityGroup = strandObject.AddComponentIfNotPresent<VisibilityGroup>();
			isVisible = visibilityGroup.IsVisible;
			visibilityGroup.OnVisibilityChanged += OnVisibilityChanged;
		}
		return true;
	}

	private void RegisterRageEvents()
	{
		if ((bool)rageStartEvent)
		{
			rageStartEvent.ReceivedEvent += OnRageStart;
		}
		if ((bool)rageEndEvent)
		{
			rageEndEvent.ReceivedEvent += OnRageEnd;
		}
	}

	private void UnregisterRageEvents()
	{
		if ((bool)rageStartEvent)
		{
			rageStartEvent.ReceivedEvent -= OnRageStart;
		}
		if ((bool)rageEndEvent)
		{
			rageEndEvent.ReceivedEvent -= OnRageEnd;
		}
	}

	private void OnRageStart()
	{
		forceRage = true;
	}

	private void OnRageEnd()
	{
		forceRage = false;
	}

	private void OnVisibilityChanged(bool visible)
	{
		isVisible = visible;
		if (isVisible)
		{
			ParticleSystem[] array = particles;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].gameObject.SetActive(value: true);
			}
			if (behaviourRoutine == null)
			{
				behaviourRoutine = StartCoroutine(BehaviourRoutine());
			}
		}
		else
		{
			ParticleSystem[] array = particles;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].gameObject.SetActive(value: false);
			}
		}
	}

	public bool OnStart()
	{
		OnAwake();
		if (hasStarted)
		{
			return false;
		}
		hasStarted = true;
		PersonalObjectPool.EnsurePooledInSceneFinished(base.gameObject);
		return true;
	}

	private void Awake()
	{
		OnAwake();
	}

	private void OnEnable()
	{
		if ((bool)hc)
		{
			if (_activeStrands == null)
			{
				_activeStrands = new List<BlackThreadStrand>();
			}
			_activeStrands.Add(this);
			if (!_audioManagerStrand)
			{
				_audioManagerStrand = this;
				audioManagerRoutine = StartCoroutine(SharedAudioControl());
			}
		}
		if (!isMenuScene && (bool)animator)
		{
			animator.cullingMode = AnimatorCullingMode.CullCompletely;
		}
		if (!visibilityGroup)
		{
			OnVisibilityChanged(visible: true);
		}
	}

	private void OnDisable()
	{
		_activeStrands?.Remove(this);
		StopLoopAudio(isInstant: true);
		if (this == _audioManagerStrand)
		{
			_audioManagerStrand = null;
			StopCoroutine(audioManagerRoutine);
			audioManagerRoutine = null;
			List<BlackThreadStrand> activeStrands = _activeStrands;
			if (activeStrands != null && activeStrands.Count > 0)
			{
				_audioManagerStrand = _activeStrands[0];
				_audioManagerStrand.closestStrandsTemp = closestStrandsTemp;
				closestStrandsTemp = null;
				_audioManagerStrand.audioManagerRoutine = _audioManagerStrand.StartCoroutine(_audioManagerStrand.SharedAudioControl());
			}
			else
			{
				_activeStrands = null;
			}
		}
		if (!visibilityGroup)
		{
			OnVisibilityChanged(visible: false);
		}
		if (behaviourRoutine != null)
		{
			StopCoroutine(behaviourRoutine);
			behaviourRoutine = null;
		}
	}

	private void OnDestroy()
	{
		UnregisterRageEvents();
	}

	private void PositionRageChild()
	{
		if ((bool)rageChild)
		{
			Vector3 closestPosToHero = GetClosestPosToHero();
			rageChild.SetPosition2D(closestPosToHero);
		}
	}

	private Vector3 GetClosestPosToHero()
	{
		if (!(hc != null))
		{
			return base.transform.position;
		}
		Vector3 position = hc.transform.position;
		Transform obj = base.transform;
		float num = heroRadius * heroRadius;
		Vector3 vector = obj.TransformPoint(spineSpawnMin);
		Vector3 vector2 = obj.TransformPoint(spineSpawnMax) - vector;
		Vector3 lhs = (Vector2)position - (Vector2)vector;
		float sqrMagnitude = vector2.sqrMagnitude;
		float num2 = Mathf.Clamp01(Vector3.Dot(lhs, vector2) / sqrMagnitude);
		float sqrMagnitude2 = ((Vector2)(vector + num2 * vector2) - (Vector2)position).sqrMagnitude;
		float num3 = 0f;
		float num4 = 1f;
		bool flag = false;
		if (sqrMagnitude2 < num)
		{
			float num5 = Mathf.Sqrt(num - sqrMagnitude2);
			float num6 = Mathf.Sqrt(sqrMagnitude);
			float num7 = num5 / num6;
			float b = Mathf.Clamp01(num2 - num7);
			float b2 = Mathf.Clamp01(num2 + num7);
			num3 = Mathf.Max(0f, b);
			num4 = Mathf.Min(1f, b2);
			flag = num3 <= num4;
		}
		Vector3 position2 = Vector3.Lerp(t: flag ? UnityEngine.Random.Range(num3, num4) : UnityEngine.Random.value, a: spineSpawnMin, b: spineSpawnMax);
		return obj.TransformPoint(position2);
	}

	private IEnumerator BehaviourRoutine()
	{
		Transform trans = base.transform;
		float prevEmissionMult = 0f;
		bool wasWaitingForLoop = false;
		bool hasHero = hc != null;
		while (CanSpawnCreature)
		{
			bool isNeedolinPlaying = HeroPerformanceRegion.GetAffectedState(trans, ignoreRange: true) != HeroPerformanceRegion.AffectedState.None;
			bool waitForLoop = false;
			MinMaxFloat delay;
			int anim;
			float emissionMult;
			if (forceRage || isNeedolinPlaying)
			{
				waitForLoop = true;
				delay = spineSpawnDelayNeedolin;
				anim = _rageAnim;
				emissionMult = emissionMultNeedolin;
				PositionRageChild();
				if (!wasWaitingForLoop && (bool)animator)
				{
					animator.Play(_rageStartAnim, 0, 0f);
					yield return null;
					yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
				}
			}
			else if ((bool)rangeTrigger && rangeTrigger.IsInside)
			{
				delay = spineSpawnDelayInRange;
				anim = _alertAnim;
				emissionMult = emissionMultInRange;
			}
			else
			{
				delay = spineSpawnDelay;
				anim = _idleAnim;
				emissionMult = 1f;
			}
			if ((bool)animator)
			{
				if (waitForLoop)
				{
					animator.Play(anim, 0, 0f);
				}
				else
				{
					if (wasWaitingForLoop)
					{
						animator.Play(_rageEndAnim, 0, 0f);
						yield return null;
						yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
					}
					animator.Play(anim);
				}
			}
			if (Math.Abs(emissionMult - prevEmissionMult) > Mathf.Epsilon)
			{
				for (int i = 0; i < particles.Length; i++)
				{
					ParticleSystem.EmissionModule emission = particles[i].emission;
					emission.rateOverTimeMultiplier = emissionMult * baseEmissionRates[i];
				}
			}
			if (waitForLoop && (bool)animator)
			{
				yield return null;
				yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
			}
			wasWaitingForLoop = waitForLoop;
			float randomValue = delay.GetRandomValue();
			if (randomValue <= Mathf.Epsilon)
			{
				yield return null;
				continue;
			}
			yield return new WaitForSecondsInterruptable(randomValue, () => skipSpawnDelay);
			if (forceRage)
			{
				continue;
			}
			if (!CanSpawnCreature)
			{
				break;
			}
			BlackThreadSpine prefab = (useProbability ? Probability.GetRandomItemByProbabilityFair<ProbabilitySpine, BlackThreadSpine>(probabilitySpines, ref probabilities) : spinePrefabs[UnityEngine.Random.Range(0, spinePrefabs.Length)]);
			Vector3 position2;
			if (hasHero)
			{
				Vector2 vector = hc.transform.position;
				float num = heroRadius * heroRadius;
				Vector3 vector2 = trans.TransformPoint(spineSpawnMin);
				Vector3 vector3 = trans.TransformPoint(spineSpawnMax) - vector2;
				Vector3 lhs = vector - (Vector2)vector2;
				float sqrMagnitude = vector3.sqrMagnitude;
				float num2 = Mathf.Clamp01(Vector3.Dot(lhs, vector3) / sqrMagnitude);
				float sqrMagnitude2 = ((Vector2)(vector2 + num2 * vector3) - vector).sqrMagnitude;
				float num3 = 0f;
				float num4 = 1f;
				bool flag = false;
				if (sqrMagnitude2 < num)
				{
					float num5 = Mathf.Sqrt(num - sqrMagnitude2);
					float num6 = Mathf.Sqrt(sqrMagnitude);
					float num7 = num5 / num6;
					float b = Mathf.Clamp01(num2 - num7);
					float b2 = Mathf.Clamp01(num2 + num7);
					num3 = Mathf.Max(0f, b);
					num4 = Mathf.Min(1f, b2);
					flag = num3 <= num4;
				}
				Vector3 position = Vector3.Lerp(t: flag ? UnityEngine.Random.Range(num3, num4) : UnityEngine.Random.value, a: spineSpawnMin, b: spineSpawnMax);
				position2 = trans.TransformPoint(position);
			}
			else
			{
				Vector3 position3 = Vector3.Lerp(spineSpawnMin, spineSpawnMax, UnityEngine.Random.Range(0f, 1f));
				position2 = trans.TransformPoint(position3);
			}
			BlackThreadSpine blackThreadSpine = prefab.Spawn();
			Transform transform = blackThreadSpine.transform;
			transform.position = position2;
			transform.rotation = trans.rotation;
			if (setSpineScaleRelative)
			{
				transform.localScale = trans.lossyScale;
				blackThreadSpine.UpdateInitialScale();
			}
			blackThreadSpine.Spawned(isNeedolinPlaying);
		}
		behaviourRoutine = null;
	}

	public void RageForTime(float time)
	{
		RageForTime(time, skipDelay: true);
	}

	public void RageForTime(float time, bool skipDelay)
	{
		if (rageTimer != null)
		{
			StopCoroutine(rageTimer);
		}
		if (base.isActiveAndEnabled)
		{
			rageTimer = StartCoroutine(RageRoutine(time, skipDelay));
		}
	}

	private IEnumerator RageRoutine(float time, bool skipDelay)
	{
		forceRage = true;
		skipSpawnDelay = skipDelay;
		yield return new WaitForSeconds(time);
		forceRage = false;
		skipSpawnDelay = false;
		rageTimer = null;
	}

	public void Deactivate()
	{
		isInactive = true;
	}

	private IEnumerator SharedAudioControl()
	{
		yield return null;
		if (closestStrandsTemp == null)
		{
			closestStrandsTemp = new List<AudioLoopInfo>(_activeStrands.Count);
		}
		WaitForSeconds wait = new WaitForSeconds(0.5f);
		while (true)
		{
			Vector3 position = hc.transform.position;
			foreach (BlackThreadStrand activeStrand in _activeStrands)
			{
				if ((bool)activeStrand.loopAudioSource)
				{
					Vector3 closestPosToHero = activeStrand.GetClosestPosToHero();
					float distance = Vector3.SqrMagnitude(closestPosToHero - position);
					closestStrandsTemp.Add(new AudioLoopInfo
					{
						Strand = activeStrand,
						LoopPosition = closestPosToHero,
						Distance = distance
					});
				}
			}
			closestStrandsTemp.Sort((AudioLoopInfo a, AudioLoopInfo b) => a.Distance.CompareTo(b.Distance));
			for (int i = 0; i < closestStrandsTemp.Count; i++)
			{
				AudioLoopInfo audioLoopInfo = closestStrandsTemp[i];
				if (i < activeLoopsCount)
				{
					audioLoopInfo.Strand.loopAudioSource.transform.position = audioLoopInfo.LoopPosition;
					audioLoopInfo.Strand.StartLoopAudio();
				}
				else
				{
					audioLoopInfo.Strand.StopLoopAudio(isInstant: false);
				}
			}
			closestStrandsTemp.Clear();
			yield return wait;
		}
	}

	private void StartLoopAudio()
	{
		if ((bool)loopAudioSource && !loopAudioSource.isPlaying && !(loopAudioTable == null))
		{
			loopAudioSource.clip = loopAudioTable.SelectClip();
			loopAudioSource.pitch = loopAudioTable.SelectPitch();
			loopAudioSource.volume = 0f;
			audioFadeTarget = loopAudioTable.SelectVolume();
			if (audioFadeRoutine != null)
			{
				StopCoroutine(audioFadeRoutine);
				audioFadeRoutine = null;
			}
			audioFadeRoutine = this.StartTimerRoutine(0f, audioFadeDuration, delegate(float t)
			{
				loopAudioSource.volume = t * audioFadeTarget;
			});
			loopAudioSource.timeSamples = UnityEngine.Random.Range(0, loopAudioSource.clip.samples);
			loopAudioSource.loop = true;
			loopAudioSource.Play();
			isFadingOut = false;
		}
	}

	private void StopLoopAudio(bool isInstant)
	{
		if (isFadingOut || !loopAudioSource || !loopAudioSource.isPlaying)
		{
			return;
		}
		if (isInstant)
		{
			loopAudioSource.Stop();
			loopAudioSource.clip = null;
			return;
		}
		if (audioFadeRoutine != null)
		{
			StopCoroutine(audioFadeRoutine);
			audioFadeRoutine = null;
		}
		audioFadeTarget = loopAudioSource.volume;
		isFadingOut = true;
		audioFadeRoutine = this.StartTimerRoutine(0f, audioFadeDuration, delegate(float t)
		{
			loopAudioSource.volume = (1f - t) * audioFadeTarget;
		}, null, delegate
		{
			loopAudioSource.Stop();
			isFadingOut = false;
		});
	}
}
