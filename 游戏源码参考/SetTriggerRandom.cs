using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class SetTriggerRandom : MonoBehaviour
{
	[Serializable]
	private class TriggerAllowRegion
	{
		public TrackTriggerObjects Trigger;

		public GameObject RequireActive;
	}

	[SerializeField]
	private string trigger = "Shine";

	[SerializeField]
	private float minInterval = 0.5f;

	[SerializeField]
	private float maxInterval = 1.5f;

	[SerializeField]
	private Animator[] animators;

	[Space]
	[SerializeField]
	private UnityEvent onTriggerSet;

	[Space]
	[SerializeField]
	private TriggerAllowRegion[] triggerAllowRegions;

	private Coroutine routine;

	private void Awake()
	{
		if (animators == null || animators.Length == 0)
		{
			Animator component = GetComponent<Animator>();
			animators = new Animator[1] { component };
		}
	}

	private void OnEnable()
	{
		routine = StartCoroutine(TriggerRoutine());
	}

	private void OnDisable()
	{
		if (routine != null)
		{
			StopCoroutine(routine);
		}
	}

	private IEnumerator TriggerRoutine()
	{
		while (true)
		{
			float seconds = UnityEngine.Random.Range(minInterval, maxInterval);
			yield return new WaitForSeconds(seconds);
			bool flag = false;
			TriggerAllowRegion[] array = triggerAllowRegions;
			foreach (TriggerAllowRegion triggerAllowRegion in array)
			{
				if (triggerAllowRegion.Trigger.IsInside && !triggerAllowRegion.RequireActive.activeInHierarchy)
				{
					flag = true;
					break;
				}
			}
			if (flag)
			{
				continue;
			}
			Animator[] array2 = animators;
			foreach (Animator animator in array2)
			{
				if ((bool)animator && animator.isActiveAndEnabled)
				{
					animator.SetTrigger(trigger);
				}
			}
			if (onTriggerSet != null)
			{
				onTriggerSet.Invoke();
			}
		}
	}
}
