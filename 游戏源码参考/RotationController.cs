using UnityEngine;

public class RotationController : MonoBehaviour
{
	public float speed = 1f;

	private void Update()
	{
		if (Input.GetAxis("Horizontal") != 0f)
		{
			Vector3 eulerAngles = base.transform.eulerAngles;
			eulerAngles.y += Input.GetAxis("Horizontal") * speed;
			base.transform.eulerAngles = eulerAngles;
		}
	}
}
