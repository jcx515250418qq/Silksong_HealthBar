using System.Collections;
using System.Collections.Generic;
using TeamCherry.NestedFadeGroup;
using UnityEngine;

public class SilkSnare : MonoBehaviour
{
	private static readonly int _appearAnim = Animator.StringToHash("Appear");

	private static readonly int _disappearAnim = Animator.StringToHash("Disappear");

	private static readonly int _blastAnim = Animator.StringToHash("Blast");

	[SerializeField]
	private Animator animator;

	[SerializeField]
	private TriggerEnterEvent groundTrigger;

	[SerializeField]
	private float blastRecycleTimer;

	[SerializeField]
	private float disappearRecycleTimer;

	[Header("Fade Settings")]
	[SerializeField]
	private NestedFadeGroup groundFadeGroup;

	[SerializeField]
	private NestedFadeGroup distantGroundFadeGroup;

	[Space]
	[SerializeField]
	private float fullEffectRadius = 5f;

	[SerializeField]
	private float falloffRadius = 10f;

	private float fullEffectRadiusSqr;

	private float falloffRadiusSqr;

	private double nextUpdate;

	private bool foundHero;

	private HeroController hc;

	private Coroutine appearRoutine;

	private Coroutine endRoutine;

	private ParticleSystem[] particleSystems;

	private static List<SilkSnare> _activeSnares = new List<SilkSnare>();

	private bool canUpdateFade;

	private bool insideRange;

	private void Awake()
	{
		particleSystems = GetComponentsInChildren<ParticleSystem>();
		groundTrigger.OnTriggerEntered += OnGroundTriggerEntered;
		EventRegister.GetRegisterGuaranteed(base.gameObject, "BENCHREST").ReceivedEvent += End;
		CalculateSqureValues();
		canUpdateFade = groundFadeGroup != null && distantGroundFadeGroup != null;
	}

	private void OnValidate()
	{
		CalculateSqureValues();
	}

	private void OnEnable()
	{
		_activeSnares.Add(this);
		appearRoutine = StartCoroutine(Appear());
		hc = HeroController.instance;
		foundHero = hc != null;
		UpdateFade();
	}

	private void OnDisable()
	{
		if (appearRoutine != null)
		{
			StopCoroutine(appearRoutine);
			appearRoutine = null;
		}
		_activeSnares.Remove(this);
		if (endRoutine != null)
		{
			StopCoroutine(endRoutine);
			endRoutine = null;
		}
	}

	private void Update()
	{
		if (Time.timeAsDouble > nextUpdate)
		{
			UpdateFade();
		}
	}

	private void OnGroundTriggerEntered(Collider2D col, GameObject sender)
	{
		if (endRoutine == null && (bool)col.GetComponentInParent<HealthManager>())
		{
			endRoutine = StartCoroutine(Blast());
		}
	}

	private bool IsAnyParticles()
	{
		ParticleSystem[] array = particleSystems;
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i].IsAlive())
			{
				return true;
			}
		}
		return false;
	}

	private void StopAllParticles()
	{
		ParticleSystem[] array = particleSystems;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Stop(withChildren: false, ParticleSystemStopBehavior.StopEmitting);
		}
	}

	private IEnumerator Blast()
	{
		StopAllParticles();
		animator.Play(_blastAnim);
		yield return new WaitForSeconds(blastRecycleTimer);
		while (IsAnyParticles())
		{
			yield return null;
		}
		base.gameObject.Recycle();
		endRoutine = null;
	}

	public void End()
	{
		if (base.gameObject.activeInHierarchy && endRoutine == null)
		{
			endRoutine = StartCoroutine(EndRoutine());
		}
	}

	private IEnumerator Appear()
	{
		animator.Play(_appearAnim, 0, 0f);
		yield return null;
		Vector2 b = base.transform.position;
		foreach (SilkSnare activeSnare in _activeSnares)
		{
			if (!(activeSnare == this) && !(Vector2.Distance(activeSnare.transform.position, b) >= 2f))
			{
				activeSnare.End();
			}
		}
	}

	private IEnumerator EndRoutine()
	{
		StopAllParticles();
		animator.Play(_disappearAnim);
		yield return new WaitForSeconds(disappearRecycleTimer);
		while (IsAnyParticles())
		{
			yield return null;
		}
		base.gameObject.Recycle();
		endRoutine = null;
	}

	private void CalculateSqureValues()
	{
		fullEffectRadiusSqr = fullEffectRadius * fullEffectRadius;
		falloffRadiusSqr = falloffRadius * falloffRadius;
	}

	public float CalculateFalloff(Vector2 targetPosition, Vector2 originPosition)
	{
		insideRange = false;
		float sqrMagnitude = (targetPosition - originPosition).sqrMagnitude;
		if (sqrMagnitude >= falloffRadiusSqr)
		{
			return 0f;
		}
		insideRange = true;
		if (sqrMagnitude <= fullEffectRadiusSqr)
		{
			return 1f;
		}
		float num = sqrMagnitude - fullEffectRadiusSqr;
		float num2 = falloffRadiusSqr - fullEffectRadiusSqr;
		return 1f - num / num2;
	}

	private void UpdateFade()
	{
		if (!canUpdateFade)
		{
			return;
		}
		if (!foundHero)
		{
			nextUpdate = Time.timeAsDouble + (double)(blastRecycleTimer * 2f);
			return;
		}
		float num = CalculateFalloff(hc.transform.position, base.transform.position);
		groundFadeGroup.AlphaSelf = num;
		distantGroundFadeGroup.AlphaSelf = 1f - num;
		if (!insideRange)
		{
			nextUpdate = Time.timeAsDouble + 0.10000000149011612;
		}
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.green;
		Gizmos.DrawWireSphere(base.transform.position, fullEffectRadius);
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(base.transform.position, falloffRadius);
	}
}
