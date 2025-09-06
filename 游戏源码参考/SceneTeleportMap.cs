using System;
using System.Collections.Generic;
using GlobalEnums;
using UnityEngine;

public class SceneTeleportMap : ScriptableObject
{
	[Serializable]
	public class SceneInfo
	{
		public string SceneFileHash;

		public MapZone MapZone;

		public List<string> TransitionGates = new List<string>();

		public List<string> RespawnPoints = new List<string>();
	}

	[Serializable]
	private class SerializableSceneInfo : SerializableNamedData<SceneInfo>
	{
	}

	[Serializable]
	private class SerializableSceneList : SerializableNamedList<SceneInfo, SerializableSceneInfo>
	{
		public SceneInfo GetSceneInfo(string sceneName)
		{
			if (RuntimeData.TryGetValue(sceneName, out var value))
			{
				return value;
			}
			SceneInfo sceneInfo = new SceneInfo();
			RuntimeData[sceneName] = sceneInfo;
			return sceneInfo;
		}

		public Dictionary<string, SceneInfo> GetAllSceneInfo()
		{
			return RuntimeData;
		}
	}

	private const string OBJECT_NAME = "SceneTeleportMap";

	[SerializeField]
	private int lintVer;

	[SerializeField]
	private SerializableSceneList sceneList;

	private static SceneTeleportMap _instance;

	private static SceneTeleportMap Instance
	{
		get
		{
			if ((bool)_instance)
			{
				return _instance;
			}
			Load();
			return _instance;
		}
	}

	public static int LintVer
	{
		get
		{
			if (!Instance)
			{
				return -1;
			}
			return _instance.lintVer;
		}
		set
		{
			if ((bool)Instance)
			{
				_instance.lintVer = value;
				_instance.SetDirty();
			}
		}
	}

	[RuntimeInitializeOnLoadMethod]
	private static void Load()
	{
		_instance = Resources.Load<SceneTeleportMap>("SceneTeleportMap");
	}

	public static void AddTransitionGate(string sceneName, string gateName)
	{
		if ((bool)Instance)
		{
			SceneInfo sceneInfo = _instance.sceneList.GetSceneInfo(sceneName);
			if (!sceneInfo.TransitionGates.Contains(gateName))
			{
				sceneInfo.TransitionGates.Add(gateName);
				_instance.SetDirty();
			}
		}
	}

	public static void AddRespawnPoint(string sceneName, string pointName)
	{
		if ((bool)Instance)
		{
			SceneInfo sceneInfo = _instance.sceneList.GetSceneInfo(sceneName);
			if (!sceneInfo.RespawnPoints.Contains(pointName))
			{
				sceneInfo.RespawnPoints.Add(pointName);
				_instance.SetDirty();
			}
		}
	}

	public static void AddMapZone(string sceneName, MapZone mapZone)
	{
		if ((bool)Instance)
		{
			_instance.sceneList.GetSceneInfo(sceneName).MapZone = mapZone;
			_instance.SetDirty();
		}
	}

	public static void RecordHash(string sceneName, string hash)
	{
		if ((bool)Instance)
		{
			_instance.sceneList.GetSceneInfo(sceneName).SceneFileHash = hash;
			_instance.SetDirty();
		}
	}

	public static void ClearInSceneLists(string sceneName)
	{
		if ((bool)Instance)
		{
			SceneInfo sceneInfo = _instance.sceneList.GetSceneInfo(sceneName);
			sceneInfo.TransitionGates.Clear();
			sceneInfo.RespawnPoints.Clear();
			_instance.SetDirty();
		}
	}

	public static Dictionary<string, SceneInfo> GetTeleportMap()
	{
		if (!Instance)
		{
			return null;
		}
		return _instance.sceneList.GetAllSceneInfo();
	}

	public new void SetDirty()
	{
	}
}
