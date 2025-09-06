using TeamCherry.Splines;
using UnityEngine;

[DisallowMultipleComponent]
public class LiftChain : MonoBehaviour
{
	[SerializeField]
	private SplineBase spline;

	[SerializeField]
	private float speed;

	[SerializeField]
	private float fpsLimit;

	private bool isMoving;

	private float moveDir;

	private double nextUpdateTime;

	protected void Awake()
	{
		spline.UpdateCondition = SplineBase.UpdateConditions.Manual;
		spline.SetDynamic();
	}

	private void Update()
	{
		if (!isMoving)
		{
			return;
		}
		spline.TextureOffset += speed * moveDir * Time.deltaTime;
		if (fpsLimit > Mathf.Epsilon)
		{
			if (Time.timeAsDouble < nextUpdateTime)
			{
				return;
			}
			nextUpdateTime = Time.timeAsDouble + (double)(1f / fpsLimit);
		}
		spline.UpdateSpline();
	}

	public void GoDown()
	{
		isMoving = true;
		moveDir = 1f;
	}

	public void GoUp()
	{
		isMoving = true;
		moveDir = -1f;
	}

	public void Stop()
	{
		isMoving = false;
	}
}
