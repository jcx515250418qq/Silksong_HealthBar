using UnityEngine;

public sealed class DustpiloExplosion : MonoBehaviour
{
	[SerializeField]
	private float removalDistance = 4f;

	private static UniqueList<DustpiloExplosion> activeExplosions = new UniqueList<DustpiloExplosion>();

	private PlayMakerFSM fsm;

	private bool started;

	private void Awake()
	{
		fsm = GetComponent<PlayMakerFSM>();
	}

	private void Start()
	{
		started = true;
		RemoveOthers();
		activeExplosions.Add(this);
	}

	private void OnEnable()
	{
		if (started)
		{
			RemoveOthers();
			activeExplosions.Add(this);
		}
	}

	private void OnDisable()
	{
		activeExplosions.Remove(this);
	}

	public void RemoveOthers()
	{
		float num = removalDistance * removalDistance;
		Vector2 vector = base.transform.position;
		activeExplosions.ReserveListUsage();
		foreach (DustpiloExplosion item in activeExplosions.List)
		{
			if (!(Vector2.SqrMagnitude((Vector2)item.transform.position - vector) > num))
			{
				item.End();
			}
		}
		activeExplosions.ReleaseListUsage();
	}

	private void End()
	{
		if (fsm == null)
		{
			fsm = GetComponent<PlayMakerFSM>();
			if (fsm == null)
			{
				return;
			}
		}
		fsm.SendEvent("END");
	}

	private void DrawGizmos()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(base.transform.position, removalDistance);
	}

	private void OnDrawGizmosSelected()
	{
		DrawGizmos();
	}
}
