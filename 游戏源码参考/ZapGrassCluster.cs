using UnityEngine;

public class ZapGrassCluster : MonoBehaviour
{
	public Transform grassL;

	public Transform grassR;

	public TrackTriggerObjects collisionTracker;

	public ParticleSystem ptIdle;

	public ParticleSystem ptAttack;

	public ColorFader attackLight;

	private AudioSource audioSource;

	private bool inRange;

	private bool zapping;

	private float rangeTimer;

	private string[] anim_IdleLeft = new string[4] { "Idle L1", "Idle L2", "Idle L3", "Idle L4" };

	private string[] anim_IdleRight = new string[4] { "Idle R1", "Idle R2", "Idle R3", "Idle R4" };

	private void Start()
	{
		ParticleSystem.EmissionModule emission = ptAttack.emission;
		emission.enabled = false;
		foreach (Transform item in grassL)
		{
			tk2dSpriteAnimator component = item.gameObject.GetComponent<tk2dSpriteAnimator>();
			string text = anim_IdleLeft[Random.Range(0, 3)];
			component.Play(text);
			float num = Random.Range(1f, 1.2f);
			item.localScale = new Vector3(num, num, num);
		}
		foreach (Transform item2 in grassR)
		{
			tk2dSpriteAnimator component2 = item2.gameObject.GetComponent<tk2dSpriteAnimator>();
			string text2 = anim_IdleRight[Random.Range(0, 3)];
			component2.Play(text2);
			float num2 = Random.Range(1f, 1.2f);
			item2.localScale = new Vector3(num2, num2, num2);
		}
		audioSource = GetComponent<AudioSource>();
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collisionTracker.InsideCount >= 1)
		{
			inRange = true;
		}
	}

	private void OnTriggerExit2D(Collider2D collision)
	{
		if (collisionTracker.InsideCount <= 0)
		{
			inRange = false;
		}
	}

	private void Update()
	{
		if (inRange && !zapping)
		{
			StartZapping();
		}
		else if (!inRange && zapping && rangeTimer <= 0f)
		{
			StopZapping();
		}
		if (inRange && rangeTimer != 1f)
		{
			rangeTimer = 0.25f;
		}
		if (!inRange && rangeTimer > 0f)
		{
			rangeTimer -= Time.deltaTime;
		}
	}

	private void StartZapping()
	{
		zapping = true;
		ParticleSystem.EmissionModule emission = ptAttack.emission;
		emission.enabled = true;
		foreach (Transform item in grassL)
		{
			item.gameObject.GetComponent<tk2dSpriteAnimator>().Play("Zap L");
		}
		foreach (Transform item2 in grassR)
		{
			item2.gameObject.GetComponent<tk2dSpriteAnimator>().Play("Zap R");
		}
		audioSource.Play();
		attackLight.Fade(up: true);
	}

	private void StopZapping()
	{
		zapping = false;
		ParticleSystem.EmissionModule emission = ptAttack.emission;
		emission.enabled = false;
		foreach (Transform item in grassL)
		{
			tk2dSpriteAnimator component = item.gameObject.GetComponent<tk2dSpriteAnimator>();
			string text = anim_IdleLeft[Random.Range(0, 3)];
			component.Play(text);
		}
		foreach (Transform item2 in grassR)
		{
			tk2dSpriteAnimator component2 = item2.gameObject.GetComponent<tk2dSpriteAnimator>();
			string text2 = anim_IdleRight[Random.Range(0, 3)];
			component2.Play(text2);
		}
		audioSource.Stop();
		attackLight.Fade(up: false);
	}
}
