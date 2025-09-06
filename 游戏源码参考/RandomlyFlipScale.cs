using UnityEngine;

public class RandomlyFlipScale : MonoBehaviour
{
	[SerializeField]
	private bool flipX;

	[SerializeField]
	private bool flipY;

	[SerializeField]
	private float flipChance = 50f;

	[SerializeField]
	private bool doOnEnable;

	private bool didScale;

	private void Start()
	{
		if (!didScale)
		{
			ApplyScale();
		}
	}

	private void OnEnable()
	{
		if (doOnEnable)
		{
			ApplyScale();
		}
	}

	public void ApplyScale()
	{
		if ((float)Random.Range(1, 100) < flipChance)
		{
			if (flipX)
			{
				base.transform.localScale = new Vector3(0f - base.transform.localScale.x, base.transform.localScale.y, base.transform.localScale.z);
			}
			if (flipY)
			{
				base.transform.localScale = new Vector3(base.transform.localScale.x, 0f - base.transform.localScale.y, base.transform.localScale.z);
			}
		}
		didScale = true;
	}
}
