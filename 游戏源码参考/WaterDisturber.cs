using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class WaterDisturber : MonoBehaviour
{
	public float minForce = 0.5f;

	public float maxForce = 1f;

	[Space]
	public float minDelay = 0.1f;

	public float maxDelay = 0.25f;

	[Space]
	public FlingGameObject splashParticles;

	private BoxCollider2D overlapCollider;

	private List<WaterDetector> detectors = new List<WaterDetector>();

	private void Start()
	{
		overlapCollider = GetComponent<BoxCollider2D>();
		Collider2D[] array = Physics2D.OverlapBoxAll((Vector2)base.transform.position + overlapCollider.offset, overlapCollider.size, 0f);
		for (int i = 0; i < array.Length; i++)
		{
			WaterDetector component = array[i].GetComponent<WaterDetector>();
			if ((bool)component)
			{
				detectors.Add(component);
			}
		}
		overlapCollider.enabled = false;
		if (detectors.Count > 0)
		{
			StartCoroutine(Disturb());
		}
	}

	private IEnumerator Disturb()
	{
		float num = float.MaxValue;
		float num2 = float.MinValue;
		foreach (WaterDetector detector in detectors)
		{
			Vector3 position = detector.transform.position;
			if (position.x < num)
			{
				num = position.x;
			}
			if (position.x > num2)
			{
				num2 = position.x;
			}
		}
		while (true)
		{
			float force = Random.Range(minForce, maxForce);
			float seconds = Random.Range(minDelay, maxDelay);
			int index = Random.Range(0, detectors.Count);
			detectors[index].Splash(force);
			splashParticles.Fling(base.transform.position, 1f);
			yield return new WaitForSeconds(seconds);
		}
	}
}
