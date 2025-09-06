using System.Collections.Generic;
using UnityEngine;

public sealed class GUIDrawer : MonoBehaviour
{
	private static bool hasInstance;

	private static GUIDrawer instance;

	private static bool isDirty;

	private static List<IOnGUI> runList = new List<IOnGUI>();

	private static HashSet<IOnGUI> drawers = new HashSet<IOnGUI>();

	private static Comparer<IOnGUI> comparer = Comparer<IOnGUI>.Create((IOnGUI a, IOnGUI b) => a.GUIDepth.CompareTo(b.GUIDepth));

	private void Awake()
	{
		if ((bool)instance && instance != this)
		{
			Object.Destroy(this);
			return;
		}
		instance = this;
		hasInstance = true;
		Object.DontDestroyOnLoad(base.gameObject);
		ToggleDrawer(drawers.Count > 0);
	}

	private void OnDestroy()
	{
		if (instance == this)
		{
			instance = null;
			hasInstance = false;
		}
	}

	public static void InsertInOrder(List<IOnGUI> list, IOnGUI item)
	{
		int num = list.BinarySearch(item, comparer);
		if (num < 0)
		{
			num = ~num;
		}
		list.Insert(num, item);
	}

	private void OnGUI()
	{
		if (isDirty)
		{
			isDirty = false;
			drawers.RemoveWhere((IOnGUI o) => o == null);
			runList.Clear();
			if (runList.Capacity < drawers.Count)
			{
				runList.Capacity = drawers.Count;
			}
			foreach (IOnGUI drawer in drawers)
			{
				InsertInOrder(runList, drawer);
			}
		}
		else
		{
			runList.RemoveAll((IOnGUI o) => o == null);
		}
		foreach (IOnGUI run in runList)
		{
			run.DrawGUI();
		}
		if (runList.Count == 0)
		{
			ToggleDrawer(enabled: false);
		}
	}

	public static void AddDrawer(IOnGUI drawer)
	{
		if (!hasInstance)
		{
			new GameObject("GUI Drawer", typeof(GUIDrawer));
		}
		if (drawers.Add(drawer))
		{
			isDirty = true;
			if (hasInstance)
			{
				ToggleDrawer(enabled: true);
			}
		}
	}

	public static void RemoveDrawer(IOnGUI drawer)
	{
		if (drawers.Remove(drawer))
		{
			isDirty = true;
			if (drawers.Count == 0)
			{
				ToggleDrawer(enabled: false);
			}
		}
	}

	private static void ToggleDrawer(bool enabled)
	{
		if (hasInstance)
		{
			instance.enabled = enabled;
		}
	}
}
