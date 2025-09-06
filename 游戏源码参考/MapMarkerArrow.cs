using GlobalEnums;
using UnityEngine;

public abstract class MapMarkerArrow : MonoBehaviour
{
	[SerializeField]
	private GameObject arrowPrefab;

	private Vector2 initialPos;

	private GameObject arrow;

	private bool wasOutsideView;

	private GameMap gameMap;

	private Collider2D viewportEdge;

	private InventoryWideMap wideMap;

	private void Awake()
	{
		Transform transform = base.transform;
		initialPos = transform.localPosition;
		gameMap = GetComponentInParent<GameMap>();
		if ((bool)gameMap)
		{
			gameMap.UpdateQuickMapDisplay += OnGameMapUpdateQuickMapDisplay;
			gameMap.ViewPosUpdated += OnGameMapViewPosUpdated;
			arrow = Object.Instantiate(arrowPrefab, transform);
			arrow.transform.Reset();
			arrow.SetActive(value: false);
		}
		else
		{
			wideMap = GetComponentInParent<InventoryWideMap>();
			if ((bool)wideMap)
			{
				wideMap.PlacedCompassIcon += OnWideMapPlacedCompassIcon;
			}
		}
		base.gameObject.SetActive(value: false);
		if ((bool)gameMap)
		{
			viewportEdge = gameMap.ViewportEdge;
		}
	}

	private void OnDestroy()
	{
		if ((bool)gameMap)
		{
			gameMap.UpdateQuickMapDisplay -= OnGameMapUpdateQuickMapDisplay;
			gameMap.ViewPosUpdated -= OnGameMapViewPosUpdated;
		}
		if ((bool)wideMap)
		{
			wideMap.PlacedCompassIcon -= OnWideMapPlacedCompassIcon;
		}
	}

	private void OnWideMapPlacedCompassIcon()
	{
		OnGameMapUpdateQuickMapDisplay(isQuickMap: false, MapZone.NONE);
	}

	private void OnGameMapUpdateQuickMapDisplay(bool isQuickMap, MapZone currentMapZone)
	{
		if (IsActive(isQuickMap, currentMapZone))
		{
			base.gameObject.SetActive(value: true);
			if (isQuickMap)
			{
				base.transform.SetLocalPosition2D(initialPos);
				wasOutsideView = false;
				arrow.SetActive(value: false);
			}
		}
		else
		{
			base.gameObject.SetActive(value: false);
		}
		GameMapPinLayout componentInParent = GetComponentInParent<GameMapPinLayout>();
		if ((bool)componentInParent)
		{
			componentInParent.DoLayout();
		}
	}

	private void OnGameMapViewPosUpdated(Vector2 pos)
	{
		if (!base.gameObject.activeSelf)
		{
			return;
		}
		Vector3 vector = base.transform.parent.TransformPoint(initialPos);
		bool flag = !viewportEdge.OverlapPoint(vector);
		Vector2 vector2 = (flag ? ((Vector2)base.transform.parent.InverseTransformPoint(viewportEdge.ClosestPoint(vector))) : initialPos);
		base.transform.SetLocalPosition2D(vector2);
		if (flag)
		{
			if (!wasOutsideView)
			{
				arrow.SetActive(value: true);
			}
			float rotation = (initialPos - vector2).DirectionToAngle();
			arrow.transform.SetLocalRotation2D(rotation);
		}
		else if (wasOutsideView)
		{
			arrow.SetActive(value: false);
		}
		wasOutsideView = flag;
	}

	public void SetPosition(Vector2 position)
	{
		initialPos = position;
	}

	protected abstract bool IsActive(bool isQuickMap, MapZone currentMapZone);
}
