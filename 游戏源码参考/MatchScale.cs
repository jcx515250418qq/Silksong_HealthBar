using UnityEngine;

public class MatchScale : MonoBehaviour
{
	public Transform target;

	private bool hasStarted;

	private void Start()
	{
		hasStarted = true;
		DoMatch();
	}

	private void OnEnable()
	{
		if (hasStarted)
		{
			DoMatch();
		}
	}

	private void DoMatch()
	{
		base.transform.localScale = target.localScale;
	}
}
