using UnityEngine;

public class SetRotationThreadPossess : MonoBehaviour
{
	[SerializeField]
	private float angleOffset;

	private bool hasStarted;

	private void OnEnable()
	{
		if (hasStarted)
		{
			DoSetRotation();
		}
	}

	private void Start()
	{
		hasStarted = true;
		DoSetRotation();
	}

	public void Update()
	{
		DoSetRotation();
	}

	private void DoSetRotation()
	{
		float angleToSilkThread = GameManager.instance.sm.AngleToSilkThread;
		base.transform.SetRotation2D(angleToSilkThread + angleOffset);
	}
}
