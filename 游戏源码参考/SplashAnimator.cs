using UnityEngine;

public class SplashAnimator : MonoBehaviour
{
	public float scaleMin;

	public float scaleMax;

	private void OnEnable()
	{
		float num = Random.Range(scaleMin, scaleMax);
		base.transform.localScale = new Vector3(num, num, num);
		if ((float)Random.Range(0, 100) < 50f)
		{
			base.transform.localScale = new Vector3(0f - base.transform.localScale.x, base.transform.localScale.y, base.transform.localScale.z);
		}
	}
}
