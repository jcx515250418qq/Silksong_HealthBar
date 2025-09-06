using UnityEngine;

public class DestroyOutOfBounds : MonoBehaviour
{
	private void Update()
	{
		if (base.transform.position.y < -1f)
		{
			base.gameObject.SetActive(value: false);
		}
	}
}
