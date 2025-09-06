using System.Collections;
using UnityEngine;

public class DreamPlantOrb : MonoBehaviour
{
	public static DreamPlant plant;

	public BasicSpriteAnimator pickupAnim;

	private Renderer rend;

	private Vector3 initialScale;

	public AudioSource loopSource;

	[Space]
	public AudioSource soundSource;

	public AudioClip collectSound;

	public float basePitch = 0.85f;

	public float increasePitch = 0.025f;

	public float maxPitch = 1.25f;

	public float pitchReturnDelay = 3f;

	private static float currentPitch;

	private static double pitchReturnTime;

	[Space]
	public GameObject whiteFlash;

	[Space]
	public ParticleSystem pickupParticles;

	[Space]
	public ParticleSystem trailParticles;

	public ParticleSystem activateParticles;

	[Space]
	public AnimationCurve spread1Curve;

	public AnimationCurve spread2Curve;

	private bool pickedUp;

	private bool canPickup;

	private bool isActive;

	private bool didEverSetSaveState;

	private Coroutine spreadRoutine;

	private void Awake()
	{
		rend = GetComponent<Renderer>();
		PersistentBoolItem persist = GetComponent<PersistentBoolItem>();
		if (!persist)
		{
			return;
		}
		persist.OnGetSaveState += delegate(out bool value)
		{
			value = pickedUp;
		};
		persist.OnSetSaveState += delegate(bool value)
		{
			if (!didEverSetSaveState)
			{
				pickedUp = value;
				didEverSetSaveState = true;
				persist.enabled = false;
			}
		};
		persist.PreSetup();
	}

	private void Start()
	{
		SetActive(value: false);
		initialScale = base.transform.localScale;
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (!isActive || pickedUp || !canPickup || !(collision.tag == "Player"))
		{
			return;
		}
		GameManager.instance.IncrementPlayerDataInt("dreamOrbs");
		EventRegister.SendEvent(EventRegisterEvents.DreamOrbCollect);
		if ((bool)soundSource && (bool)collectSound)
		{
			if (currentPitch <= 0f || Time.timeAsDouble >= pitchReturnTime)
			{
				currentPitch = basePitch;
			}
			if (currentPitch > maxPitch)
			{
				currentPitch = maxPitch;
			}
			soundSource.pitch = currentPitch;
			soundSource.PlayOneShot(collectSound);
			currentPitch += increasePitch;
			pitchReturnTime = Time.timeAsDouble + (double)pitchReturnDelay;
		}
		if ((bool)pickupParticles)
		{
			pickupParticles.gameObject.SetActive(value: true);
		}
		if ((bool)whiteFlash)
		{
			whiteFlash.gameObject.SetActive(value: true);
		}
		if ((bool)pickupAnim)
		{
			pickupAnim.gameObject.SetActive(value: true);
			StartCoroutine(DisableAfterTime(pickupAnim.gameObject, pickupAnim.Length));
		}
		PersistentBoolItem component = GetComponent<PersistentBoolItem>();
		if ((bool)component)
		{
			component.enabled = true;
		}
		pickedUp = true;
		Disable();
	}

	public void Show()
	{
		if (!pickedUp)
		{
			SetActive(value: true);
			plant.AddOrbCount();
			spreadRoutine = StartCoroutine(Spread());
		}
	}

	private void SetActive(bool value)
	{
		isActive = value;
		if ((bool)rend)
		{
			rend.enabled = value;
		}
		if ((bool)loopSource)
		{
			loopSource.enabled = value;
		}
	}

	private IEnumerator Spread()
	{
		if ((bool)rend)
		{
			rend.enabled = false;
		}
		yield return null;
		base.transform.localScale = initialScale.MultiplyElements(new Vector3(0.5f, 0.5f, 1f));
		Vector3 position = plant.transform.position;
		position.z = Random.Range(0.003f, 0.004f);
		position.x += Random.Range(-1, 1);
		position.y += Random.Range(-3, -2);
		Vector3 initialPos = base.transform.position;
		initialPos.z = 0.003f;
		base.transform.position = position;
		if ((bool)rend)
		{
			rend.enabled = true;
		}
		if ((bool)trailParticles)
		{
			trailParticles.gameObject.SetActive(value: true);
		}
		Vector3 vector = initialPos - base.transform.position;
		vector.z = 0f;
		vector.Normalize();
		vector *= Random.Range(2f, 10f);
		Vector3 position2 = base.transform.position + vector;
		yield return StartCoroutine(TweenPosition(position2, 1f, spread1Curve));
		yield return new WaitForSeconds(Random.Range(1f, 1.5f));
		yield return StartCoroutine(TweenPosition(initialPos, 1f, spread2Curve));
		base.transform.localScale = initialScale.MultiplyElements(new Vector3(1f, 1f, 1f));
		if ((bool)whiteFlash)
		{
			whiteFlash.gameObject.SetActive(value: true);
		}
		if ((bool)activateParticles)
		{
			activateParticles.gameObject.SetActive(value: true);
		}
		if ((bool)trailParticles)
		{
			trailParticles.Stop(withChildren: true, ParticleSystemStopBehavior.StopEmitting);
		}
		canPickup = true;
	}

	private void Disable()
	{
		pickedUp = true;
		if (plant != null)
		{
			plant.RemoveOrbCount();
		}
		SetActive(value: false);
		if (spreadRoutine != null)
		{
			StopCoroutine(spreadRoutine);
		}
	}

	private IEnumerator DisableAfterTime(GameObject obj, float time)
	{
		yield return new WaitForSeconds(time);
		obj.SetActive(value: false);
	}

	private IEnumerator TweenPosition(Vector3 position, float time, AnimationCurve curve)
	{
		Vector3 startPos = base.transform.position;
		for (float elapsed = 0f; elapsed <= time; elapsed += Time.deltaTime)
		{
			base.transform.position = Vector3.Lerp(startPos, position, curve.Evaluate(elapsed / time));
			yield return null;
		}
		base.transform.position = position;
	}
}
