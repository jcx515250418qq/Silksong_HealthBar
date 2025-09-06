using GlobalEnums;
using UnityEngine;

public class DisplayOnWorldMapOnly : MonoBehaviour
{
	private enum UpdateState
	{
		Never = 0,
		Normal = 1,
		QuickMap = 2
	}

	[SerializeField]
	private GameMap gameMap;

	private GameMapScene parentScene;

	private Renderer renderer;

	private bool hasEverRefreshed;

	private static UpdateState updateState;

	private void Reset()
	{
		gameMap = GetComponentInParent<GameMap>(includeInactive: true);
	}

	private void Awake()
	{
		gameMap = GetComponentInParent<GameMap>();
		gameMap.UpdateQuickMapDisplay += Refresh;
		renderer = GetComponent<Renderer>();
		parentScene = GetComponentInParent<GameMapScene>(includeInactive: true);
	}

	private void Start()
	{
		if (updateState != 0)
		{
			Refresh(updateState == UpdateState.QuickMap, MapZone.NONE);
		}
	}

	private void OnDestroy()
	{
		gameMap.UpdateQuickMapDisplay -= Refresh;
	}

	private void Refresh(bool isQuickMap, MapZone _)
	{
		updateState = ((!isQuickMap) ? UpdateState.Normal : UpdateState.QuickMap);
		if ((bool)renderer)
		{
			renderer.enabled = !isQuickMap && (!parentScene || parentScene.IsMapped || parentScene.InitialState != GameMapScene.States.Hidden);
		}
	}
}
