using System.Collections;
using UnityEngine;

public class Drip : MonoBehaviour
{
	public float minWaitTime = 1f;

	public float maxWaitTime = 7f;

	public GameObject idleSprite;

	public GameObject dripSprite;

	private Animator dripAnimator;

	public Transform dripSpawnPoint;

	public float dripDelay = 0.6f;

	public GameObject dripPrefab;

	private void Awake()
	{
		dripAnimator = dripSprite.GetComponent<Animator>();
	}

	private void Start()
	{
		StartCoroutine(DripRoutine());
	}

	private IEnumerator DripRoutine()
	{
		while (true)
		{
			float seconds = Random.Range(minWaitTime, maxWaitTime);
			yield return new WaitForSeconds(seconds);
			idleSprite.SetActive(value: false);
			dripSprite.SetActive(value: true);
			StartCoroutine(DropDrip());
			yield return new WaitForSeconds(dripAnimator.GetCurrentAnimatorStateInfo(0).length);
			idleSprite.SetActive(value: true);
			dripSprite.SetActive(value: false);
		}
	}

	private IEnumerator DropDrip()
	{
		yield return new WaitForSeconds(dripDelay);
		dripPrefab.Spawn(dripSpawnPoint.position).transform.SetPositionZ(0.003f);
	}
}
