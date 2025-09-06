using UnityEngine;

public class RandomTranslation : MonoBehaviour
{
	public float xRange;

	public float yRange;

	public float zRange;

	private bool shifted;

	private void OnEnable()
	{
		shifted = false;
	}

	private void LateUpdate()
	{
		if (!shifted)
		{
			DoShift();
			shifted = true;
		}
	}

	private void DoShift()
	{
		base.transform.position = new Vector3(base.transform.position.x + Random.Range(0f - xRange, xRange), base.transform.position.y + Random.Range(0f - yRange, yRange), base.transform.position.z + Random.Range(0f - zRange, zRange));
	}
}
