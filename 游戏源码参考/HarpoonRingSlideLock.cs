using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class HarpoonRingSlideLock : MonoBehaviour
{
	[SerializeField]
	private GameObject ringPrefab;

	[SerializeField]
	private Transform ring;

	[SerializeField]
	private Vector2 dropOffset;

	[SerializeField]
	private float dropDelay;

	[SerializeField]
	private AnimationCurve dropCurve;

	[SerializeField]
	private float dropDuration;

	[SerializeField]
	private CameraShakeTarget dropImpactShake;

	[SerializeField]
	private PersistentBoolItem persistent;

	[SerializeField]
	private float unlockDelay;

	[SerializeField]
	private UnlockablePropBase[] unlockables;

	[Space]
	public UnityEvent BeforeDrop;

	public UnityEvent Dropped;

	private bool isComplete;

	private Vector3 initialRingPos;

	private Coroutine dropRoutine;

	private void OnDrawGizmosSelected()
	{
		if ((bool)ring)
		{
			if ((bool)ring.parent)
			{
				Gizmos.matrix = ring.parent.localToWorldMatrix;
			}
			Vector3 localPosition = ring.localPosition;
			Vector3 vector = localPosition + (Vector3)dropOffset;
			Gizmos.DrawWireSphere(vector, 0.1f);
			Gizmos.DrawLine(localPosition, vector);
		}
	}

	private void Awake()
	{
		GameObject obj = Object.Instantiate(ringPrefab, ring);
		obj.transform.localPosition = new Vector3(0f, -0.46f, 0f);
		obj.transform.SetPositionZ(ringPrefab.transform.position.z);
		Transform transform = obj.transform.Find("Backing");
		if ((bool)transform)
		{
			transform.gameObject.SetActive(value: false);
		}
		initialRingPos = ring.localPosition;
		if (!persistent)
		{
			return;
		}
		persistent.OnGetSaveState += delegate(out bool value)
		{
			value = isComplete;
		};
		persistent.OnSetSaveState += delegate(bool value)
		{
			isComplete = value;
			if (isComplete)
			{
				ring.localPosition = initialRingPos + (Vector3)dropOffset;
				UnlockablePropBase[] array = unlockables;
				for (int i = 0; i < array.Length; i++)
				{
					array[i].Opened();
				}
			}
		};
	}

	public void HeroOnRing()
	{
		if (!isComplete && dropRoutine == null)
		{
			dropRoutine = StartCoroutine(DropSequence());
		}
	}

	public void HeroOffRing()
	{
		if (!isComplete && dropRoutine != null)
		{
			StopCoroutine(dropRoutine);
			dropRoutine = null;
		}
	}

	private IEnumerator DropSequence()
	{
		yield return new WaitForSeconds(dropDelay);
		BeforeDrop.Invoke();
		isComplete = true;
		Vector3 targetRingPos = initialRingPos + (Vector3)dropOffset;
		for (float elapsed = 0f; elapsed < dropDuration; elapsed += Time.deltaTime)
		{
			float t = dropCurve.Evaluate(elapsed / dropDuration);
			ring.localPosition = Vector3.Lerp(initialRingPos, targetRingPos, t);
			yield return null;
		}
		ring.localPosition = targetRingPos;
		dropImpactShake.DoShake(this);
		Dropped.Invoke();
		FSMUtility.SendEventToGameObject(HeroController.instance.gameObject, "RING DROP IMPACT");
		yield return new WaitForSeconds(unlockDelay);
		UnlockablePropBase[] array = unlockables;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Open();
		}
		dropRoutine = null;
	}
}
