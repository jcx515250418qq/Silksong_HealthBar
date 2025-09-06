using UnityEngine;

public class RandomRotation : MonoBehaviour
{
	private void Start()
	{
		RandomRotate();
	}

	private void OnEnable()
	{
		RandomRotate();
	}

	private void RandomRotate()
	{
		Transform obj = base.transform;
		Vector3 localEulerAngles = obj.localEulerAngles;
		localEulerAngles.z = Random.Range(0f, 360f);
		obj.localEulerAngles = localEulerAngles;
	}
}
