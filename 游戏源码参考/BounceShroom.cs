using System.Collections;
using UnityEngine;

public class BounceShroom : MonoBehaviour
{
	private enum AnimType
	{
		Idle = 0,
		Bob = 1,
		Bounce = 2
	}

	[Tooltip("Active false by default since this script may be used elsewhere as just a flag")]
	public bool active;

	[Space]
	public GameObject idleParticlePrefab;

	public GameObject bounceSmallPrefab;

	public GameObject bounceLargePrefab;

	public GameObject heroParticlePrefab;

	private const float bounceParticleDelay = 0.25f;

	private static double nextBounceParticleTime;

	[Header("Animations")]
	public string idleAnim = "Idle 1";

	public string bobAnim = "Bob 1";

	public string bounceAnim = "Bounce 1";

	[Space]
	public GameObject hitEffect;

	[Space]
	public AudioSource audioSourcePrefab;

	public RandomAudioClipTable hitSound;

	private tk2dSpriteAnimator anim;

	private Coroutine idleRoutine;

	private Coroutine bounceRoutine;

	private static GameObject heroParticles;

	private static double nextCamShakeTime;

	private void Awake()
	{
		anim = GetComponentInChildren<tk2dSpriteAnimator>();
	}

	private void Start()
	{
		if (!active)
		{
			return;
		}
		GameObject gameObject = Object.Instantiate(idleParticlePrefab);
		if ((bool)gameObject)
		{
			gameObject.transform.SetPositionX(base.transform.position.x);
			gameObject.transform.SetPositionY(base.transform.position.y);
		}
		idleRoutine = StartCoroutine(Idle());
		CollisionEnterEvent[] componentsInChildren = GetComponentsInChildren<CollisionEnterEvent>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].CollisionEnteredDirectional += delegate(CollisionEnterEvent.Direction direction, Collision2D collision)
			{
				switch (direction)
				{
				case CollisionEnterEvent.Direction.Left:
					HeroController.instance.SendMessage("RecoilLeft");
					break;
				case CollisionEnterEvent.Direction.Right:
					HeroController.instance.SendMessage("RecoilRight");
					break;
				case CollisionEnterEvent.Direction.Top:
					HeroController.instance.SendMessage("Bounce");
					HeroController.instance.SendMessage((Random.Range(0, 2) == 0) ? "RecoilLeft" : "RecoilRight");
					break;
				}
				BounceSmall();
			};
		}
	}

	private IEnumerator Idle()
	{
		while (true)
		{
			PlayAnims(AnimType.Idle);
			yield return new WaitForSeconds(Random.Range(2f, 8f));
			yield return new WaitForSeconds(PlayAnims(AnimType.Bob));
		}
	}

	private float PlayAnims(AnimType animType)
	{
		tk2dSpriteAnimationClip clipByName = anim.GetClipByName(GetAnimName(animType));
		anim.Play(clipByName);
		return clipByName.Duration;
	}

	private string GetAnimName(AnimType animType)
	{
		string result = "NONE";
		switch (animType)
		{
		case AnimType.Idle:
			result = idleAnim;
			break;
		case AnimType.Bob:
			result = bobAnim;
			break;
		case AnimType.Bounce:
			result = bounceAnim;
			break;
		}
		return result;
	}

	protected void BounceSmall()
	{
		if (active)
		{
			if ((bool)bounceSmallPrefab)
			{
				bounceSmallPrefab.Spawn(base.transform.position).transform.SetPositionZ(-0.001f);
			}
			if (bounceRoutine == null)
			{
				bounceRoutine = StartCoroutine(Bounce());
			}
		}
	}

	private IEnumerator Bounce()
	{
		if (idleRoutine != null)
		{
			StopCoroutine(idleRoutine);
		}
		hitSound.SpawnAndPlayOneShot(audioSourcePrefab, base.transform.position);
		yield return new WaitForSeconds(PlayAnims(AnimType.Bounce));
		bounceRoutine = null;
		idleRoutine = StartCoroutine(Idle());
	}

	public void BounceLarge(bool useEffects = true)
	{
		if (!active)
		{
			return;
		}
		if (useEffects)
		{
			if (Time.timeAsDouble >= nextBounceParticleTime)
			{
				nextBounceParticleTime = Time.timeAsDouble + 0.25;
			}
			else
			{
				useEffects = false;
			}
		}
		if ((bool)bounceLargePrefab && useEffects)
		{
			bounceLargePrefab.Spawn(base.transform.position).transform.SetPositionZ(-0.001f);
		}
		if (bounceRoutine == null)
		{
			bounceRoutine = StartCoroutine(Bounce());
		}
		if (Time.timeAsDouble > nextCamShakeTime)
		{
			GameCameras.instance.cameraShakeFSM.SendEvent("EnemyKillShake");
			nextCamShakeTime = Time.timeAsDouble + 0.25;
		}
		if (!useEffects)
		{
			return;
		}
		if ((bool)hitEffect)
		{
			hitEffect.SetActive(value: true);
		}
		if ((bool)heroParticlePrefab)
		{
			ParticleSystem particleSystem = (heroParticles ? heroParticles.GetComponent<ParticleSystem>() : null);
			if (!heroParticles || !heroParticles.activeSelf || ((bool)particleSystem && !particleSystem.isEmitting))
			{
				heroParticles = heroParticlePrefab.Spawn(HeroController.instance.transform, new Vector3(0f, -1.5f, -0.002f));
			}
		}
	}
}
