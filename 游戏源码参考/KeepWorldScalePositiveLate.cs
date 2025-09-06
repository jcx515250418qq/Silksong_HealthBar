using UnityEngine;

public class KeepWorldScalePositiveLate : MonoBehaviour
{
	public bool x;

	public bool y;

	private void LateUpdate()
	{
		if (base.transform.lossyScale.x < 0f)
		{
			base.transform.localScale = new Vector3(0f - base.transform.localScale.x, base.transform.localScale.y, base.transform.localScale.z);
		}
	}
}
