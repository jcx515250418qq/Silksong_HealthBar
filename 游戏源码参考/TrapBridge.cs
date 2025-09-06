using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public abstract class TrapBridge : MonoBehaviour
{
	[SerializeField]
	private TrackTriggerObjects insideTracker;

	[SerializeField]
	private float anticDelay;

	[SerializeField]
	private float openDelay;

	[SerializeField]
	private float openHoldDuration;

	[SerializeField]
	private AudioEvent openSound;

	[SerializeField]
	private AudioEvent closeSound;

	[Space]
	[SerializeField]
	protected UnityEvent onOpenAntic;

	[SerializeField]
	protected UnityEvent onOpen;

	[SerializeField]
	protected UnityEvent onOpened;

	[SerializeField]
	protected UnityEvent onClose;

	private Coroutine openBehaviourRoutine;

	private float openHoldTimeLeft;

	public void Open()
	{
		if (openBehaviourRoutine != null)
		{
			openHoldTimeLeft = openHoldDuration;
		}
		else
		{
			openBehaviourRoutine = StartCoroutine(OpenBehaviour());
		}
	}

	private IEnumerator OpenBehaviour()
	{
		if (anticDelay > 0f)
		{
			yield return new WaitForSeconds(anticDelay);
		}
		onOpenAntic.Invoke();
		if (openDelay > 0f)
		{
			yield return new WaitForSeconds(openDelay);
		}
		onOpen.Invoke();
		openSound.SpawnAndPlayOneShot(base.transform.position);
		yield return StartCoroutine(DoOpenAnim());
		onOpened.Invoke();
		if ((bool)insideTracker)
		{
			openHoldTimeLeft = openHoldDuration;
			while (openHoldTimeLeft > 0f)
			{
				if (insideTracker.IsInside)
				{
					openHoldTimeLeft = openHoldDuration;
				}
				else
				{
					openHoldTimeLeft -= Time.deltaTime;
				}
				yield return null;
			}
		}
		else
		{
			yield return new WaitForSeconds(openHoldDuration);
		}
		onClose.Invoke();
		closeSound.SpawnAndPlayOneShot(base.transform.position);
		yield return StartCoroutine(DoCloseAnim());
		openBehaviourRoutine = null;
	}

	protected abstract IEnumerator DoOpenAnim();

	protected abstract IEnumerator DoCloseAnim();
}
