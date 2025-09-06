using UnityEngine;

public class SetZMin : MonoBehaviour
{
	public float zMin;

	private void OnEnable()
	{
		SetZ();
	}

	private void Start()
	{
		SetZ();
	}

	private void SetZ()
	{
		if (base.transform.position.z < zMin)
		{
			base.transform.position = new Vector3(base.transform.position.x, base.transform.position.y, zMin);
		}
	}
}
