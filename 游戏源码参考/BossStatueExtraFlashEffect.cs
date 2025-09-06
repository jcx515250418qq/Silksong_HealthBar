using System;
using System.Collections;
using UnityEngine;

public class BossStatueExtraFlashEffect : MonoBehaviour
{
	public BossStatueFlashEffect mainEffect;

	public float flashSequenceDelay = 2f;

	public TriggerEnterEvent triggerUnlockEvent;

	public float toggleEnableTime = 1.35f;

	public GameObject toggle;

	private void Start()
	{
		BossStatue componentInParent = GetComponentInParent<BossStatue>();
		if ((bool)componentInParent && !componentInParent.DreamStatueState.hasBeenSeen && !componentInParent.isAlwaysUnlockedDream)
		{
			toggle.SetActive(value: false);
			if (componentInParent.DreamStatueState.isUnlocked)
			{
				if (componentInParent.StatueState.isUnlocked && !componentInParent.StatueState.hasBeenSeen && (bool)mainEffect)
				{
					mainEffect.OnFlashBegin += delegate
					{
						Invoke("DoAppear", flashSequenceDelay);
					};
				}
				else if ((bool)triggerUnlockEvent)
				{
					TriggerEnterEvent.CollisionEvent temp = null;
					temp = (TriggerEnterEvent.CollisionEvent)Delegate.Combine(temp, (TriggerEnterEvent.CollisionEvent)delegate
					{
						DoAppear();
						triggerUnlockEvent.OnTriggerEntered -= temp;
					});
					triggerUnlockEvent.OnTriggerEntered += temp;
				}
			}
		}
		base.gameObject.SetActive(value: false);
	}

	private void DoAppear()
	{
		base.gameObject.SetActive(value: true);
		StartCoroutine(AppearRoutine(toggle));
	}

	private IEnumerator AppearRoutine(GameObject toggle)
	{
		yield return new WaitForSeconds(toggleEnableTime);
		toggle.SetActive(value: true);
	}
}
