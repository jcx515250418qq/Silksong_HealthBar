using System;
using System.Collections.Generic;
using System.Linq;
using TeamCherry.SharedUtils;
using UnityEngine;

public class MazeController : MonoBehaviour
{
	[Serializable]
	private class EntryMatch
	{
		public string EntryScene;

		public string EntryDoorDir;

		public string ExitDoorDir;

		public MinMaxFloat FogRotationRange;
	}

	[SerializeField]
	[Tooltip("Is this scene an entry or exit scene?")]
	private bool isCapScene;

	[SerializeField]
	private List<TransitionPoint> entryDoors;

	[Space]
	[SerializeField]
	private string[] sceneNames;

	[SerializeField]
	private int neededCorrectDoors;

	[SerializeField]
	private int allowedIncorrectDoors;

	[Space]
	[SerializeField]
	private int restScenePoint;

	[SerializeField]
	private string restSceneName;

	[Space]
	[SerializeField]
	private string exitSceneName;

	[SerializeField]
	private EntryMatch[] entryMatchExit;

	[Space]
	[SerializeField]
	private SetMaterialPropertyBlocks[] fogPropertyControllers;

	[Space]
	[SerializeField]
	private bool startInactive;

	private bool isActive;

	private bool forceExit;

	private readonly List<TransitionPoint> correctDoors = new List<TransitionPoint>();

	private static readonly int _fogRotationProp = Shader.PropertyToID("_FogRotation");

	public bool IsCapScene => isCapScene;

	public IReadOnlyCollection<string> SceneNames => (IReadOnlyCollection<string>)(object)sceneNames;

	public bool IsDoorLinkComplete { get; private set; }

	public int CorrectDoorsLeft
	{
		get
		{
			PlayerData instance = PlayerData.instance;
			return neededCorrectDoors - instance.CorrectMazeDoorsEntered;
		}
	}

	public int IncorrectDoorsLeft
	{
		get
		{
			PlayerData instance = PlayerData.instance;
			return allowedIncorrectDoors - instance.IncorrectMazeDoorsEntered;
		}
	}

	public static MazeController NewestInstance { get; private set; }

	public event Action DoorsLinked;

	private void OnValidate()
	{
		if (neededCorrectDoors < 1)
		{
			neededCorrectDoors = 1;
		}
		if (allowedIncorrectDoors < 1)
		{
			allowedIncorrectDoors = 1;
		}
		if (restScenePoint > neededCorrectDoors)
		{
			restScenePoint = neededCorrectDoors;
		}
		if (!Application.isPlaying)
		{
			Shader.SetGlobalFloat(_fogRotationProp, 0f);
		}
	}

	private void Awake()
	{
		OnValidate();
		NewestInstance = this;
	}

	private void OnDestroy()
	{
		if (NewestInstance == this)
		{
			NewestInstance = null;
		}
	}

	private void Start()
	{
		if (!startInactive)
		{
			Activate();
		}
	}

	private EntryMatch GetExitMatch()
	{
		PlayerData instance = PlayerData.instance;
		EntryMatch[] array = entryMatchExit;
		foreach (EntryMatch entryMatch in array)
		{
			if (entryMatch.EntryScene == instance.MazeEntranceScene && instance.MazeEntranceDoor.StartsWith(entryMatch.EntryDoorDir))
			{
				return entryMatch;
			}
		}
		throw new UnityException("Ne exit matches found for maze entry scene");
	}

	public void Activate()
	{
		if (isActive)
		{
			return;
		}
		isActive = true;
		List<TransitionPoint> list = ((isCapScene || entryDoors.Count > 0) ? entryDoors : TransitionPoint.TransitionPoints.Where((TransitionPoint door) => door.gameObject.scene == base.gameObject.scene).ToList());
		PlayerData instance = PlayerData.instance;
		if (base.gameObject.scene.name == "Dust_Maze_Last_Hall" && instance.PreviousMazeScene != exitSceneName)
		{
			instance.CorrectMazeDoorsEntered = neededCorrectDoors + 1;
			instance.IncorrectMazeDoorsEntered = 0;
			forceExit = true;
		}
		foreach (TransitionPoint item in list)
		{
			SubscribeDoorEntered(item);
		}
		LinkDoors(list);
		if (isCapScene)
		{
			ResetSaveData();
		}
		else
		{
			float t = Mathf.Clamp01((float)instance.CorrectMazeDoorsEntered / (float)neededCorrectDoors);
			float lerpedValue = GetExitMatch().FogRotationRange.GetLerpedValue(t);
			SetMaterialPropertyBlocks[] array = fogPropertyControllers;
			foreach (SetMaterialPropertyBlocks setMaterialPropertyBlocks in array)
			{
				if ((bool)setMaterialPropertyBlocks)
				{
					setMaterialPropertyBlocks.SetFloatModifier("_FogRotation", lerpedValue);
				}
			}
		}
		IsDoorLinkComplete = true;
		this.DoorsLinked?.Invoke();
	}

