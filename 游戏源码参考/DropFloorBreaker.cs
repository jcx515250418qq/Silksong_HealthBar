using System;
using System.Collections;
using System.Linq;
using TeamCherry.SharedUtils;
using UnityEngine;

public class DropFloorBreaker : MonoBehaviour
{
	[Serializable]
	private struct BreakThreshold
	{
		public float YPos;

		public GameObject Break;

		public GameObject BreakEffects;
	}

	[SerializeField]
	private Rigidbody2D dropBody;

	[SerializeField]
	private GameObject activeInIdle;

	[SerializeField]
	private GameObject activeInFall;

	[SerializeField]
	private GameObject droppedCamlock;

	[SerializeField]
	private BreakThreshold[] breakThresholds;

	[SerializeField]
	private OverrideFloat endYPos;

	private Coroutine dropRoutine;

	private void OnDrawGizmosSelected()
	{
		if (breakThresholds != null)
		{
			Gizmos.matrix = base.transform.localToWorldMatrix;
			BreakThreshold[] array = breakThresholds;
			for (int i = 0; i < array.Length; i++)
			{
				BreakThreshold breakThreshold = array[i];
				Gizmos.DrawWireSphere(new Vector3(0f, breakThreshold.YPos, 0f), 0.2f);
			}
		}
	}

	private void Awake()
	{
		if ((bool)dropBody)
		{
			dropBody.isKinematic = true;
			dropBody.linearVelocity = Vector2.zero;
			dropBody.angularVelocity = 0f;
		}
		if ((bool)activeInIdle)
		{
			activeInIdle.SetActive(value: true);
		}
		if ((bool)activeInFall)
		{
			activeInFall.SetActive(value: false);
		}
		BreakThreshold[] array = breakThresholds;
		for (int i = 0; i < array.Length; i++)
		{
			BreakThreshold breakThreshold = array[i];
			if ((bool)breakThreshold.Break)
			{
				breakThreshold.Break.SetActive(value: true);
			}
			if ((bool)breakThreshold.BreakEffects)
			{
				breakThreshold.BreakEffects.SetActive(value: false);
			}
		}
	}

	public void Break()
	{
		if (dropRoutine == null)
		{
			if ((bool)dropBody)
			{
				dropBody.isKinematic = false;
			}
			if ((bool)activeInIdle)
			{
				activeInIdle.SetActive(value: false);
			}
			if ((bool)activeInFall)
			{
				activeInFall.SetActive(value: true);
			}
			dropRoutine = StartCoroutine(DropRoutine());
			if ((bool)droppedCamlock)
			{
				droppedCamlock.SetActive(value: true);
			}
		}
	}

	public void Broken()
	{
		if (dropRoutine != null)
		{
			StopCoroutine(dropRoutine);
		}
		if ((bool)dropBody)
		{
			dropBody.gameObject.SetActive(value: false);
		}
		if ((bool)activeInIdle)
		{
			activeInIdle.SetActive(value: false);
		}
		if ((bool)activeInFall)
		{
			activeInFall.SetActive(value: false);
		}
		BreakThreshold[] array = breakThresholds;
		for (int i = 0; i < array.Length; i++)
		{
			BreakThreshold breakThreshold = array[i];
			if ((bool)breakThreshold.Break)
			{
				breakThreshold.Break.SetActive(value: false);
			}
			if ((bool)breakThreshold.BreakEffects)
			{
				breakThreshold.BreakEffects.SetActive(value: false);
			}
		}
		if ((bool)droppedCamlock)
		{
			droppedCamlock.SetActive(value: true);
		}
	}

	private IEnumerator DropRoutine()
	{
		BreakThreshold[] orderedBreakThresholds = breakThresholds.OrderBy((BreakThreshold a) => a.YPos).Reverse().ToArray();
		Transform trans = base.transform;
		WaitForFixedUpdate wait = new WaitForFixedUpdate();
		int currentIndex = 0;
		while (currentIndex < orderedBreakThresholds.Length)
		{
			Vector2 position = dropBody.position;
			Vector3 vector = trans.InverseTransformPoint(position);
			BreakThreshold breakThreshold = orderedBreakThresholds[currentIndex];
			if (vector.y < breakThreshold.YPos)
			{
				if ((bool)breakThreshold.Break)
				{
					breakThreshold.Break.SetActive(value: false);
				}
				if ((bool)breakThreshold.BreakEffects)
				{
					breakThreshold.BreakEffects.SetActive(value: true);
				}
				currentIndex++;
			}
			yield return wait;
		}
		float endYPosVal = (endYPos.IsEnabled ? endYPos.Value : (-20f));
		while (true)
		{
			Vector2 position2 = dropBody.position;
			if (trans.InverseTransformPoint(position2).y < endYPosVal)
			{
				break;
			}
			yield return wait;
		}
		if ((bool)activeInFall)
		{
			activeInFall.SetActive(value: false);
		}
	}
}
