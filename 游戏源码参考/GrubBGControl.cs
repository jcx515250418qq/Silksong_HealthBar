using System.Collections;
using UnityEngine;

public class GrubBGControl : MonoBehaviour
{
	public int grubNumber;

	[Space]
	public TriggerEnterEvent waveRegion;

	[Space]
	public AudioSource audioSourcePrefab;

	public AudioEventRandom idleSounds;

	public AudioEventRandom waveSounds;

	private Coroutine idleRoutine;

	private Coroutine waveRoutine;

	private tk2dSpriteAnimator anim;

	private void Awake()
	{
		anim = GetComponent<tk2dSpriteAnimator>();
	}

	private void Start()
	{
		int playerDataInt = GameManager.instance.GetPlayerDataInt("grubsCollected");
		if (grubNumber > playerDataInt)
		{
			base.gameObject.SetActive(value: false);
			return;
		}
		idleRoutine = StartCoroutine(Idle());
		if (!waveRegion)
		{
			return;
		}
		waveRegion.OnTriggerEntered += delegate
		{
			if (waveRoutine == null)
			{
				waveRoutine = StartCoroutine(Wave());
			}
		};
	}

	private IEnumerator Idle()
	{
		anim.Play("Home Bounce");
		while (true)
		{
			yield return new WaitForSeconds(Random.Range(3f, 10f));
			idleSounds.SpawnAndPlayOneShot(audioSourcePrefab, base.transform.position);
		}
	}

	private IEnumerator Wave()
	{
		if (idleRoutine != null)
		{
			StopCoroutine(idleRoutine);
		}
		Vector3 position = base.transform.position;
		position.z = 0f;
		waveSounds.SpawnAndPlayOneShot(audioSourcePrefab, position);
		yield return StartCoroutine(anim.PlayAnimWait("Home Wave"));
		waveRoutine = null;
		idleRoutine = StartCoroutine(Idle());
	}
}
