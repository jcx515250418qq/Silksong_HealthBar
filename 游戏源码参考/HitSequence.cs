using System.Collections;
using GlobalSettings;
using UnityEngine;
using UnityEngine.Events;

public class HitSequence : MonoBehaviour
{
	[SerializeField]
	private HitRigidbody2D[] hitOrder;

	[SerializeField]
	private ToolItem requireHitWith;

	[SerializeField]
	private bool requireSameHitSource;

	[Space]
	[SerializeField]
	private float sequenceCompleteDelay;

	[SerializeField]
	private AudioEvent completeSound;

	[SerializeField]
	private string completeEvent;

	[Space]
	public UnityEvent OnSequenceComplete;

	private int nextHitIndex;

	private GameObject previousHitSource;

	private bool isComplete;

	private void OnDrawGizmos()
	{
		if (hitOrder == null)
		{
			return;
		}
		for (int i = 1; i < hitOrder.Length; i++)
		{
			HitRigidbody2D hitRigidbody2D = hitOrder[i];
			HitRigidbody2D hitRigidbody2D2 = hitOrder[i - 1];
			if ((bool)hitRigidbody2D && (bool)hitRigidbody2D2)
			{
				Gizmos.DrawLine(hitRigidbody2D.transform.position, hitRigidbody2D2.transform.position);
			}
		}
	}

	private void Awake()
	{
		for (int i = 0; i < hitOrder.Length; i++)
		{
			HitRigidbody2D obj = hitOrder[i];
			int capturedIndex = i;
			obj.WasHitBy += delegate(HitInstance hitInstance)
			{
				OnObjHit(capturedIndex, hitInstance);
			};
		}
	}

	private void OnObjHit(int index, HitInstance hitInstance)
	{
		if (isComplete || !hitInstance.IsFirstHit)
		{
			return;
		}
		if (index != nextHitIndex)
		{
			CancelSequence();
			return;
		}
		if ((bool)requireHitWith && hitInstance.RepresentingTool != requireHitWith)
		{
			CancelSequence();
			return;
		}
		if (requireSameHitSource && (bool)previousHitSource && hitInstance.Source != previousHitSource)
		{
			CancelSequence();
			return;
		}
		previousHitSource = hitInstance.Source;
		nextHitIndex++;
		if (nextHitIndex >= hitOrder.Length)
		{
			isComplete = true;
			StartCoroutine(CompleteDelayed());
		}
	}

	private void CancelSequence()
	{
		nextHitIndex = 0;
		previousHitSource = null;
	}

	private IEnumerator CompleteDelayed()
	{
		yield return new WaitForSeconds(sequenceCompleteDelay);
		OnSequenceComplete.Invoke();
		completeSound.SpawnAndPlayOneShot(Audio.DefaultUIAudioSourcePrefab, base.transform.position);
		if (!string.IsNullOrEmpty(completeEvent))
		{
			EventRegister.SendEvent(completeEvent);
		}
	}
}
