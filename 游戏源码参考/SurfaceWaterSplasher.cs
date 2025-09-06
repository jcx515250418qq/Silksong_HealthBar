using UnityEngine;

public class SurfaceWaterSplasher : MonoBehaviour
{
	private Vector3 splashOffset;

	private int insideCount;

	private SurfaceWaterRegion insideRegion;

	public bool smallSplash;

	public bool recycleOnSplash;

	private bool isRecycled;

	private void Awake()
	{
		Collider2D component = GetComponent<Collider2D>();
		if ((bool)component)
		{
			Vector3 center = component.bounds.center;
			splashOffset = center - base.transform.position;
			splashOffset.z = 0f;
		}
	}

	private void OnEnable()
	{
		isRecycled = false;
		insideCount = 0;
	}

	private void OnDisable()
	{
		insideRegion = null;
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (isRecycled || !base.gameObject.activeSelf)
		{
			return;
		}
		SurfaceWaterRegion componentInParent = collision.gameObject.GetComponentInParent<SurfaceWaterRegion>();
		if ((bool)componentInParent)
		{
			insideCount++;
			if (insideCount == 1)
			{
				insideRegion = componentInParent;
				DoSplashIn();
			}
		}
	}

	private void OnTriggerExit2D(Collider2D collision)
	{
		if (insideCount == 0 || !base.gameObject.activeSelf)
		{
			return;
		}
		SurfaceWaterRegion componentInParent = collision.gameObject.GetComponentInParent<SurfaceWaterRegion>();
		if ((bool)componentInParent)
		{
			insideCount--;
			if (insideCount == 0)
			{
				componentInParent.DoSplashOut(base.transform, Vector2.zero);
				insideRegion = null;
			}
		}
	}

	public void DoSplashIn()
	{
		if (!isRecycled && (bool)insideRegion)
		{
			if (!smallSplash)
			{
				insideRegion.DoSplashIn(base.transform, splashOffset, isBigSplash: false);
			}
			else
			{
				insideRegion.DoSplashInSmall(base.transform, splashOffset);
			}
			if (recycleOnSplash)
			{
				isRecycled = true;
				base.gameObject.Recycle();
			}
		}
	}
}
