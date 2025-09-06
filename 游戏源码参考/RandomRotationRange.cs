using UnityEngine;

public class RandomRotationRange : MonoBehaviour
{
	public float min;

	public float max;

	public bool relativeToStartRotation;

	private bool started;

	private void Start()
	{
		RandomRotate();
		started = true;
	}

	private void OnEnable()
	{
		if (started)
		{
			RandomRotate();
		}
	}

	private void RandomRotate()
	{
		if (relativeToStartRotation)
		{
			float num = Random.Range(min, max);
			float z = base.transform.localEulerAngles.z + num;
			base.transform.localEulerAngles = new Vector3(base.transform.localEulerAngles.x, base.transform.localEulerAngles.y, z);
		}
		else
		{
			base.transform.localEulerAngles = new Vector3(base.transform.localEulerAngles.x, base.transform.localEulerAngles.y, Random.Range(min, max));
		}
	}
}
