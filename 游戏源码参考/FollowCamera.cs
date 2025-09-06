using UnityEngine;

public class FollowCamera : MonoBehaviour
{
	[SerializeField]
	private bool followX;

	[SerializeField]
	[ModifiableProperty]
	[Conditional("followX", true, false, false)]
	private float offsetX;

	[SerializeField]
	private bool followY;

	[SerializeField]
	[ModifiableProperty]
	[Conditional("followY", true, false, false)]
	private float offsetY;

	[SerializeField]
	private BoxCollider2D clampToCollider;

	[SerializeField]
	private bool clampToRange;

	[SerializeField]
	[ModifiableProperty]
	[Conditional("clampToRange", true, false, true)]
	private Vector2 xRange = new Vector2(0f, 9999f);

	[SerializeField]
	[ModifiableProperty]
	[Conditional("clampToRange", true, false, true)]
	private Vector2 yRange = new Vector2(0f, 9999f);

	[SerializeField]
	private GameObject enableOnCameraPosStart;

	private GameCameras gc;

	private GameObject gameCamera;

	private void Awake()
	{
		if ((bool)enableOnCameraPosStart)
		{
			enableOnCameraPosStart.SetActive(value: false);
		}
	}

	private void OnEnable()
	{
		gc = GameCameras.instance;
		gameCamera = gc.mainCamera.gameObject;
		gc.cameraController.PositionedAtHero += OnPositionedAtHero;
	}

	private void OnDisable()
	{
		if (gc != null)
		{
			gc.cameraController.PositionedAtHero -= OnPositionedAtHero;
			gc = null;
		}
	}

	private void Update()
	{
		UpdatePosition();
	}

	private void OnPositionedAtHero()
	{
		UpdatePosition();
		if ((bool)enableOnCameraPosStart)
		{
			enableOnCameraPosStart.SetActive(value: true);
		}
	}

	private void UpdatePosition()
	{
		Vector3 position = base.transform.position;
		Vector3 position2 = gameCamera.transform.position;
		if (followX)
		{
			position.x = position2.x + offsetX;
		}
		if (followY)
		{
			position.y = position2.y + offsetY;
		}
		if ((bool)clampToCollider)
		{
			Bounds bounds = clampToCollider.bounds;
			Vector3 min = bounds.min;
			Vector3 max = bounds.max;
			if (position.x < min.x)
			{
				position.x = min.x;
			}
			else if (position.x > max.x)
			{
				position.x = max.x;
			}
			if (position.y < min.y)
			{
				position.y = min.y;
			}
			else if (position.y > max.y)
			{
				position.y = max.y;
			}
		}
		if (clampToRange)
		{
			if (position.x < xRange.x)
			{
				position.x = xRange.x;
			}
			else if (position.x > xRange.y)
			{
				position.x = xRange.y;
			}
			if (position.y < yRange.x)
			{
				position.y = yRange.x;
			}
			else if (position.y > yRange.y)
			{
				position.y = yRange.y;
			}
		}
		base.transform.position = position;
	}

	public void SetClampRangeX(float rangeMin, float rangeMax)
	{
		xRange = new Vector2(rangeMin, rangeMax);
	}

	public void SetClampRangeY(float rangeMin, float rangeMax)
	{
		yRange = new Vector2(rangeMin, rangeMax);
	}
}
