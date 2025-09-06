using System.Collections;
using UnityEngine;

public class DropRecycle : MonoBehaviour
{
	[SerializeField]
	private float dropDelay;

	[SerializeField]
	private float dropDuration;

	[SerializeField]
	private float dropDistance;

	[SerializeField]
	private Rigidbody2D waitForBodySleep;

	[SerializeField]
	private bool waitForCall;

	private bool hasStarted;

	private Coroutine dropRoutine;

	private Collider2D[] colliders;

	private bool[] enabledStates;

	private void Awake()
	{
		colliders = GetComponentsInChildren<Collider2D>(includeInactive: true);
		enabledStates = new bool[colliders.Length];
	}

	private void OnEnable()
	{
		if (hasStarted && !waitForCall)
		{
			StartDrop();
		}
	}

	private void Start()
	{
		if (!waitForCall)
		{
			StartDrop();
		}
		hasStarted = true;
	}

	private void OnDisable()
	{
		if (dropRoutine != null)
		{
			StopCoroutine(dropRoutine);
			dropRoutine = null;
		}
	}

	public static void AddInactive(GameObject gameObject)
	{
		DropRecycle dropRecycle = gameObject.GetComponent<DropRecycle>();
		if (!dropRecycle)
		{
			dropRecycle = gameObject.AddComponent<DropRecycle>();
		}
		dropRecycle.waitForCall = true;
		Collider2D component = gameObject.GetComponent<Collider2D>();
		if ((bool)component)
		{
			dropRecycle.dropDuration = 0f;
			dropRecycle.dropDistance = component.bounds.size.y + 0.5f;
		}
		else
		{
			dropRecycle.dropDuration = 2f;
			dropRecycle.dropDistance = 0f;
		}
	}

	public void StartDrop()
	{
		if (base.isActiveAndEnabled && dropRoutine == null)
		{
			dropRoutine = StartCoroutine(DropTimer());
		}
	}

	private IEnumerator DropTimer()
	{
		for (int i = 0; i < colliders.Length; i++)
		{
			enabledStates[i] = colliders[i].enabled;
		}
		if ((bool)waitForBodySleep)
		{
			for (float elapsed = 0f; elapsed < dropDelay; elapsed = ((!waitForBodySleep.IsSleeping()) ? 0f : (elapsed + Time.deltaTime)))
			{
				yield return null;
			}
		}
		else
		{
			yield return new WaitForSeconds(dropDelay);
		}
		Collider2D[] array = colliders;
		for (int j = 0; j < array.Length; j++)
		{
			array[j].enabled = false;
		}
		float dropTimeLeft = dropDuration;
		float startY = base.transform.position.y;
		do
		{
			yield return null;
			if (dropDuration > 0f)
			{
				dropTimeLeft -= Time.deltaTime;
				if (dropTimeLeft <= 0f)
				{
					break;
				}
			}
		}
		while (!(dropDistance > 0f) || !(startY - base.transform.position.y >= dropDistance));
		for (int k = 0; k < colliders.Length; k++)
		{
			colliders[k].enabled = enabledStates[k];
		}
		base.gameObject.Recycle();
	}
}
