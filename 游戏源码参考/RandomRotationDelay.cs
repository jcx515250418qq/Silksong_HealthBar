using System.Collections;
using UnityEngine;

public class RandomRotationDelay : MonoBehaviour
{
	public float delay = 0.25f;

	private void Start()
	{
		StartCoroutine(RandomRotate());
	}

	private void OnEnable()
	{
		StartCoroutine(RandomRotate());
	}

	private IEnumerator RandomRotate()
	{
		yield return new WaitForSeconds(delay);
		Transform obj = base.transform;
		Vector3 localEulerAngles = obj.localEulerAngles;
		localEulerAngles.z = Random.Range(0f, 360f);
		obj.localEulerAngles = localEulerAngles;
	}
}