	private void LinkDoors(IReadOnlyList<TransitionPoint> totalDoors)
	{
		correctDoors.Clear();
		var list = (from door in totalDoors.Select(delegate(TransitionPoint door)
			{
				if (door == null)
				{
					return (_003C_003Ef__AnonymousType4<TransitionPoint, string, string>)null;
				}
				string doorName = door.name;
				string doorDirMatch = GetDoorDirMatch(doorName);
				return string.IsNullOrEmpty(doorDirMatch) ? null : new
				{
					Door = door,
					DoorName = doorName,
					TargetDoorMatch = doorDirMatch
				};
			})
			where door != null
			select door).ToList();
		Dictionary<string, SceneTeleportMap.SceneInfo> teleportMap = SceneTeleportMap.GetTeleportMap();
		List<KeyValuePair<string, SceneTeleportMap.SceneInfo>> list2 = teleportMap.Where((KeyValuePair<string, SceneTeleportMap.SceneInfo> kvp) => kvp.Key != base.gameObject.scene.name && sceneNames.Contains(kvp.Key)).ToList();
		PlayerData instance = PlayerData.instance;
		int num = list.Count;
		int num2 = (instance.hasNeedolin ? UnityEngine.Random.Range(0, num) : (-1));
		list.Shuffle();
		int num3 = neededCorrectDoors - 1;
		if (!isCapScene)
		{
			string sceneName;
			IReadOnlyList<string> readOnlyList;
			bool flag;
			if (instance.CorrectMazeDoorsEntered >= num3)
			{
				sceneName = exitSceneName;
				if (forceExit)
				{
					readOnlyList = teleportMap[exitSceneName].TransitionGates;
				}
				else
				{
					EntryMatch exitMatch = GetExitMatch();
					readOnlyList = ((exitMatch == null) ? teleportMap[exitSceneName].TransitionGates : teleportMap[exitSceneName].TransitionGates.Where((string gate) => gate.StartsWith(exitMatch.ExitDoorDir)).ToList());
				}
				flag = true;
			}
			else
			{
				if (instance.CorrectMazeDoorsEntered < restScenePoint - 1 || instance.EnteredMazeRestScene)
				{
					goto IL_0229;
				}
				sceneName = restSceneName;
				readOnlyList = teleportMap[restSceneName].TransitionGates;
				flag = false;
			}
			for (int num4 = list.Count - 1; num4 >= 0; num4--)
			{
				var anon = list[num4];
				TransitionPoint door2 = anon.Door;
				string doorName2 = anon.DoorName;
				if (string.IsNullOrEmpty(instance.PreviousMazeTargetDoor) || !(instance.PreviousMazeTargetDoor == doorName2))
				{
					bool flag2 = false;
					foreach (string item2 in readOnlyList)
					{
						if (TryMatchDoor(sceneName, item2, anon.TargetDoorMatch, door2, isCorrectDoor: true))
						{
							flag2 = true;
							break;
						}
					}
					if (flag2)
					{
						num2 = -1;
						list.RemoveAt(num4);
						num--;
						if (flag)
						{
							break;
						}
					}
				}
			}
		}
		goto IL_0229;
		IL_0229:
		for (int i = 0; i < num; i++)
		{
			var anon2 = list[i];
			TransitionPoint door3 = anon2.Door;
			string doorName3 = anon2.DoorName;
			if (!string.IsNullOrEmpty(instance.PreviousMazeTargetDoor) && doorName3 == instance.PreviousMazeTargetDoor)
			{
				door3.SetTargetScene(instance.PreviousMazeScene);
				door3.entryPoint = instance.PreviousMazeDoor;
				if (instance.PreviousMazeScene == exitSceneName)
				{
					num2 = i;
				}
				else if (num > 1)
				{
					while (num2 == i)
					{
						num2 = UnityEngine.Random.Range(0, num);
					}
				}
				if (num2 == i)
				{
					correctDoors.Add(door3);
				}
				continue;
			}
			list2.Shuffle();
			if (!string.IsNullOrEmpty(instance.PreviousMazeScene))
			{
				for (int num5 = list2.Count - 1; num5 >= 0; num5--)
				{
					KeyValuePair<string, SceneTeleportMap.SceneInfo> item = list2[num5];
					if (item.Key == instance.PreviousMazeScene)
					{
						list2.RemoveAt(num5);
						list2.Add(item);
					}
				}
			}
			foreach (KeyValuePair<string, SceneTeleportMap.SceneInfo> item3 in list2)
			{
				if (TryFindMatchingDoor(item3.Key, item3.Value.TransitionGates, anon2.TargetDoorMatch, door3, num2 == i))
				{
					break;
				}
			}
		}
		if (!isCapScene && correctDoors.Count <= 0 && num2 >= 0)
		{
			LinkDoors(totalDoors);
		}
	}

