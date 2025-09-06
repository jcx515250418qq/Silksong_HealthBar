using UnityEngine;

public class SetZRandom : MonoBehaviour
{
	[SerializeField]
	private float zMin;

	[SerializeField]
	private float zMax;

	[SerializeField]
	private bool localSpace;

	[SerializeField]
	private bool relativeToInitial;

	private float initialZ;

	private void Awake()
	{
		Transform transform = base.transform;
		initialZ = (localSpace ? transform.localPosition.z : transform.position.z);
	}

	private void Start()
	{
		DoSetZ();
	}

	private void OnEnable()
	{
		DoSetZ();
	}

	private void DoSetZ()
	{
		float num = Random.Range(zMin, zMax);
		if (relativeToInitial)
		{
			num += initialZ;
		}
		if (localSpace)
		{
			base.transform.SetLocalPositionZ(num);
		}
		else
		{
			base.transform.SetPositionZ(num);
		}
	}
}
