using UnityEngine;

public class DeparentAfterDelay : MonoBehaviour
{
	public float time;

	private float timer;

	private bool deparented;

	private void Update()
	{
		if (!deparented)
		{
			if (timer > 0f)
			{
				timer -= Time.deltaTime;
				return;
			}
			base.gameObject.transform.parent = null;
			deparented = true;
			base.enabled = false;
		}
	}
}