	private string GetDoorDirMatch(string doorName)
	{
		if (doorName.StartsWith("left"))
		{
			return "right";
		}
		if (doorName.StartsWith("right"))
		{
			return "left";
		}
		if (doorName.StartsWith("top"))
		{
			return "bot";
		}
		if (doorName.StartsWith("bot"))
		{
			return "top";
		}
		return null;
	}

	private bool TryFindMatchingDoor(string sceneName, List<string> transitionGates, string targetDoorMatch, TransitionPoint door, bool isCorrectDoor)
	{
		foreach (string transitionGate in transitionGates)
		{
			if (TryMatchDoor(sceneName, transitionGate, targetDoorMatch, door, isCorrectDoor))
			{
				return true;
			}
		}
		return false;
	}

	private bool TryMatchDoor(string sceneName, string doorName, string targetDoorMatch, TransitionPoint door, bool isCorrectDoor)
	{
		if (!doorName.StartsWith(targetDoorMatch))
		{
			return false;
		}
		door.SetTargetScene(sceneName);
		door.entryPoint = doorName;
		if (isCorrectDoor)
		{
			correctDoors.Add(door);
		}
		return true;
	}

	private void SubscribeDoorEntered(TransitionPoint door)
	{
		door.OnBeforeTransition += delegate
		{
			PlayerData instance = PlayerData.instance;
			string text = door.name;
			if (!isCapScene)
			{
				if (door.targetScene == restSceneName)
				{
					instance.EnteredMazeRestScene = true;
					instance.CorrectMazeDoorsEntered = neededCorrectDoors - restScenePoint;
					instance.IncorrectMazeDoorsEntered = 0;
				}
				else if (!(instance.PreviousMazeTargetDoor == text))
				{
					if (correctDoors.Contains(door))
					{
						instance.CorrectMazeDoorsEntered++;
						instance.IncorrectMazeDoorsEntered = 0;
					}
					else
					{
						instance.CorrectMazeDoorsEntered = 0;
						instance.IncorrectMazeDoorsEntered++;
						instance.EnteredMazeRestScene = false;
						if (instance.IncorrectMazeDoorsEntered >= allowedIncorrectDoors && text.StartsWith("right"))
						{
							door.SetTargetScene("Dust_Maze_09_entrance");
							door.entryPoint = "left1";
						}
					}
				}
			}
			instance.PreviousMazeTargetDoor = door.entryPoint;
			instance.PreviousMazeScene = door.gameObject.scene.name;
			instance.PreviousMazeDoor = text;
		};
	}

	public static void ResetSaveData()
	{
		PlayerData instance = PlayerData.instance;
		instance.PreviousMazeTargetDoor = string.Empty;
		instance.PreviousMazeScene = string.Empty;
		instance.PreviousMazeDoor = string.Empty;
		instance.CorrectMazeDoorsEntered = 0;
		instance.IncorrectMazeDoorsEntered = 0;
		instance.EnteredMazeRestScene = false;
	}

	public IEnumerable<TransitionPoint> EnumerateCorrectDoors()
	{
		foreach (TransitionPoint correctDoor in correctDoors)
		{
			yield return correctDoor;
		}
	}
}
