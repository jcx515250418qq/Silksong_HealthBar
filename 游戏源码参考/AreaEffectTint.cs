using System;
using GlobalEnums;
using GlobalSettings;
using TeamCherry.SharedUtils;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AreaEffectTint : MonoBehaviour
{
	public enum Area
	{
		None = 0,
		Swamp = 1,
		Abyss = 2
	}

	[SerializeField]
	private TintRendererGroup tinter;

	private Color initialColor;

	private static bool registeredEvent;

	private static bool sceneDirty = true;

	private static Area activeArea;

	private void Awake()
	{
		if ((bool)tinter)
		{
			initialColor = tinter.Color;
		}
		RegisterEvents();
	}

	private void OnEnable()
	{
		DoTint();
	}

	public void DoTint()
	{
		if ((bool)tinter)
		{
			if (IsActive(base.transform.position, out var tintColor) || (MaggotRegion.IsInsideAny && GetComponentInParent<HeroController>() != null))
			{
				tinter.Color = initialColor * tintColor;
			}
			else
			{
				tinter.Color = initialColor;
			}
		}
	}

	private void OnApplicationQuit()
	{
		UnregisterEvents();
	}

	private static void RegisterEvents()
	{
		if (!registeredEvent)
		{
			registeredEvent = true;
			SceneManager.activeSceneChanged += SceneManagerOnActiveSceneChanged;
		}
	}

	private static void SceneManagerOnActiveSceneChanged(Scene arg0, Scene arg1)
	{
		sceneDirty = true;
	}

	private static void UnregisterEvents()
	{
		if (registeredEvent)
		{
			registeredEvent = false;
			SceneManager.activeSceneChanged -= SceneManagerOnActiveSceneChanged;
		}
	}

	public static bool IsActive(Vector2 pos, out Color tintColor)
	{
		if (SwampZone.IsInside(pos))
		{
			tintColor = Effects.MossEffectsTintDust;
			return true;
		}
		switch (GetActiveAreaInScene())
		{
		case Area.None:
			tintColor = Color.white;
			return false;
		case Area.Swamp:
			tintColor = Effects.MossEffectsTintDust;
			return true;
		case Area.Abyss:
			tintColor = Color.black;
			return true;
		default:
			throw new ArgumentOutOfRangeException();
		}
	}

	private static Area GetActiveAreaInScene()
	{
		if (!sceneDirty)
		{
			return activeArea;
		}
		GameManager instance = GameManager.instance;
		string sceneNameString = instance.GetSceneNameString();
		switch (instance.GetCurrentMapZoneEnum())
		{
		case MapZone.DUSTPENS:
			activeArea = ((!(sceneNameString == "Shadow_Weavehome")) ? Area.Swamp : Area.None);
			break;
		case MapZone.SWAMP:
			activeArea = ((!(sceneNameString == "Shadow_24")) ? Area.Swamp : Area.None);
			break;
		case MapZone.AQUEDUCT:
			activeArea = ((!(sceneNameString == "Aqueduct_05")) ? Area.Swamp : Area.None);
			break;
		case MapZone.ABYSS:
			activeArea = Area.Abyss;
			break;
		default:
			activeArea = Area.None;
			break;
		}
		sceneDirty = false;
		return activeArea;
	}
}
