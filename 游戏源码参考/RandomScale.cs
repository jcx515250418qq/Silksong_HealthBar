using UnityEngine;

public class RandomScale : MonoBehaviour, IExternalDebris
{
	[SerializeField]
	private float minScale;

	[SerializeField]
	private float maxScale;

	[SerializeField]
	private bool randomlyFlipX;

	[SerializeField]
	private bool scaleOnEnable;

	private void Start()
	{
		ApplyScale();
	}

	private void OnEnable()
	{
		if (scaleOnEnable)
		{
			ApplyScale();
		}
	}

	private void ApplyScale()
	{
		float num = Random.Range(minScale, maxScale);
		float y = num;
		if (randomlyFlipX && (float)Random.Range(1, 100) > 50f)
		{
			num = 0f - num;
		}
		base.transform.localScale = new Vector3(num, y, 1f);
	}

	public void InitExternalDebris()
	{
		ApplyScale();
	}
}
