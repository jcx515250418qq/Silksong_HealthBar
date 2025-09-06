using GlobalEnums;
using UnityEngine;

public class MapNextAreaDisplay : MonoBehaviour
{
	[SerializeField]
	private GameMap gameMap;

	[SerializeField]
	[PlayerDataField(typeof(bool), false)]
	private string visitedString;

	[SerializeField]
	private MapPinConditional.Condition visibleCondition;

	[SerializeField]
	private string sceneVisited;

	private PlayerData pd;

	private void Reset()
	{
		gameMap = GetComponentInParent<GameMap>();
	}

	private void Awake()
	{
		gameMap.UpdateQuickMapDisplay += Refresh;
	}

	private void OnDestroy()
	{
		gameMap.UpdateQuickMapDisplay -= Refresh;
	}

	private void Refresh(bool display, MapZone _)
	{
		if (pd == null)
		{
			pd = GameManager.instance.playerData;
		}
		bool flag = string.IsNullOrEmpty(visitedString);
		if (!flag)
		{
			flag = pd.GetBool(visitedString);
		}
		if (flag && !visibleCondition.IsFulfilled)
		{
			flag = false;
		}
		if (flag && !string.IsNullOrEmpty(sceneVisited) && !pd.scenesVisited.Contains(sceneVisited))
		{
			flag = false;
		}
		if (flag && display)
		{
			GameMapScene componentInParent = base.gameObject.GetComponentInParent<GameMapScene>(includeInactive: true);
			if ((bool)componentInParent)
			{
				if (!componentInParent.IsMapped)
				{
					flag = false;
				}
			}
			else
			{
				Debug.LogError("Next area display \"" + base.gameObject.name + "\" in \"" + base.transform.parent.name + "\" did not have map scene parent.");
			}
		}
		if (base.gameObject.activeSelf)
		{
			if (!flag || !display)
			{
				base.gameObject.SetActive(value: false);
			}
		}
		else if (flag && display)
		{
			base.gameObject.SetActive(value: true);
		}
	}
}
