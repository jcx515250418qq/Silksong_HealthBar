using System.Collections;
using UnityEngine;

public class SetZ : MonoBehaviour
{
	[SerializeField]
	[ModifiableProperty]
	[Conditional("randomizeFromStartingValue", false, false, false)]
	private float z;

	[SerializeField]
	[ModifiableProperty]
	[Conditional("randomizeFromStartingValue", false, false, false)]
	private bool dontRandomize;

	[SerializeField]
	private bool randomizeFromStartingValue;

	[SerializeField]
	private float delayBeforeRandomizing = 0.5f;

	[SerializeField]
	private int waitFrames;

	[SerializeField]
	private bool deParent;

	private bool cancel;

	private float setZ;

	private Coroutine delayRoutine;

	private WaitForSeconds waitForSeconds;

	private bool started;

	private void OnEnable()
	{
		if (started)
		{
			StartSetZ();
		}
	}

	private void Start()
	{
		StartSetZ();
		started = true;
	}

	private void OnDisable()
	{
		if (delayRoutine != null)
		{
			StopCoroutine(delayRoutine);
			delayRoutine = null;
		}
	}

	private void StartSetZ()
	{
		if (delayBeforeRandomizing > 0f || waitFrames > 0)
		{
			if (delayRoutine == null)
			{
				delayRoutine = StartCoroutine(SetPosition());
			}
		}
		else
		{
			DoSetZ();
		}
	}

	private IEnumerator SetPosition()
	{
		if (delayBeforeRandomizing > 0f)
		{
			if (waitForSeconds == null)
			{
				waitForSeconds = new WaitForSeconds(delayBeforeRandomizing);
			}
			yield return waitForSeconds;
		}
		if (waitFrames > 0)
		{
			for (int i = 0; i < waitFrames; i++)
			{
				yield return null;
			}
		}
		if (!cancel)
		{
			DoSetZ();
		}
		delayRoutine = null;
	}

	private void DoSetZ()
	{
		setZ = z;
		Vector3 position = base.transform.position;
		if (randomizeFromStartingValue)
		{
			setZ = Random.Range(position.z, position.z + 0.0009999f);
		}
		else if (!dontRandomize)
		{
			setZ = Random.Range(z, z + 0.0009999f);
		}
		if (deParent && (bool)base.transform.parent)
		{
			base.transform.SetParent(null, worldPositionStays: true);
		}
		base.transform.SetPositionZ(setZ);
	}

	public void CancelSetZ()
	{
		cancel = true;
	}
}
