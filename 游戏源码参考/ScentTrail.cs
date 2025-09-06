using System;
using System.Collections.Generic;
using UnityEngine;

public class ScentTrail : MonoBehaviour
{
	public List<GameObject> scentClouds;

	private void Start()
	{
		foreach (Transform item in base.transform)
		{
			scentClouds.Add(item.gameObject);
		}
		float num = 0f;
		for (int i = 0; i < scentClouds.Count; i++)
		{
			if (i < scentClouds.Count - 1)
			{
				Transform transform2 = scentClouds[i].transform;
				Transform obj = scentClouds[i + 1].transform;
				float y = obj.position.y - transform2.position.y;
				float x = obj.position.x - transform2.position.x;
				float num2;
				for (num2 = Mathf.Atan2(y, x) * (180f / MathF.PI); num2 < 0f; num2 += 360f)
				{
				}
				transform2.SetRotationZ(num2);
				ParticleSystem.MainModule main = scentClouds[i].GetComponent<ParticleSystem>().main;
				main.startDelay = num;
				ParticleSystem.MainModule main2 = scentClouds[i].transform.GetChild(0).gameObject.GetComponent<ParticleSystem>().main;
				main2.startDelay = num;
				num += 0.1f;
			}
			else
			{
				scentClouds[i].SetActive(value: false);
			}
		}
	}

	public void StartTrail()
	{
		for (int i = 0; i < scentClouds.Count; i++)
		{
			scentClouds[i].GetComponent<ParticleSystem>().Play();
		}
	}

	public void StopTrail()
	{
		for (int i = 0; i < scentClouds.Count; i++)
		{
			scentClouds[i].GetComponent<ParticleSystem>().Stop();
		}
	}
}
